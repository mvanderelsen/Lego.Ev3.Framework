using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    public interface ISocket
    {
        Task<Response> Execute(Command command);
    }
}
