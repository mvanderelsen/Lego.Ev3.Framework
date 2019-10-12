using Lego.Ev3.Framework.Firmware;
using System;

namespace Lego.Ev3.Framework
{
    public class FirmwareException : Exception
    {
        internal FirmwareException(Response response) : base("Firmware api method returned an invalid response") 
        {
            Data["Status"] = response.Status;
            Data["PayLoad"] = response.PayLoad;
        }
    }
}
