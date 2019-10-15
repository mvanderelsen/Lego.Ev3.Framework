using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Memory
    /// </summary>
    public class Memory
    {
        internal Memory() { }

        /// <summary>
        /// Gets memory information total and free.
        /// </summary>
        /// <returns></returns>
        public async Task<MemoryInfo> GetMemoryInfo()
        {
            return await MemoryMethods.GetMemoryInfo(Brick.Socket);
        }
    }
}
