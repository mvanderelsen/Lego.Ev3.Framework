namespace Lego.Ev3.Framework.Internals
{
    internal static class PortNameExtensionMethods
    {
        public static int AbsolutePortNumber(this InputPortName port, ChainLayer layer)
        {
            return ((int)layer * 8) + (int)port;
        }

        public static int AbsolutePortNumber(this OutputPortName port, ChainLayer layer)
        {
            return (((int)layer + 1) * 16) + (int)port;
        }
    }
}
