using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// Socket
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// Executes a command on the socket
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Response</returns>
        Task<Response> Execute(Command command);
    }
}
