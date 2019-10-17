using System.Threading.Tasks;
using Xunit;
using F = Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class UIButtonMethods
    {
        [Fact]
        public async Task GetClick() 
        {
            bool click;

            click = await F.UIButtonMethods.GetClick(Socket.Instance, ButtonType.Any);
            Assert.False(click);
            
            click = await F.UIButtonMethods.GetClick(Socket.Instance, ButtonType.None);
            Assert.True(click);

            click = await F.UIButtonMethods.GetClick(Socket.Instance, ButtonType.Up, ButtonMode.LongPress);
            Assert.False(click);

        }
    }
}
