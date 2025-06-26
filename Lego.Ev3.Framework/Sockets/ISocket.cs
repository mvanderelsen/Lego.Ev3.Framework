using Lego.Ev3.Framework.Firmware;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal interface ISocket
    {
        string ConnectionInfo { get; }

        bool IsConnected { get; }

        Task<bool> Connect(CancellationToken cancellationToken = default);

        void Disconnect();

        CancellationToken CancellationToken { get; }

        ConcurrentDictionary<ushort, byte[]> Responses { get; }

        ConcurrentQueue<Command> Events { get; }

        ConcurrentDictionary<ushort, Command> Commands { get; }

        void Enqueue(Command command, bool isEvent);

        void Dispose();
    }
}
