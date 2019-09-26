using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using System;
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
        private  OutputPortNames PortNames { get; set; }


        public const int INITIAL_SPEED = Motor.INITIAL_SPEED;

        public int Speed { get; private set; }

        /// <summary>
        /// Motor polarity
        /// </summary>
        public Polarity Polarity { get; private set; }


        internal SynchronizedMotors(Motor motor1, Motor motor2)
        {
            if (motor1 == null || motor2 == null) throw new InvalidOperationException("Must combine two valid motors. Can not be null.");
            if (motor1.Equals(motor2)) throw new InvalidOperationException("Can not sync motor with it self");
            if (motor1.Type != motor2.Type) throw new InvalidOperationException("Must combine two motors with the same device type");
            if (motor1.Layer != motor2.Layer) throw new InvalidOperationException("Must combine two motors on the same layer");
            Layer = motor1.Layer;
            PortNames = motor1.PortNames | motor2.PortNames;
            Speed = INITIAL_SPEED;
            Polarity = motor1.Polarity;
            PortNumber = motor1.PortNumber;
        }

        #region Firmware Methods


        /// <summary>
        /// This function enables setting the output percentage power
        /// </summary>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public async Task SetPower(int power)
        {
            await OutputMethods.SetPower(Brick.Socket, Layer, PortNames, power);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the motors. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public async Task SetSpeed(int speed)
        {
            Speed = speed;
            await OutputMethods.SetPower(Brick.Socket, Layer, PortNames, speed);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio
        /// (depending on Polarity)
        /// for a specified duration specified in tacho counts at default speed.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="ratio">The turn ratio</param>
        public async Task Turn(int tachoCounts, TurnRatio ratio)
        {
            await Turn(tachoCounts, Speed, ratio);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio
        /// (depending on Polarity)
        /// for a specified duration specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        /// <param name="ratio">The turn ratio</param>
        public async Task Turn(int tachoCounts, int speed, TurnRatio ratio)
        {
            if (tachoCounts < 1) throw new ArgumentException("Tacho counts must greater than 1", "tachoCounts");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, -speed, (int)ratio, tachoCounts);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio : Right (Device A will run. B will run in opposite direction) 
        /// for a specified duration specified in tacho counts at default speed.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        public async Task TurnRight(int tachoCounts)
        {
            await TurnRight(tachoCounts, Speed);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio : Right (Device A will run. B will run in opposite direction) 
        /// for a specified duration specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task TurnRight(int tachoCounts, int speed)
        {
            if (tachoCounts < 1) throw new ArgumentException("Tacho counts must greater than 1", "tachoCounts");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            TurnRatio ratio = (Polarity == Polarity.Backward) ? TurnRatio.Left : TurnRatio.Right;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, -speed, (int)ratio, tachoCounts);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio : Left (Device B will run. A will run in opposite direction) 
        /// for a specified duration specified in tacho counts at default speed.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        public async Task TurnLeft(int tachoCounts)
        {
            await TurnLeft(tachoCounts, Speed);
        }

        /// <summary>
        /// This function enables to run both motors in turn ratio : Left (Device B will run. A will run in opposite direction) 
        /// for a specified duration specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task TurnLeft(int tachoCounts, int speed)
        {
            if (tachoCounts < 1) throw new ArgumentException("Tacho counts must greater than 1", "tachoCounts");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            TurnRatio ratio = (Polarity == Polarity.Backward) ? TurnRatio.Right : TurnRatio.Left;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, -speed, (int)ratio, tachoCounts);
        }

        /// <summary>
        /// This function enables to run both motors for a specified duration specified in tacho counts at default running speed
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        public async Task ReverseForTachoCounts(int tachoCounts)
        {
            await ReverseForTachoCounts(tachoCounts, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified duration specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task ReverseForTachoCounts(int tachoCounts, int speed)
        {
            if (tachoCounts < 1) throw new ArgumentException("Tacho counts must greater than 1", "tachoCounts");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, -speed, 0, tachoCounts);
        }

        /// <summary>
        /// This function enables to run both motors for a specified duration specified in tacho counts at default running speed
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        public async Task RunForTachoCounts(int tachoCounts)
        {
            await RunForTachoCounts(tachoCounts, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified duration specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho counts [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task RunForTachoCounts(int tachoCounts, int speed)
        {
            if (tachoCounts < 1) throw new ArgumentException("Tacho counts must greater than 1", "tachoCounts");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, speed, 0, tachoCounts);
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible, for example to achieve a model driving straight. 
        /// Duration is specified in tacho counts.
        /// </summary>
        /// <param name="tachoCounts">Tacho pulses, 0 = Infinite</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motor will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task StepSync(int tachoCounts, int turnRatio = 0, Brake brake = Brake.Float)
        {
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, Speed, turnRatio, tachoCounts, brake);
        }

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
            await OutputMethods.StepSync(Brick.Socket, Layer, PortNames, speed, turnRatio, tachoCounts, brake);
        }

        /// <summary>
        /// This function enables to run both motors for a specified time at default running speed
        /// </summary>
        /// <param name="time">Time in milliseconds, [1 - n]</param>
        public async Task RunForTime(int time)
        {
            await RunForTime(time, Speed);
        }

        /// <summary>
        /// This function enables to run both motors for a specified time.
        /// </summary>
        /// <param name="time">Time in milliseconds, [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task RunForTime(int time, int speed)
        {
            if (time < 1) throw new ArgumentException("Time must greater than 1", "time");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            await OutputMethods.TimeSync(Brick.Socket, Layer, PortNames, speed, 0, time);
        }

        /// <summary>
        /// This function enables to run both motors reversed for a specified time at default running speed
        /// </summary>
        /// <param name="time">Time in milliseconds, [1 - n]</param>
        public async Task ReverseForTime(int time)
        {
            await TimeSync(time, -Speed,0, Brake.Float);
        }

        /// <summary>
        /// This function enables to run both motors reversed for a specified time.
        /// </summary>
        /// <param name="time">Time in milliseconds, [1 - n]</param>
        /// <param name="speed">Specify output speed [1 – 100 %]</param>
        public async Task ReverseForTime(int time, int speed)
        {
            await TimeSync(time, -speed, 0, Brake.Float);
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
        public async Task TimeSync(int time, int turnRatio = 0, Brake brake = Brake.Float)
        {
            await TimeSync(time, Speed, turnRatio, brake);
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
            if (time < 1) throw new ArgumentOutOfRangeException("Time must be greater than 1", "time");
            if (speed < 1 || speed > 100) throw new ArgumentOutOfRangeException("Speed is out of range 1-100", "speed");
            Speed = speed;
            await OutputMethods.TimeSync(Brick.Socket, Layer, PortNames, speed, turnRatio, time, brake);
        }

        /// <summary>
        /// This function sends stop to motors
        /// </summary>
        public async Task Stop(Brake brake = Brake.Float)
        {
            await OutputMethods.Stop(Brick.Socket, Layer, PortNames, brake);
        }

        /// <summary>
        /// This function runs the motors in sync indefinitely at default running speed
        /// </summary>
        public async Task Run()
        {
            await OutputMethods.TimeSync(Brick.Socket, Layer, PortNames, Speed, 0, 0);
        }

        /// <summary>
        /// This function runs the motors reversed in sync indefinitely at default running speed
        /// </summary>
        public async Task Reverse()
        {
            await OutputMethods.TimeSync(Brick.Socket, Layer, PortNames, -Speed, 0, 0);
        }

        /// <summary>
        /// This function enables the program to test if the motors are busy.
        /// </summary>
        public async Task<bool> IsBusy()
        {
            return await OutputMethods.IsBusy(Brick.Socket, Layer, PortNames);
        }

        /// <summary>
        /// This function enables resetting the tacho count of the motors
        /// </summary>
        public async Task Reset()
        {
            await OutputMethods.Reset(Brick.Socket, Layer, PortNames);
        }

        /// <summary>
        /// This function enables reading current speed of Motor 1
        /// </summary>
        /// <returns>speed</returns>
        public async Task<int> GetSpeed()
        {
            float value = await InputMethods.GetReadySIValue(Brick.Socket, PortNumber, 0, 2);
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// This function enables reading current Tacho count of Motor 1   
        /// </summary>
        /// <returns>tacho count</returns>
        public async Task<int> GetTachoCount()
        {
            float value = await InputMethods.GetReadySIValue(Brick.Socket, PortNumber, 0, 0);
            return Convert.ToInt32(value);
        }

        ///// <summary>
        ///// Waits for motors to become ready eg. after run for time
        ///// If run is indefinite waits for Stop() to be called from eventhandler to complete.
        ///// Blocks the current thread
        ///// </summary>
        ///// <returns></returns>
        //public async Task RunComplete()
        //{
        //    CancellationToken token = Brick.Socket.CancellationToken;
        //    await Task.Factory.StartNew
        //        (
        //         async() =>
        //         {
        //             bool isBusy = true;
        //             while (isBusy)
        //             {
        //                 isBusy = await IsBusy();
        //                 if (!isBusy) break;
        //                 await Task.Delay(200, token);
        //             }
        //         }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current
        //        );
        //}


        #endregion
    }
}
