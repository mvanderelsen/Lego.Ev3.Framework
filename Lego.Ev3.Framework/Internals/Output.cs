using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All Output ports on all layers/bricks
    /// </summary>
    internal class Output
    {
        /// <summary>
        /// List of all Output ports with port number index.
        /// </summary>
        public Dictionary<int, OutputPort> Ports { get; private set; }

        public Output()
        {
            //maximum of 16 ports 
            Ports = new Dictionary<int, OutputPort>(16);
        }

        public void AddPorts(IOPorts ports)
        {
            Ports.Add(ports.OutputPort.A.Number, ports.OutputPort.A);
            Ports.Add(ports.OutputPort.B.Number, ports.OutputPort.B);
            Ports.Add(ports.OutputPort.C.Number, ports.OutputPort.C);
            Ports.Add(ports.OutputPort.D.Number, ports.OutputPort.D);
        }

        #region Firmware Methods


        /// <summary>
        /// This function enables resetting the tacho count on all ports on all bricks
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Reset(OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.Reset(Brick.Socket, (ChainLayer)i, portFlag);
            }
        }

        /// <summary>
        /// This function sends stop to device on all ports on all bricks
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="brake">Specify break level, [0: Float, 1: Break]</param>
        public async Task Stop(OutputPortNames portFlag = OutputPortNames.All, Brake brake = Brake.Float)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.Stop(Brick.Socket, (ChainLayer)i, portFlag, brake);
            }
        }

        /// <summary>
        /// This function enables setting the output percentage power on all ports on all bricks
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="power">Specify output power [-100 – 100 %]</param>
        public async Task SetPower(int power, OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.SetPower(Brick.Socket, (ChainLayer)i, portFlag, power);
            }
        }

        /// <summary>
        /// This function enables setting the output percentage speed on the device on all ports on all bricks
        /// This automatically enables speed control, which means the system will automatically adjust the power to keep the specified speed.
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        /// <param name="speed">Specify output speed [-100 – 100 %]</param>
        public async Task SetSpeed(int speed, OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.SetSpeed(Brick.Socket, (ChainLayer)i, portFlag, speed);
            }
        }

        /// <summary>
        /// This function enables starting the device on all ports on all bricks
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task Start(OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.Start(Brick.Socket, (ChainLayer)i, portFlag);
            }
        }


        /// <summary>
        /// This function enables the program to test if a output device is busy on all ports on all bricks
        /// </summary>
        public async Task<bool> IsBusy(OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                bool result = await OutputMethods.IsBusy(Brick.Socket, (ChainLayer)i, portFlag);
                if (result) return true;
            }
            return false;
        }


        /// <summary>
        /// This function enables the program to clear the tacho count used as sensor input on all ports on all bricks
        /// </summary>
        /// <param name="portFlag">A flag indicating the ports to target. Eg PortNames.A|PortNames.C</param>
        public async Task ClearCount(OutputPortNames portFlag = OutputPortNames.All)
        {
            for (int i = 0; i < 4; i++)
            {
                await OutputMethods.ClearCount(Brick.Socket, (ChainLayer)i, portFlag);
            }
        }

        #endregion
    }
}
