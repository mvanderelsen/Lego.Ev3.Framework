using System;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// #define DIRECT_REPLY 0x02 // Direct command reply OK
    /// #define DIRECT_REPLY_ERROR 0x04 // Direct command reply ERROR
    /// Byte 0 – 1: Reply size, Little Endian. Reply size not including these 2 bytes
    /// Byte 2 – 3: Message counter, Little Endian. Equals the Direct Command
    /// Byte 4: Reply type.
    /// Byte 5 - n: Resonse buffer. I.e. the content of the by the Command reserved global variables. I.e. if the command reserved 64 bytes, these bytes will be placed in the reply packet as the bytes 5 to 68.
    /// </remarks>
    /// <remarks>
    /// #define SYSTEM_REPLY 0x03 // System command reply OK
    /// #define SYSTEM_REPLY_ERROR 0x05 // System command reply ERROR
    /// Byte 0 – 1: Reply size, Little Endian. Reply size not including these 2 bytes
    /// Byte 2 – 3: Message counter, Little Endian. Equals the Direct Command
    /// Byte 4: Reply type. See defines above
    /// Byte 5: System Command which this is reply to.
    /// Byte 6: System Reply Status – Error, info or success. See the definitions below:
    /// Byte 7 – n: Further
    /// </remarks>
    internal class Response
    {
        public ushort Id { get; private set; }

        public ResponseType Type { get; private set; }

        public SYSTEM_COMMAND_STATUS Status { get; private set; }

        public byte[] PayLoad { get; private set; }


        public static Response Error(CommandHandle handle)
        {
            return new Response
            {
                Id = handle.Id,
                Type = ResponseType.ERROR,
                Status = SYSTEM_COMMAND_STATUS.UNKNOWN_ERROR
            };
        }

        public static Response Ok(CommandHandle handle)
        {
            return new Response
            {
                Id = handle.Id,
                Type = ResponseType.OK,
                Status = SYSTEM_COMMAND_STATUS.SUCCESS
            };
        }

        public static Response FromPayLoad(CommandHandle handle, byte[] payLoad)
        {
            Response response = Error(handle);

            switch (handle.Type)
            {
                case CommandType.DIRECT_COMMAND_REPLY:
                    {
                        if (((DIRECT_COMMAND_REPLY_TYPE)(int)payLoad[2]) == DIRECT_COMMAND_REPLY_TYPE.DIRECT_REPLY)
                        {
                            response.Type = ResponseType.OK;
                            response.Status = SYSTEM_COMMAND_STATUS.SUCCESS;
                        }
                        int payLoadLength = payLoad.Length - 3;
                        if (payLoadLength > 0)
                        {
                            response.PayLoad = new byte[payLoadLength];
                            Array.Copy(payLoad, 3, response.PayLoad, 0, payLoadLength);
                        }
                        return response;
                    }
                case CommandType.SYSTEM_COMMAND_REPLY:
                    {
                        if (((SYSTEM_COMMAND_REPLY_TYPE)(int)payLoad[2]) == SYSTEM_COMMAND_REPLY_TYPE.SYSTEM_REPLY)
                        {
                            response.Type = ResponseType.OK;
                            response.Status = SYSTEM_COMMAND_STATUS.SUCCESS;
                        }

                        //skip OpCode in response
                        //response.OpCode = (SYSTEM_OP)payLoad[3];

                        response.Status = (SYSTEM_COMMAND_STATUS)payLoad[4];

                        int payLoadLength = payLoad.Length - 5;
                        if (payLoadLength > 0)
                        {
                            response.PayLoad = new byte[payLoadLength];
                            Array.Copy(payLoad, 5, response.PayLoad, 0, payLoadLength);
                        }
                        return response;
                    }
                default: throw new InvalidOperationException(nameof(handle.Type));
            }
        }

        public static ushort GetId(byte[] payLoad)
        {
            return (ushort)(payLoad[0] | (payLoad[1] << 8));
        }
    }
}
