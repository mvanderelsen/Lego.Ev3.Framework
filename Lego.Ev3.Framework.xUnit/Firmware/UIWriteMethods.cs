using System.Threading.Tasks;
using Xunit;
using F = Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class UIWriteMethods
    {
        [Fact]
        public async Task Led() 
        {
            await F.UIWriteMethods.Led(Socket.Instance, LedMode.Off, true); //await reply
            await Task.Delay(500);
            await F.UIWriteMethods.Led(Socket.Instance, LedMode.Green);
        }
    }
}
