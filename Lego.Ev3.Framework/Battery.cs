using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Battery
    /// </summary>
    public sealed class Battery
    {

        internal Battery()
        {

        }

        /// <summary>
        /// Gets all battery information
        /// </summary>
        /// <returns></returns>
        public async Task<BatteryInfo> GetBatteryInfo()
        {
           return await UIReadMethods.GetBattery(Brick.Socket);
         
        }

    }
}
