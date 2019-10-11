using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Lego.Ev3.Framework.Firmware
{
    internal static class BrickLog
    {
        public static void Log(this Exception e, OP opCode) 
        {
            Brick.Logger.LogError(e, $"opcode: {opCode}");
        }

        public static void Log(this Response response, OP opCode) 
        {
            switch (response.Type) 
            {
                //case ResponseType.OK: 
                //    {
                //        Trace(opCode, response);
                //        break;
                //    }
                case ResponseType.ERROR: 
                    {
                        Error(opCode, response);
                        break;
                    }
            }
        }

        private static void Error(OP opCode, Response response) 
        {
            Brick.Logger.LogError(GetMessage(opCode, response));
        }

        private static void Trace(OP opCode, Response response)
        {
            Brick.Logger.LogTrace(GetMessage(opCode, response));
        }

        private static string GetMessage(OP opCode, Response response) 
        {
            StringBuilder sb = new StringBuilder();
            if (response.PayLoad != null)
            {
                sb.Append("payload: ");
                foreach (byte b in response.PayLoad)
                {
                    sb.Append($"[{b}]");
                }
            }
            return $"op: {opCode} response: {response.Type} status: {response.Status} {sb.ToString()}".Trim();
        }
    }
}
