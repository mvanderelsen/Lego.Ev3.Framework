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
    public class Response
    {
        /// <summary>
        /// Id
        /// </summary>
        public ushort Id { get; private set; }

        /// <summary>
        /// Type
        /// </summary>
        public ResponseType Type { get; private set; }

        /// <summary>
        /// Status
        /// </summary>
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// PayLoad
        /// </summary>
        public byte[] PayLoad { get; private set; }


        /// <summary>
        /// Error Response
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Response Error(ushort id)
        {
            return new Response
            {
                Id = id,
                Type = ResponseType.ERROR,
                Status = ResponseStatus.UNKNOWN_ERROR
            };
        }

        /// <summary>
        /// Ok Response
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Response Ok(ushort id)
        {
            return new Response
            {
                Id = id,
                Type = ResponseType.OK,
                Status = ResponseStatus.SUCCESS
            };
        }

        /// <summary>
        /// Gets a Response from payload
        /// </summary>
        /// <param name="payLoad"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Response FromPayLoad(byte[] payLoad)
        {
            ushort id = GetId(payLoad);

            switch (payLoad[2]) 
            {
                case 0x02: //DIRECT_REPLY
                    {
                        Response response = Ok(id);
                        int payLoadLength = payLoad.Length - 3;
                        if (payLoadLength > 0)
                        {
                            response.PayLoad = new byte[payLoadLength];
                            Array.Copy(payLoad, 3, response.PayLoad, 0, payLoadLength);
                        }
                        return response;
                    }
                case 0x05: //SYSTEM_REPLY_ERROR treat as OK response we need to check the status
                case 0x03: //SYSTEM_REPLY
                    {
                        Response response = Ok(id);

                        //skip OpCode in response (SYSTEM_OP)payLoad[3];

                        response.Status = (ResponseStatus)payLoad[4];

                        int payLoadLength = payLoad.Length - 5;
                        if (payLoadLength > 0)
                        {
                            response.PayLoad = new byte[payLoadLength];
                            Array.Copy(payLoad, 5, response.PayLoad, 0, payLoadLength);
                        }
                        return response;
                    }
                case 0x04: //DIRECT_REPLY_ERROR
                    {
                        return Error(id);
                    }
                default: throw new ArgumentOutOfRangeException(nameof(payLoad));
            }
        }

        /// <summary>
        /// Gets the Response id from the payload
        /// </summary>
        /// <param name="payLoad"></param>
        /// <returns></returns>
        public static ushort GetId(byte[] payLoad)
        {
            return (ushort)(payLoad[0] | (payLoad[1] << 8));
        }
    }
}
