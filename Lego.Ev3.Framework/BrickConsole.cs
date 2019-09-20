using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A console to advanced brick functions
    /// </summary>
    public sealed class BrickConsole
    {
        internal BrickConsole() {}

        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="port"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public async Task<Format> GetFormat(InputPortName port, ChainLayer layer = ChainLayer.One)
        {
            int portNumber = port.AbsolutePortNumber(layer);
            return await InputMethods.GetFormat(Brick.Socket, portNumber);
        }

        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="port"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public async Task<Format> GetFormat(OutputPortName port, ChainLayer layer = ChainLayer.One)
        {
            int portNumber = port.AbsolutePortNumber(layer);
            return await InputMethods.GetFormat(Brick.Socket, portNumber);
        }


        /// <summary>
        /// Scans all possible ports (all chainlayers) for connected devices
        /// </summary>
        /// <returns>A list of devices with absolute chained portnumbers and devices.</returns>
        public async Task<IEnumerable<PortInfo>> PortScan()
        {
            return await InputMethods.PortScan(Brick.Socket);
        }

        /// <summary>
        /// Gets information about the brick: firmware, hardware, OS etc.
        /// </summary>
        /// <returns></returns>
        public async Task<BrickInfo> GetBrickInfo()
        {
            return await BrickInfo.GetBrickInfo();
        }

        /// <summary>
        /// This function enables specifying the output device type
        /// </summary>
        /// <param name="port">The Name of the port</param>
        /// <param name="type">The output device type to set</param>
        public async Task SetType(OutputPortName port, DeviceType type, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.SetType(Brick.Socket, layer, port, type);
        }

        /// <summary>
        /// This function enables resetting the tacho count
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Reset(OutputPortNames portFlag, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.Reset(Brick.Socket, layer, portFlag);
        }

        /// <summary>
        /// This function sends stop to device
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task Stop(OutputPortNames portFlag, Brake brake = Brake.Float, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.Stop(Brick.Socket, layer, portFlag, brake);
        }

        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public async Task SetPower(OutputPortNames portFlag, int power, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.SetPower(Brick.Socket, layer, portFlag, power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the device. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public async Task SetSpeed(OutputPortNames portFlag, int speed, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.SetSpeed(Brick.Socket, layer, portFlag, speed);
        }

        /// <summary>
        /// This function enables starting the device
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Start(OutputPortNames portFlag, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.Start(Brick.Socket, layer, portFlag);
        }

        /// <summary>
        /// This function sets the polarity of the device.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="polarity">Polarity -1 : backward 0 : opposite direction 1 : forward</param>
        public async Task SetPolarity(OutputPortNames portFlag, Polarity polarity, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.SetPolarity(Brick.Socket, layer, portFlag, polarity);
        }

        /// <summary>
        /// This function enables the program to test if a output device is busy.
        /// </summary>
        public async Task<bool> Test(OutputPortNames portFlag, ChainLayer layer = ChainLayer.One)
        {
            return await OutputMethods.IsBusy(Brick.Socket, layer, portFlag);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in tacho counts. 
        /// RampUp specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampDown specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task StepPower(OutputPortNames portFlag, int power, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.StepPower(Brick.Socket, layer, portFlag, power, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in time. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant power period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task TimePower(OutputPortNames portFlag, int power, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.TimePower(Brick.Socket, layer, portFlag, power, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in tacho counts. 
        /// The system will automatically adjust the power level to the device to keep the specified output speed. 
        /// RampDown specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampUp specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task StepSpeed(OutputPortNames portFlag, int speed, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.StepPower(Brick.Socket, layer, portFlag, speed, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in time. 
        /// The system will automatically adjust the power level to the device to keep the specified output speed. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant speed period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task TimeSpeed(OutputPortNames portFlag, int speed, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.TimeSpeed(Brick.Socket, layer, portFlag, speed, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }

        /// <summary>
        /// This function enables the program to clear the tacho count used as sensor input.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task ClearCount(OutputPortNames portFlag, ChainLayer layer = ChainLayer.One)
        {
            await OutputMethods.ClearCount(Brick.Socket, layer, portFlag);
        }

        /// <summary>
        /// Clear all device counters and values
        /// </summary>
        /// <returns></returns>
        public async Task Reset(ChainLayer layer = ChainLayer.One)
        {
            await InputMethods.Reset(Brick.Socket, layer);
        }

        //TODO
        /*
        OutputMethods
        internal static async Task Read(IExecute socket, ChainLayer layer, OutputPortName port)
        internal static async Task Ready(IExecute socket, ChainLayer layer, OutputPortNames ports)
        internal static async Task GetCount(IExecute socket, ChainLayer layer, OutputPortNames ports)
        internal static async Task ProgramStop(IExecute socket)
        */
    }
}
