using System.Collections.Concurrent;

namespace Lego.Ev3.Framework.Sockets
{
    internal abstract class SocketBase
    {
        public abstract string ConnectionInfo { get; }

        public ConcurrentDictionary<ushort, byte[]> Responses { get; } = new ConcurrentDictionary<ushort, byte[]>();

        public ConcurrentQueue<byte[]> Commands { get; } = new ConcurrentQueue<byte[]>();

        public ConcurrentQueue<byte[]> Events { get; } = new ConcurrentQueue<byte[]>();

        protected void Clear()
        {
            Responses.Clear();
            while (Commands.Count > 0) { Commands.TryDequeue(out _); }
            while (Events.Count > 0) { Events.TryDequeue(out _); }
        }
    }
}
