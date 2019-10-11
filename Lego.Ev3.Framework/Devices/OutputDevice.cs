using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Devices
{
    /// <summary>
    /// A device that can connected to the brick output ports.
    /// Use this class to derive from if adding a new custom output device.
    /// It contains all output methods protected so they can be exposed or used in deriving class for new custom output device.
    /// Call methods only after connection to the brick is made.
    /// </summary>
    public abstract class OutputDevice : Device, IOutputDevice
    {
        #region Internal Port members
        internal ChainLayer Layer { get; set; }
        internal OutputPortName PortName { get; set; }
        internal int PortNumber { get; set; }
        internal OutputPortNames PortNames { get; set; }
        #endregion

        /// <summary>
        /// Sets or gets the general device mode [0-7]
        /// </summary>
        protected DeviceMode DeviceMode { get; set; }


        /// <summary>
        /// Constructs OutputDevice <see cref="Device(DeviceType)"/>
        /// </summary>
        /// <param name="type">The Device type of the device</param>
        protected OutputDevice(DeviceType type):base(type){}

        /// <summary>
        /// Connect the device to a port
        /// </summary>
        /// <param name="port">Any output port on brick</param>
        public void Connect(OutputPort port)
        {
            if (Brick.IsConnected) throw new InvalidOperationException("Can not connect devices after connection to the brick is made!");
            if (IsConnected) throw new InvalidOperationException("Can not reconnect devices to other ports!");
            port.Set(this);
        }

        /// <summary>
        /// Internal method called from Brick upon connect through Port
        /// </summary>
        internal async Task InitializeDevice()
        {
            await Initialize();
        }

        /// <summary>
        /// Method is called from Brick connect to initialize the device, Eg. set correct mode and type and starting values.
        /// Override for custom implementation
        /// </summary>
        protected virtual async Task Initialize()
        {
            await Task.FromResult(true);
        }

        #region Firmware Methods

        /// <summary>
        /// This function enables specifying the output device type
        /// </summary>
        protected async Task SetType()
        {
            await OutputMethods.SetType(Socket, Layer, PortName, Type);
        }

        /// <summary>
        /// This function enables resetting the tacho count
        /// </summary>
        protected async Task Reset()
        {
            await OutputMethods.Reset(Socket, Layer, PortNames);
        }

        /// <summary>
        /// This function sends stop to device
        /// </summary>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        protected async Task Stop(Brake brake = Brake.Float)
        {
            await OutputMethods.Stop(Socket, Layer, PortNames, brake);
        }

        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        protected async Task SetPower(int power)
        {
            await OutputMethods.SetPower(Socket, Layer, PortNames, power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the device. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        protected async Task SetSpeed(int speed)
        {
            await OutputMethods.SetSpeed(Socket, Layer, PortNames, speed);
        }

        /// <summary>
        /// This function enables starting the device
        /// </summary>
        protected async Task Start()
        {
            await OutputMethods.Start(Socket, Layer, PortNames);
        }

        /// <summary>
        /// This function sets the polarity of the device.
        /// </summary>
        /// <param name="polarity">Polarity -1 : backward 0 : opposite direction 1 : forward</param>
        protected async Task SetPolarity(Polarity polarity)
        {
            await OutputMethods.SetPolarity(Socket, Layer, PortNames, polarity);
        }

        /// <summary>
        /// This function enables the program to test if a output device is busy.
        /// </summary>
        protected async Task<bool> IsBusy()
        {
            return await OutputMethods.IsBusy(Socket, Layer, PortNames);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in tacho counts. 
        /// RampUp specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampDown specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        protected async Task StepPower(int power, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepPower(Socket, Layer, PortNames, power, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in time. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant power period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        protected async Task TimePower(int power, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.TimePower(Socket, Layer, PortNames, power, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in tacho counts. 
        /// The system will automatically adjust the power level to the device to keep the specified output speed. 
        /// RampDown specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampUp specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        protected async Task StepSpeed(int speed, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepPower(Socket, Layer, PortNames, speed, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in time. 
        /// The system will automatically adjust the power level to the device to keep the specified output speed. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant speed period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        protected async Task TimeSpeed(int speed, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.TimeSpeed(Socket, Layer, PortNames, speed, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }
        
        /// <summary>
        /// This function enables the program to clear the tacho count used as sensor input.
        /// </summary>
        protected async Task ResetTachoCount()
        {
            await OutputMethods.ResetTachoCount(Socket, Layer, PortNames);
        }


        /// <summary>
        /// This function enables reading current speed 
        /// </summary>
        /// <returns>speed</returns>
        protected async Task<int> GetSpeed()
        {
            float value = await InputMethods.GetReadySIValue(Socket, PortNumber, 0, 2);
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// This function enables reading current Tacho count        
        /// </summary>
        /// <returns>tacho count</returns>
        protected async Task<int> GetTachoCount()
        {
            return await OutputMethods.GetTachoCount(Socket, Layer, PortName);
        }



        //TODO
        /*
        OutputMethods
        internal static async Task ProgramStop(Socket socket)
         * 
         *         ///// <summary>
        ///// Enables program execution to wait for output ready. (Wait for completion)
        ///// </summary>
        //protected async Task Ready()
        //{
        //    await OutputMethods.Ready(Socket, Layer, PortNames);
        //}
        */

        #endregion
    }
}
