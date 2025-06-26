using Lego.Ev3.Framework.Firmware;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal abstract class Socket : IDisposable, ISocket
    {
        public bool IsConnected { get; private set; }

        public abstract string ConnectionInfo { get; }

        private CancellationTokenSource _cancellationTokenSource;

        public CancellationToken CancellationToken { get; private set; }

        public ConcurrentDictionary<ushort, Command> Commands { get; } = [];
        public ConcurrentDictionary<ushort, byte[]> Responses { get; } = new ConcurrentDictionary<ushort, byte[]>();
        private ConcurrentQueue<Command> _buffer { get; } = new ConcurrentQueue<Command>();
        public ConcurrentQueue<Command> Events { get; } = new ConcurrentQueue<Command>();

        /// <summary>
        /// Connect to the EV3 brick.
        /// </summary>
        public async Task<bool> Connect(CancellationToken cancellationToken = default) 
        {
            if (IsConnected) return IsConnected;
            if (cancellationToken == default)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken = _cancellationTokenSource.Token;
            }
            else
            {
                CancellationToken = cancellationToken;
            }

            IsConnected = await ConnectSocket();
            if (IsConnected) Start();
            return IsConnected;
        }


        protected abstract Task<bool> ConnectSocket();


        /// <summary>
        /// Disconnect from the EV3 brick.
        /// </summary>
        public void Disconnect() 
        {
            if (!IsConnected) return;
            IsConnected = false;
            if (_cancellationTokenSource != null)
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
                catch { }
            }
            Responses.Clear();
            while (!_buffer.IsEmpty) { _buffer.TryDequeue(out _); }
            while (!Events.IsEmpty) { Events.TryDequeue(out _); }
            try
            {
                DisconnectSocket();
            }
            catch { };
        }

        private void Start() 
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    while (!CancellationToken.IsCancellationRequested)
                    {
                        if (_buffer.TryDequeue(out Command command))
                        {
                            await Write(command);
                            
                            Commands.TryAdd(command.Id, command);

                            if (command.NoReply)
                            {
                                Responses.TryAdd(command.Id, null);
                            }
                            else
                            {
                                await GetResponse(command.Id);
                            }
                        }

                        if (Events.TryDequeue(out command))
                        {
                            await Write(command);

                            Commands.TryAdd(command.Id, command);

                            await GetResponse(command.Id);
                        }

                        await Task.Delay(50, CancellationToken);

                    }

                }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
            catch (TaskCanceledException) 
            {
                Disconnect();
            }
        }

        private async Task GetResponse(ushort commandId, int retry = 0)
        {
            if (retry == 100) return;

            byte[] payLoad = await Read(commandId);
            if (payLoad != null && Response.GetId(payLoad) == commandId)
            {
               Responses.TryAdd(commandId, payLoad);
            }
            else 
            {
                await Task.Delay(10, CancellationToken);
                await GetResponse(commandId, retry + 1);
            }
        }

        protected abstract Task Write(Command command);

        protected abstract Task<byte[]> Read(ushort id);

        protected abstract void DisconnectSocket();

        public void Enqueue(Command command, bool isEvent) 
        {
            if (isEvent) Events.Enqueue(command);
            else _buffer.Enqueue(command);
        }

        #region disposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
