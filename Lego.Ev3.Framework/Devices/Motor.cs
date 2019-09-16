using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Devices
{
    /// <summary>
    /// Base class for medium and large motors
    /// For all methods that apply to both. Call methods only after connection to the brick is made.
    /// </summary>
    public abstract class Motor : OutputDevice
    {
        public const int INITIAL_SPEED = 80;

        /// <summary>
        /// Motor polarity
        /// </summary>
        public Polarity Polarity { get; internal set; }

        /// <summary>
        /// Constructs Motor <see cref="Device(DeviceType)"/>
        /// Mode is automatically set through method calls
        /// </summary>
        /// <param name="type">The Device type of the device</param>
        /// <param name="polarity">The initial polarity</param>
        protected Motor(DeviceType type, Polarity polarity) : base(type) 
        {
            Polarity = polarity;
        }

        /// <summary>
        /// Override to set the port to the correct motor type
        /// Polarity and initial speed control on init
        /// </summary>
        protected sealed override async Task Initialize()
        {
            await base.SetType();
            await base.SetPolarity(Polarity);
            await base.SetSpeed(INITIAL_SPEED);
        }

        #region Firmware Methods

        /// <summary>
        /// This function enables resetting the tacho count
        /// </summary>
        public new async Task Reset()
        {
            await base.Reset();
        }

        /// <summary>
        /// This function sends stop to motor
        /// </summary>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public new async Task Stop(Brake brake = Brake.Float)
        {
            await base.Stop(brake);
        }

        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public new async Task SetPower(int power)
        {
            await base.SetPower(power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the motor. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public new async Task SetSpeed(int speed)
        {
            await base.SetSpeed(speed);
        }

        /// <summary>
        /// This function enables starting the motor
        /// </summary>
        public new async Task Start()
        {
            await base.Start();
        }

        /// <summary>
        /// This function sets the polarity of the motor.
        /// </summary>
        /// <param name="polarity">Polarity -1 : Motor will run backward 0 : Motor will run opposite direction 1 : Motor will run forward</param>
        public new async Task SetPolarity(Polarity polarity)
        {
            Polarity = polarity;
            await base.SetPolarity(polarity);
        }

        /// <summary>
        /// This function enables the program to test if a motor is busy.
        /// </summary>
        public new async Task<bool> IsBusy()
        {
            return await base.IsBusy();
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
        public new async Task StepPower(int power, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await base.StepPower(power, tachoPulsesContinuesRun, tachoPulsesRampUp, tachoPulsesRampDown, brake);
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
        public new  async Task TimePower(int power, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await base.TimePower(power, timeContinuesRun, timeRampUp, timeRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in tacho counts. 
        /// The system will automatically adjust the power level to the motor to keep the specified output speed. 
        /// RampDown specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampUp specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public new async Task StepSpeed(int speed, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await base.StepSpeed(speed, tachoPulsesContinuesRun, tachoPulsesRampUp, tachoPulsesRampDown, brake);
        }

        /// <summary>
        /// This function enables specifying a full motor power cycle in time. 
        /// The system will automatically adjust the power level to the motor to keep the specified output speed. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant speed period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public new async Task TimeSpeed(int speed, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await base.TimeSpeed(speed, timeContinuesRun, timeRampUp, timeRampDown, brake);
        }


        /// <summary>
        /// This function enables reading current speed 
        /// </summary>
        /// <returns>speed</returns>
        public new async Task<int> GetSpeed()
        {
            return await base.GetSpeed();
        }

        /// <summary>
        /// This function enables reading current Tacho count        
        /// </summary>
        /// <returns>tacho count</returns>
        public new async Task<int> GetTachoCount()
        {
            return await base.GetTachoCount();
        }




        //TODO
        /*
        OutputMethods
        internal static async Task Ready(Socket socket, ChainLayer layer, OutputPortNames ports)
        internal static async Task ProgramStop(Socket socket)
         * 
         *         /// <summary>
        /// Enables program execution to wait for output ready. (Wait for completion)
        /// </summary>
        public new async Task Ready()
        {
            await base.Ready();
        }
        */

        #endregion
    }
}
