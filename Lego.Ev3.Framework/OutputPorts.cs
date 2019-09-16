using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All output ports on a brick
    /// </summary>
    public sealed class OutputPorts
    {
        /// <summary>
        /// The brick layer
        /// </summary>
        internal ChainLayer Layer { get; private set; }

        /// <summary>
        /// Port A
        /// </summary>
        public OutputPort A { get; private set; }

        /// <summary>
        /// Port B
        /// </summary>
        public OutputPort B { get; private set; }

        /// <summary>
        /// Port C
        /// </summary>
        public OutputPort C { get; private set; }

        /// <summary>
        /// Port D
        /// </summary>
        public OutputPort D { get; private set; }


        /// <summary>
        /// Construct all 4 output ports
        /// </summary>
        /// <param name="layer">The brick layer</param>
        internal OutputPorts(ChainLayer layer)
        {
            Layer = layer;
            A = new OutputPort(layer, OutputPortName.A);
            B = new OutputPort(layer, OutputPortName.B);
            C = new OutputPort(layer, OutputPortName.C);
            D = new OutputPort(layer, OutputPortName.D);
        }


        #region Firmware Methods

        /// <summary>
        /// This function enables specifying the output device type
        /// </summary>
        /// <param name="port">The Name of the port</param>
        /// <param name="type">The output device type to set</param>
        public async Task SetType(OutputPortName port, DeviceType type)
        {
            await OutputMethods.SetType(Brick.Socket, Layer, port, type);
        }

        /// <summary>
        /// This function enables resetting the tacho count
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Reset(OutputPortNames portFlag)
        {
            await OutputMethods.Reset(Brick.Socket, Layer, portFlag);
        }

        /// <summary>
        /// This function sends stop to device
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task Stop(OutputPortNames portFlag, Brake brake = Brake.Float)
        {
            await OutputMethods.Stop(Brick.Socket, Layer, portFlag, brake);
        }

        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public async Task SetPower(OutputPortNames portFlag, int power)
        {
            await OutputMethods.SetPower(Brick.Socket, Layer, portFlag, power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the device. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public async Task SetSpeed(OutputPortNames portFlag, int speed)
        {
            await OutputMethods.SetSpeed(Brick.Socket, Layer, portFlag, speed);
        }

        /// <summary>
        /// This function enables starting the device
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Start(OutputPortNames portFlag)
        {
            await OutputMethods.Start(Brick.Socket, Layer, portFlag);
        }

        /// <summary>
        /// This function sets the polarity of the device.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="polarity">Polarity -1 : backward 0 : opposite direction 1 : forward</param>
        public async Task SetPolarity(OutputPortNames portFlag, Polarity polarity)
        {
            await OutputMethods.SetPolarity(Brick.Socket, Layer, portFlag, polarity);
        }

        /// <summary>
        /// This function enables the program to test if a output device is busy.
        /// </summary>
        public async Task<bool> Test(OutputPortNames portFlag)
        {
            return await OutputMethods.IsBusy(Brick.Socket, Layer, portFlag);
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
        public async Task StepPower(OutputPortNames portFlag, int power, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepPower(Brick.Socket, Layer, portFlag, power, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
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
        public async Task TimePower(OutputPortNames portFlag, int power, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.TimePower(Brick.Socket, Layer, portFlag, power, timeRampUp, timeContinuesRun, timeRampDown, brake);
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
        public async Task StepSpeed(OutputPortNames portFlag, int speed, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepPower(Brick.Socket, Layer, portFlag, speed, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
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
        public async Task TimeSpeed(OutputPortNames portFlag, int speed, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.TimeSpeed(Brick.Socket, Layer, portFlag, speed, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }

        /// <summary>
        /// This function enables the program to clear the tacho count used as sensor input.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task ClearCount(OutputPortNames portFlag)
        {
            await OutputMethods.ClearCount(Brick.Socket, Layer, portFlag);
        }


        //TODO
        /*
        OutputMethods
        internal static async Task Read(Socket socket, ChainLayer layer, OutputPortName port)
        internal static async Task Ready(Socket socket, ChainLayer layer, OutputPortNames ports)
        internal static async Task GetCount(Socket socket, ChainLayer layer, OutputPortNames ports)
        internal static async Task ProgramStop(Socket socket)
        */

        #endregion
    }
}
