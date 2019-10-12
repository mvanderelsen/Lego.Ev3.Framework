using Lego.Ev3.Framework.Firmware;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal interface ISocket
    {
        string ConnectionInfo { get; }

        bool IsConnected { get; }

        Task<bool> Connect();

        Task Disconnect();

        CancellationToken CancellationToken { get; }

        ConcurrentDictionary<ushort, byte[]> Responses { get; }

        ConcurrentQueue<Command> NoReplyCommands { get; }

        ConcurrentQueue<Command> Commands { get; }

        ConcurrentQueue<Command> Events { get; }

        void Dispose();
    }
}
