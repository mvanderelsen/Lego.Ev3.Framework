using Lego.Ev3.Framework.Firmware;
using System.Collections.Concurrent;

namespace Lego.Ev3.Framework.Sockets
{
    internal abstract class SocketBase
    {
        public abstract string ConnectionInfo { get; }

        public ConcurrentDictionary<ushort, byte[]> Responses { get; } = new ConcurrentDictionary<ushort, byte[]>();

        public ConcurrentQueue<Command> NoReplyCommands { get; } = new ConcurrentQueue<Command>();

        public ConcurrentQueue<Command> Commands { get; } = new ConcurrentQueue<Command>();

        public ConcurrentQueue<Command> Events { get; } = new ConcurrentQueue<Command>();

        protected void Clear()
        {
            Responses.Clear();
            while (NoReplyCommands.Count > 0) { NoReplyCommands.TryDequeue(out _); }
            while (Commands.Count > 0) { Commands.TryDequeue(out _); }
            while (Events.Count > 0) { Events.TryDequeue(out _); }
        }
    }
}
