using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;
using Xunit;
using F = Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class SystemMethods : IClassFixture<SocketFixture>
    {

        [Fact]
        public async Task Methods() 
        {
            byte[] file = System.IO.File.ReadAllBytes("File.rsf");
            const string brickFilePath = "../prjs/xunit/File.rsf";
            const string brickDirectoryPath = "../prjs/xunit/";
            bool success;

            success = await F.SystemMethods.CreateDirectory(Socket.Instance, brickDirectoryPath);
            Assert.True(success);

            success = await F.SystemMethods.UploadFile(Socket.Instance, file, brickFilePath);
            Assert.True(success);


            byte[] data = await F.SystemMethods.DownLoadFile(Socket.Instance, brickFilePath);
            Assert.Equal(data, file);

            DirectoryContent content = await F.SystemMethods.GetDirectoryContent(Socket.Instance, brickDirectoryPath);
            Assert.True(content.Files.Length == 1);

            //can not delete non empty directory
            success = await F.SystemMethods.Delete(Socket.Instance, brickDirectoryPath);
            Assert.False(success);

            success = await F.SystemMethods.Delete(Socket.Instance, brickFilePath);
            Assert.True(success);


            content = await F.SystemMethods.GetDirectoryContent(Socket.Instance, brickDirectoryPath);
            Assert.True(content.Files.Length == 0);

            success = await F.SystemMethods.Delete(Socket.Instance, brickDirectoryPath);
            Assert.True(success);
        }
    }
}
