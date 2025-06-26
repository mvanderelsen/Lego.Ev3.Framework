using System;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{

    /// <summary>
    /// All methods that interact with brick output
    /// </summary>
    internal static class OutputMethods
    {
        /// <summary>
        /// This function enables specifying the output device type
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="port">Port number [0 - 3]</param>
        /// <param name="type">Output device type, (0x07: Large motor, Medium motor = 0x08)</param>
        /// <param name="requireReply">indicate if the brick should respond OK on method call</param>
        /// <remarks>
        /// Instruction opOutput_Set_Type (LAYER, NO, TYPE)
        /// Opcode 0xA1
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NO – Port number [0 - 3]
        /// (Data8) TYPE – Output device type, (0x07: Large motor, Medium motor = 0x08)
        /// Dispatch status Unchanged
        /// Description This function enables specifying the output device type
        /// </remarks>
        public static async Task SetType(ISocket socket, ChainLayer layer, OutputPortName port, DeviceType type, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_SET_TYPE);
                cb.SHORT((int)layer);
                cb.SHORT((int)port);
                cb.PAR8((byte)type);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables resetting the tacho count for the individual output ports
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Reset (LAYER, NOS)
        /// Opcode 0xA2
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// Dispatch status Unchanged
        /// Description This function enables resetting the tacho count for the individual output ports
        /// </remarks>
        public static async Task Reset(ISocket socket, ChainLayer layer, OutputPortFlag ports, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_RESET);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function sends stop to all individual output ports
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="brake">Specify break level [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Stop (LAYER, NOS, BRAKE)
        /// Opcode 0xA3
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) BRAKE – Specify break level [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables restting the tacho count for the individual output ports
        /// </remarks>
        public static async Task Stop(ISocket socket, ChainLayer layer, OutputPortFlag ports, Brake brake = Brake.Float, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_STOP);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables setting the output percentage power on the output ports
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Power (LAYER, NOS, POWER)
        /// Opcode 0xA4
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) POWER – Specify output speed [-100 – 100 %]
        /// Dispatch status Unchanged
        /// Description This function enables setting the output percentage power on the output ports
        /// </remarks>
        public static async Task SetPower(ISocket socket, ChainLayer layer, OutputPortFlag ports, int power, bool requireReply = true)
        {
            if (power < -100 || power > 100) throw new ArgumentException(nameof(power), "[-100,100]");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_POWER);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(power);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the output ports. 
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Speed (LAYER, NOS, SPEED)
        /// Opcode 0xA5
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) SPEED – Specify output speed [-100 – 100 %]
        /// Dispatch status Unchanged
        /// Description This function enables setting the output percentage speed on the output ports. This modes automatically enables speed control, which means the system will automa-tically adjust the power to keep the specified speed.
        /// </remarks>
        public static async Task SetSpeed(ISocket socket, ChainLayer layer, OutputPortFlag ports, int speed, bool requireReply = true)
        {
            if (speed < -100 || speed > 100) throw new ArgumentException(nameof(speed), "[-100 and 100]");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_SPEED);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(speed);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables starting the specified output port.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Start (LAYER, NOS)
        /// Opcode 0xA6
        /// Arguments (Data8) LAYER – Specify chain layer number, [0 - 3]
        /// (Data8) NOS – Output bit field, [0x00 – 0x0F]
        /// Dispatch status Unchanged
        /// Description This function enables starting the specified output port.
        /// </remarks>
        public static async Task Start(ISocket socket, ChainLayer layer, OutputPortFlag ports, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_START);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function sets the polarity of the specified output port(s).
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="polarity">Polarity -1 : backward 0 : opposite direction 1 : forward</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Polarity (LAYER, NOS, POL)
        /// Opcode 0xA7
        /// Arguments (Data8) LAYER – Specify chain layer number, [0 - 3]
        /// (Data8) NOS – Output bit field, [0x00 – 0x0F]
        /// (Data8) POL – Polarity [-1, 0, 1], see documentation below
        /// Dispatch status Unchanged
        /// Description This function enables starting the specified output port.
        /// </remarks>
        public static async Task SetPolarity(ISocket socket, ChainLayer layer, OutputPortFlag ports, Polarity polarity, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_POLARITY);
                cb.PAR8((int)layer);
                cb.PAR8((int)ports);
                cb.PAR8((byte)polarity);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        ///// <summary>
        ///// This function enables reading current motor speed and tacho count level.
        ///// </summary>
        ///// <param name="socket">socket for executing command to brick</param>
        ///// <param name="layer">Specify chain layer number [0 - 3]</param>
        ///// <param name="port">Port number [0 - 3]</param>
        ///// <remarks>
        ///// Instruction opOutput_Read (LAYER, NO, *SPEED, *TACHO)
        ///// Opcode 0xA8
        ///// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        ///// (Data8) NO – Port number [0 - 3]
        ///// (Data8) *SPEED – Output speed level detected, [-100 - 100]
        ///// (Data32) *TACHO – Current output tacho count
        ///// Dispatch status Unchanged
        ///// Description This function enables reading current motor speed and tacho count level.
        ///// </remarks>
        //public static async Task Read(ISocket socket, ChainLayer layer, OutputPortName port)
        //{
        //    Command cmd = null;
        //    using (CommandBuilder cb = new CommandBuilder(DIRECT_COMMAND_TYPE.DIRECT_COMMAND_NO_REPLY,0,2))
        //    {
        //        cb.OpCode(OpCode.opOutput_Read);
        //        cb.Argument((byte)layer);
        //        cb.Argument((byte)port);
        //        cb.LocalIndex(0);
        //        cb.LocalIndex32(1);
        //        cmd = cb.ToCommand();
        //    }
        //    await socket.Execute(cmd);

        //    CommandReply reply = cmd.Reply;
        //}

        /// <summary>
        /// This function enables the program to test if a output port is busy.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <remarks>
        /// Instruction opOutput_Test (LAYER, NOS, BUSY)
        /// Opcode 0xA9
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// Return (Data8) BUSY – Output busy flag, [0 = Ready, 1 = Busy]
        /// Dispatch status Unchanged
        /// Description This function enables the program to test if a output port is busy.
        /// </remarks>
        public static async Task<bool> IsBusy(ISocket socket, ChainLayer layer, OutputPortFlag ports)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opOUTPUT_TEST);
                cb.PAR8((int)layer);
                cb.PAR8((int)ports);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            return BitConverter.ToBoolean(response.PayLoad, 0);
        }

        ///// <summary>
        ///// Enables program execution to wait for output ready. (Wait for completion)
        ///// </summary>
        ///// <param name="socket">socket for executing command to brick</param>
        ///// <param name="layer">Specify chain layer number [0 - 3]</param>
        ///// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        ///// <remarks>
        ///// Instruction opOutput_Ready (LAYER, NOS)
        ///// Opcode 0xAA
        ///// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        ///// (Data8) NOS – Output bit field [0x00 – 0x0F]
        ///// Dispatch status Can changed to BUSYBREAK
        ///// Description Enables program execution to wait for output ready. (Wait for completion)
        ///// </remarks>
        //public static async Task Ready(ISocket socket, ChainLayer layer, OutputPortFlag ports)
        //{
        //    Command cmd = null;
        //    using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
        //    {
        //        cb.OpCode(OP.opOUTPUT_READY);
        //        cb.SHORT((int)layer);
        //        cb.SHORT((int)ports);
        //        cmd = cb.ToCommand();
        //    }
        //    await socket.Execute(cmd);
        //}

        /// <summary>
        /// This function enables specifying a full motor power cycle in tacho counts. 
        /// RampUp specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampDown specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Step_Power (LAYER, NOS, POWER, STEP1, STEP2, STEP3, BRAKE)
        /// Opcode 0xAC
        /// Arguments (Data8) LAYER – Specify chain layer number, [0 - 3]
        /// (Data8) NOS – Output bit field, [0x00 – 0x0F]
        /// (Data8) POWER – Power level, [-100 - 100]
        /// (Data32) STEP1 – Tacho pulses during ramp up
        /// (Data32) STEP2 – Tacho pulses during continues run
        /// (Data32) STEP3 – Tacho pulses during ramp down
        /// (Data8) BRAKE - Specify break level, [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables specifying a full motor power cycle in tacho counts. Step1 specifies the power ramp up periode in tacho count, Step2 specifies the constant power period in tacho counts, Step 3 specifies the power down period in tacho counts.
        /// </remarks>
        public static async Task StepPower(ISocket socket, ChainLayer layer, OutputPortFlag ports, int power, int tachoPulsesRampUp, int tachoPulsesContinuesRun, int tachoPulsesRampDown, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (power < -100 || power > 100) throw new ArgumentOutOfRangeException(nameof(power), "[-100,100]");
            if (tachoPulsesContinuesRun < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesContinuesRun), ">=0");
            if (tachoPulsesRampUp < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesRampUp), ">=0");
            if (tachoPulsesRampDown < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesRampDown), ">=0");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_STEP_POWER);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(power);
                cb.PAR32(tachoPulsesRampUp);
                cb.PAR32(tachoPulsesContinuesRun);
                cb.PAR32(tachoPulsesRampDown);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables specifying a full motor power cycle in time. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant power period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="power">Specify output power [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Time_Power (LAYER, NOS, POWER, STEP1, STEP2, STEP3, BRAKE)
        /// Opcode 0xAD
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) POWER – Power level, [-100 – 100]
        /// (Data32) STEP1 – Time in milliseconds for ramp up
        /// (Data32) STEP2 – Time in milliseconds for continues run
        /// (Data32) STEP3 – Time in milliseconds for ramp down
        /// (Data8) BRAKE - Specify break level [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables specifying a full motor power cycle in time. Step1 specifies the power ramp up periode in milliseconds, Step2 specifies the constant power period in milliseconds, Step 3 specifies the power down period in milliseconds.
        /// </remarks>
        public static async Task TimePower(ISocket socket, ChainLayer layer, OutputPortFlag ports, int power, int timeRampUp, int timeContinuesRun, int timeRampDown, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (power < -100 || power > 100) throw new ArgumentOutOfRangeException(nameof(power), "[-100,100]");
            if (timeContinuesRun < 0) throw new ArgumentOutOfRangeException(nameof(timeContinuesRun), ">=0");
            if (timeRampUp < 0) throw new ArgumentOutOfRangeException(nameof(timeRampUp), ">=0");
            if (timeRampDown < 0) throw new ArgumentOutOfRangeException(nameof(timeRampDown), ">=0");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_TIME_POWER);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(power);
                cb.PAR32(timeRampUp);
                cb.PAR32(timeContinuesRun);
                cb.PAR32(timeRampDown);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables specifying a full motor power cycle in tacho counts. 
        /// The system will automatically adjust the power level to the motor to keep the specified output speed. 
        /// RampDown specifies the power ramp up periode in tacho count, 
        /// ContinuesRun specifies the constant power period in tacho counts, 
        /// RampUp specifies the power down period in tacho counts.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="tachoPulsesRampUp">Tacho pulses during ramp up</param>
        /// <param name="tachoPulsesContinuesRun">Tacho pulses during continues run</param>
        /// <param name="tachoPulsesRampDown">Tacho pulses during ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Step_Speed (LAYER, NOS, SPEED, STEP1, STEP2, STEP3, BRAKE)
        /// Opcode 0xAE
        /// Arguments (Data8) LAYER – Specify chain layer number, [0 - 3]
        /// (Data8) NOS – Output bit field, [0x00 – 0x0F]
        /// (Data8) SPEED – Power level, [-100 - 100]
        /// (Data32) STEP1 – Tacho pulses during ramp up
        /// (Data32) STEP2 – Tacho pulses during continues run
        /// (Data32) STEP3 – Tacho pulses during ramp down
        /// (Data8) BRAKE - Specify break level, [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables specifying a full motor power cycle in tacho counts. The system will automatically adjust the power level to the motor to keep the specified output speed. Step1 specifies the power ramp up periode in tacho count, Step2 specifies the constant power period in tacho counts, Step 3 specifies the power down period in tacho counts.
        /// </remarks>
        public static async Task StepSpeed(ISocket socket, ChainLayer layer, OutputPortFlag ports, int speed, int tachoPulsesRampUp, int tachoPulsesContinuesRun, int tachoPulsesRampDown, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (speed < -100 || speed > 100) throw new ArgumentOutOfRangeException(nameof(speed), "[-100,100]");
            if (tachoPulsesContinuesRun < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesContinuesRun), ">=0");
            if (tachoPulsesRampUp < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesRampUp), ">=0");
            if (tachoPulsesRampDown < 0) throw new ArgumentOutOfRangeException(nameof(tachoPulsesRampDown), ">=0");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_STEP_SPEED);
                cb.PAR8((int)layer);
                cb.PAR8((int)ports);
                cb.PAR8(speed);
                cb.PAR32((uint)tachoPulsesRampUp);
                cb.PAR32((uint)tachoPulsesContinuesRun);
                cb.PAR32((uint)tachoPulsesRampDown);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables specifying a full power cycle in time. 
        /// The system will automatically adjust the power level to keep the specified output speed. 
        /// RampUp specifies the power ramp up periode in milliseconds, 
        /// ContinuesRun specifies the constant speed period in milliseconds, 
        /// RampDown specifies the power down period in milliseconds.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="speed">Specify output speed [-100 – 100]</param>
        /// <param name="timeRampUp">Time in milliseconds for ramp up</param>
        /// <param name="timeContinuesRun">Time in milliseconds for continues run</param>
        /// <param name="timeRampDown">Time in milliseconds for ramp down</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Time_Speed (LAYER, NOS, SPEED, STEP1, STEP2, STEP3, BRAKE)
        /// Opcode 0xAF
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) SPEED – Power level, [-100 – 100]
        /// (Data32) STEP1 – Time in milliseconds for ramp up
        /// (Data32) STEP2 – Time in milliseconds for continues run
        /// (Data32) STEP3 – Time in milliseconds for ramp down
        /// (Data8) BRAKE - Specify break level [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables specifying a full motor power cycle in time. The system will automatically adjust the power level to the motor to keep the specified output speed. Step1 specifies the power ramp up periode in milliseconds, Step2 specifies the constant power period in milliseconds, Step 3 specifies the power down period in milliseconds.
        /// </remarks>
        public static async Task TimeSpeed(ISocket socket, ChainLayer layer, OutputPortFlag ports, int speed, int timeRampUp, int timeContinuesRun, int timeRampDown, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (speed < -100 || speed > 100) throw new ArgumentOutOfRangeException(nameof(speed), "[-100,100]");
            if (timeContinuesRun < 0) throw new ArgumentOutOfRangeException(nameof(timeContinuesRun), ">=0");
            if (timeRampUp < 0) throw new ArgumentOutOfRangeException(nameof(timeRampUp), ">=0");
            if (timeRampDown < 0) throw new ArgumentOutOfRangeException(nameof(timeRampDown), ">=0");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_TIME_SPEED);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(speed);
                cb.PAR32(timeRampUp);
                cb.PAR32(timeContinuesRun);
                cb.PAR32(timeRampDown);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible, for example to achieve a model driving straight. 
        /// Duration is specified in tacho counts.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motor will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="tachoCounts">Tacho pulses, 0 = Infinite</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Step_Sync (LAYER, NOS, SPEED, TURN, STEP, BRAKE)
        /// Opcode 0xB0
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) SPEED – Power level, [-100 – 100]
        /// (Data16) TURN – Turn ratio, [-200 - 200], see documentation below
        /// (Data32) STEP – Tacho pulses, 0 = Infinite
        /// (Data8) BRAKE - Specify break level [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables synchonizing two motors. Synchonization should be used when motors should run as synchrone as possible, for example to archieve a model driving straight. Duration is specified in tacho counts.
        /// </remarks>
        public static async Task StepSync(ISocket socket, ChainLayer layer, OutputPortFlag ports, int speed, int turnRatio, int tachoCounts, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (tachoCounts < 0) throw new ArgumentOutOfRangeException(nameof(tachoCounts), ">=0");
            if (speed < -100 || speed > 100) throw new ArgumentOutOfRangeException(nameof(speed), "[-100,100]");
            if (turnRatio < -200 || turnRatio > 200) throw new ArgumentOutOfRangeException(nameof(turnRatio), "[-200,200]");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_STEP_SYNC);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(speed);
                cb.PAR16(turnRatio);
                cb.PAR32(tachoCounts);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables synchonizing two motors. 
        /// Synchonization should be used when motors should run as synchrone as possible,
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="speed">Speed level, [-100 – 100]</param>
        /// <param name="turnRatio">Turn ratio, [-200 - 200]
        /// 0 : Motor will run with same power
        /// 100 : One motor will run with specified power while the other will be close to zero
        /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
        /// </param>
        /// <param name="time">Time in milliseconds, 0 = Infinite</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Time_Sync (LAYER, NOS, SPEED, TURN, STEP, BRAKE)
        /// Opcode 0xB1
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data8) SPEED – Power level, [-100 – 100]
        /// (Data16) TURN – Turn ratio, [-200 - 200], see documentation below
        /// (Data32) TIME – Time in milliseconds, 0 = Infinite
        /// (Data8) BRAKE - Specify break level [0: Float, 1: Break]
        /// Dispatch status Unchanged
        /// Description This function enables synchonizing two motors. Synchonization should be used when motors should run as synchrone as possible,
        /// </remarks>
        public static async Task TimeSync(ISocket socket, ChainLayer layer, OutputPortFlag ports, int speed, int turnRatio, int time, Brake brake = Brake.Float, bool requireReply = true)
        {
            if (time < 0) throw new ArgumentOutOfRangeException(nameof(time), ">=0");
            if (speed < -100 || speed > 100) throw new ArgumentOutOfRangeException(nameof(speed), "[-100,100]");
            if (turnRatio < -200 || turnRatio > 200) throw new ArgumentOutOfRangeException(nameof(turnRatio), "[-200,200]");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_TIME_SYNC);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cb.PAR8(speed);
                cb.PAR16(turnRatio);
                cb.PAR32(time);
                cb.PAR8((byte)brake);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables the program to clear the tacho count used as sensor input.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="ports">Output bit field [0x00 – 0x0F]</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Clr_Count (LAYER, NOS)
        /// Opcode 0xB2
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// Dispatch status Unchanged
        /// Description This function enables the program to clear the tacho count used as sensor input.
        /// </remarks>
        public static async Task ResetTachoCount(ISocket socket, ChainLayer layer, OutputPortFlag ports, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_CLR_COUNT);
                cb.SHORT((int)layer);
                cb.SHORT((int)ports);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }


        /// <summary>
        /// This function enables the program to read the tacho count as sensor input.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <param name="port">Output bit field [0x00 – 0x0F]</param>
        /// <remarks>
        /// Instruction opOutput_Get_Count (LAYER, NOS, *TACHO)
        /// Opcode 0xB3
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NOS – Output bit field [0x00 – 0x0F]
        /// (Data32) *TACHO – Tacho count as sensor value
        /// Dispatch status Unchanged
        /// Description This function enables the program to read the tacho count as sensor input.
        /// </remarks>
        public static async Task<int> GetTachoCount(ISocket socket, ChainLayer layer, OutputPortName port)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                cb.OpCode(OP.opOUTPUT_GET_COUNT);
                cb.PAR8((byte)layer);
                cb.PAR8((byte)port);
                cb.VARIABLE_PAR32(0, PARAMETER_VARIABLE_SCOPE.GLOBAL);
                cmd = cb.ToCommand();
            }

            Response response = await socket.Execute(cmd);

            return BitConverter.ToInt32(response.PayLoad, 0);
        }



        /// <summary>
        /// This function should be called a program end. It enables breaking the motor for a short period and right after floating the motors. The function relates to the layer on which it is executed.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="requireReply"></param>
        /// <remarks>
        /// Instruction opOutput_Prg_Stop
        /// Opcode 0xB4
        /// Arguments
        /// Dispatch status Unchanged
        /// Description This function should be called a program end. It enables breaking the motor for a short period and right after floating the motors. The function relates to the layer on which it is executed.
        /// </remarks>
        public static async Task ProgramStop(ISocket socket, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opOUTPUT_PRG_STOP);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }


    }
}
