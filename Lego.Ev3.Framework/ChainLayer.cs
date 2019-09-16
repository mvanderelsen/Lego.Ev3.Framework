
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Chain layer number [0-3]
    /// </summary>
    public enum ChainLayer
    {

        /// <summary>
        /// Layer one (Brick one (controlling brick) in daisy chain)
        /// </summary>
        One = 0x00,
        /// <summary>
        /// Layer two (Brick two in daisy chain)
        /// </summary>
        Two = 0x01,
        /// <summary>
        /// Layer three (Brick three in daisy chain)
        /// </summary>
        Three = 0x02,
        /// <summary>
        /// Layer four (Brick four in daisy chain)
        /// </summary>
        Four = 0x03
    }
}
