using Lego.Ev3.Framework.Firmware;
using System.Collections.Concurrent;

namespace Lego.Ev3.Framework.Sockets
{
    internal abstract class SocketBase
    {
        public abstract string ConnectionInfo { get; }

        public ConcurrentDictionary<ushort, byte[]> ResponseBuffer { get; } = new ConcurrentDictionary<ushort, byte[]>();

        public ConcurrentQueue<Command> NoReplyCommandBuffer { get; } = new ConcurrentQueue<Command>();

        public ConcurrentQueue<Command> CommandBuffer { get; } = new ConcurrentQueue<Command>();

        public ConcurrentQueue<Command> EventBuffer { get; } = new ConcurrentQueue<Command>();

        protected void Clear()
        {
            ResponseBuffer.Clear();
            while (NoReplyCommandBuffer.Count > 0) { NoReplyCommandBuffer.TryDequeue(out _); }
            while (CommandBuffer.Count > 0) { CommandBuffer.TryDequeue(out _); }
            while (EventBuffer.Count > 0) { EventBuffer.TryDequeue(out _); }
        }
    }
}
