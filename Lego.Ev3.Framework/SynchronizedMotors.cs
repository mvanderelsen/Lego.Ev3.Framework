using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Two paired motors in sync. 
    /// Enables synchonizing two motors. 
    /// Synchonization should be used when motors should run as synchrone as possible, for example to achieve a model driving straight. 
    /// </summary>
    public sealed class SynchronizedMotors
    {
        private int PortNumber { get; set; }
        private ChainLayer Layer { get; set; }
        private OutputPortFlag PortFlag { get; set; }

        private OutputPortName PortName { get; set; }


        /// <summary>
        /// Initial speed at start up
        /// </summary>
        public const int INITIAL_SPEED = Motor.INITIAL_SPEED;

        /// <summary>
        /// Gets the current speed
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Motor polarity
        /// </summary>
        public Polarity Polarity { get; private set; }


        internal SynchronizedMotors(Motor motor1, Motor motor2)
        {
            if (motor1 == null || motor2 == null) throw new InvalidOperationException("Must combine two valid motors. Can not be null.");
            if (motor1.Equals(motor2)) throw new InvalidOperationException("Can not sync motor with it self");
            if (motor1.Type != motor2.Type) throw new InvalidOperationException("Must combine two motors of the same device type");
            if (motor1.Layer != motor2.Layer) throw new InvalidOperationException("Must combine two motors on the same layer");
            Layer = motor1.Layer;
            PortFlag = motor1.PortFlag | motor2.PortFlag;
            Speed = INITIAL_SPEED;
            Polarity = motor1.Polarity;
            PortNumber = motor1.PortNumber;
            PortName = motor1.PortName;
        }

        #region firmware methods
        //copy from Outputdevice
        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public async Task SetPower(int power)
        {
            await OutputMethods.SetPower(Brick.Socket, Layer, PortFlag, power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the motors. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public async Task SetSpeed(int speed)
        {
            Speed = speed;
            await OutputMethods.SetPower(Brick.Socket, Layer, PortFlag, speed);
        }

        /// <summary>
        /// This function sends stop to motors
        /// </summary>
        public async Task Stop(Brake brake = Brake.Float)
        {
            await OutputMethods.Stop(Brick.Socket, Layer, PortFlag, brake);
        }

        /// <summary>
        /// This function enables the program to test if the motors are busy.
        /// </summary>
        public async Task<bool> IsBusy()
        {
            return await OutputMethods.IsBusy(Brick.Socket, Layer, PortFlag);
        }

        /// <summary>
        /// This function enables resetting the tacho count of the motors
        /// </summary>
        public async Task Reset()
        {
            await OutputMethods.Reset(Brick.Socket, Layer, PortFlag);
        }

        public async Task ResetTachoCount() 
        {
            await OutputMethods.ResetTachoCount(Brick.Socket, Layer, PortFlag);
        }

        /// <summary>
        /// This function enables reading current speed of Motor 1
        /// </summary>
        /// <returns>speed</returns>
        public async Task<int> GetSpeed()
        {
            float value = await InputMethods.GetReadySIValue(Brick.Socket, PortNumber, 0, 2);
            //return (int) Math.Floor(value);
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// This function enables reading current Tacho count of Motor 1   
        /// </summary>
        /// <returns>tacho count</returns>
        public async Task<int> GetTachoCount()
        {
            return await OutputMethods.GetTachoCount(Brick.Socket, Layer, PortName);
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
        public async Task StepPower(int power, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepPower(Brick.Socket, Layer, PortFlag, power, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
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
        public async Task TimePower(int power, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.TimePower(Brick.Socket, Layer, PortFlag, power, timeRampUp, timeContinuesRun, timeRampDown, brake);
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
        public async Task StepSpeed(int speed, int tachoPulsesContinuesRun, int tachoPulsesRampUp = 0, int tachoPulsesRampDown = 0, Brake brake = Brake.Float)
        {
            Speed = speed;
            await OutputMethods.StepPower(Brick.Socket, Layer, PortFlag, speed, tachoPulsesRampUp, tachoPulsesContinuesRun, tachoPulsesRampDown, brake);
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
        public async Task TimeSpeed(int speed, int timeContinuesRun, int timeRampUp = 0, int timeRampDown = 0, Brake brake = Brake.Float)
        {
            Speed = speed;
            await OutputMethods.TimeSpeed(Brick.Socket, Layer, PortFlag, speed, timeRampUp, timeContinuesRun, timeRampDown, brake);
        }
        #endregion

        #region firmware methods synchronized motors only
        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible, for example to achieve a model driving straight. 
        /// Duration is specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho pulses, 0 = Infinite</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motor will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task StepSync(int tachoCounts, int speed, int turnRatio = 0, Brake brake = Brake.Float)
        {
            Speed = speed;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortFlag, speed, turnRatio, tachoCounts, brake);
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible, for example to achieve a model driving straight. 
        /// Duration is specified in tacho counts.
        /// Method awaits for tacho counts and method to complete.
        /// </summary>
        /// <param name="tachoCounts">Tacho pulses, 0 = Infinite</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motor will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="cancellationToken"></param>
        public async Task StepSyncComplete(int tachoCounts, int speed, int turnRatio = 0, Brake brake = Brake.Float, CancellationToken cancellationToken = default)
        {
            int initialTachoCount = await GetTachoCount();

            await OutputMethods.StepSync(Brick.Socket, Layer, PortFlag, speed, turnRatio, tachoCounts, brake);

            if (tachoCounts > 0 && await IsBusy()) // can not wait for indefinite to complete
            {

                int tachoCount = 0;

                DateTime start = DateTime.Now;

                while (tachoCount < tachoCounts)
                {
                    int currentTachoCount = await GetTachoCount();

                    tachoCount = Math.Abs(currentTachoCount - initialTachoCount);

                    int todoTachoCount = tachoCounts - tachoCount;

                    if (todoTachoCount > 0)
                    {
                        double elapsedTime = (DateTime.Now - start).TotalMilliseconds;
                        double tachoCountPerMillisecond = tachoCount / elapsedTime;
                        int delay = (int)Math.Ceiling(todoTachoCount * tachoCountPerMillisecond);
                        if (delay > 0)
                        {
                            try
                            {
                                await Task.Delay(delay, cancellationToken);
                            }
                            catch (TaskCanceledException) { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible,
        /// </summary>
        /// <param name="time">Time in milliseconds, 0 = Infinite</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motors will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task TimeSync(int time, int speed, int turnRatio = 0, Brake brake = Brake.Float)
        {
            Speed = speed;
            await OutputMethods.TimeSync(Brick.Socket, Layer, PortFlag, speed, turnRatio, time, brake);
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible,
        /// Method awaits for time to elapse and method to complete.
        /// </summary>
        /// <param name="time">Time in milliseconds, 0 = Infinite</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motors will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="cancellationToken"></param>
        public async Task TimeSyncComplete(int time, int speed, int turnRatio = 0, Brake brake = Brake.Float, CancellationToken cancellationToken = default)
        {
            await TimeSync(time, speed, turnRatio, brake);
            if (time > 0) // can not wait for indefinite to complete
            {
                try
                {
                    await Task.Delay(time, cancellationToken);

                }
                catch (TaskCanceledException) { }
            }
        }
        #endregion

        #region overloads

        /// <summary>
        /// This function runs the motors in sync indefinitely at current running speed
        /// </summary>
        public async Task Run()
        {
            await TimeSync(0, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified time at current running speed
        /// </summary>
        public async Task Run(TimeSpan time)
        {
            await TimeSync((int)time.TotalMilliseconds, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified tacho count at current running speed
        /// </summary>
        public async Task Run(int tachoCount)
        {
            await StepSync(tachoCount, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified time at current running speed and waits for operation to complete
        /// </summary>
        public async Task RunComplete(TimeSpan time, CancellationToken cancellationToken = default)
        {
            await TimeSyncComplete((int)time.TotalMilliseconds, Speed, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// This function enables to run both motors for a specified tacho count at current running speed and waits for operation to complete
        /// </summary>
        public async Task RunComplete(int tachoCount, CancellationToken cancellationToken = default)
        {
            await StepSyncComplete(tachoCount, Speed, cancellationToken: cancellationToken);
        }


        /// <summary>
        /// This function runs the motors reversed in sync indefinitely at current running speed
        /// </summary>
        public async Task Reverse()
        {
            await TimeSync(0, -Speed);
        }

        /// <summary>
        /// This function runs the motors reversed in sync indefinitely at current running speed
        /// </summary>
        public async Task Reverse(TimeSpan time)
        {
            await TimeSync((int)time.TotalMilliseconds, -Speed);
        }

        /// <summary>
        /// This function enables to run both motors reversed for a specified tacho count at current running speed
        /// </summary>
        public async Task Reverse(int tachoCount)
        {
            await StepSync(tachoCount, -Speed);
        }

        /// <summary>
        /// This function runs the motors reversed in sync for specified time at current running speed and waits for operation to complete
        /// </summary>
        public async Task ReverseComplete(TimeSpan time, CancellationToken cancellationToken = default)
        {
            await TimeSyncComplete((int)time.TotalMilliseconds, -Speed, cancellationToken: cancellationToken);
        }


        /// <summary>
        /// This function runs the motors reversed for a specified tacho count at current running speed and waits for operation to complete
        /// </summary>
        public async Task ReverseComplete(int tachoCount, CancellationToken cancellationToken = default)
        {
            await StepSyncComplete(tachoCount, -Speed, cancellationToken: cancellationToken);
        }


        /// <summary>
        /// This function enables to run both motors in turn ratio
        /// (depending on Polarity)
        /// for a specified duration specified in tacho counts at current speed.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="ratio">The turn ratio</param>
        public async Task Turn(int tachoCounts, TurnRatio ratio)
        {
            await StepSync(tachoCounts, Speed, (int)ratio);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio
        /// (depending on Polarity)
        /// for a specified duration specified in tacho counts at current speed and waits for operation to complete
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="ratio">The turn ratio</param>
        /// <param name="cancellationToken"></param>
        public async Task TurnComplete(int tachoCounts, TurnRatio ratio, CancellationToken cancellationToken = default)
        {
            await StepSyncComplete(tachoCounts, Speed, (int)ratio, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
