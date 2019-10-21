using System.Threading.Tasks;
using Xunit;
using F = Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class UIReadMethods
    {
        [Fact]
        public async Task GetBatteryValue()
        {
            await F.UIReadMethods.GetBatteryValue(Socket.Instance);
        }

        [Fact]
        public async Task GetOSVersion()
        {
            await F.UIReadMethods.GetOSVersion(Socket.Instance);
        }

        [Fact]
        public async Task GetOSBuild()
        {
            await F.UIReadMethods.GetOSBuild(Socket.Instance);
        }

        [Fact]
        public async Task GetHardwareVersion()
        {
            await F.UIReadMethods.GetHardwareVersion(Socket.Instance);
        }

        [Fact]
        public async Task GetFirmwareVersion()
        {
            await F.UIReadMethods.GetFirmwareVersion(Socket.Instance);
        }

        [Fact]
        public async Task GetFirmwareBuild()
        {
            await F.UIReadMethods.GetFirmwareBuild(Socket.Instance);
        }

        [Fact]
        public async Task GetWarnings()
        {
            await F.UIReadMethods.GetWarnings(Socket.Instance);
        }

        [Fact]
        public async Task GetVersion()
        {
           string version =  await F.UIReadMethods.GetVersion(Socket.Instance);
        }

        [Fact]
        public async Task GetIPAddress()
        {
            string ip = await F.UIReadMethods.GetIPAddress(Socket.Instance);
            Assert.Equal("", ip);
        }
    }
}
