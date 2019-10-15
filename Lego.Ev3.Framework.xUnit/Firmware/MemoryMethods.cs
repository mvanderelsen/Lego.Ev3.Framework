using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;
using Xunit;
using F = Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class MemoryMethods: IClassFixture<SocketFixture>
    {

        [Fact]
        public async Task MemoryInfo()
        {
            MemoryInfo memoryInfo = await F.MemoryMethods.GetMemoryInfo(Socket.Instance);
            Assert.Equal(6000, memoryInfo.Total);
        }


        [Fact]
        public async Task Exists() 
        {
            bool exists = await F.MemoryMethods.Exists(Socket.Instance, BrickExplorer.PROJECTS_PATH);
            Assert.True(exists);
        }

        [Fact]
        public async Task DirectoryInfo()
        {
            DirectoryInfo directoryInfo = await F.MemoryMethods.GetDirectoryInfo(Socket.Instance, BrickExplorer.ROOT_PATH);
            Assert.Equal(6, directoryInfo.Items);
        }
    }
}
