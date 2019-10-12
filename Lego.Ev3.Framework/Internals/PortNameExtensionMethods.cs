namespace Lego.Ev3.Framework.Internals
{
    internal static class PortNameExtensionMethods
    {
        public static int AbsolutePortNumber(this InputPortName port, ChainLayer layer)
        {
            return ((int)layer * 4) + (int)port;
        }

        public static int AbsolutePortNumber(this OutputPortName port, ChainLayer layer)
        {
            return (((int)layer * 4) + 16) + (int)port;
        }

        public static OutputPortFlag ToFlag(this OutputPortName port) 
        {
            return (OutputPortFlag)(1 << (int)port); //convert to bitfield e.g. 0010 equals port 2
        }
    }
}
