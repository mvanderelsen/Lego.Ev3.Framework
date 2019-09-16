
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Turn ratio, [-200 - 200]
    /// 0 : Motor will run with same power
    /// 100 : One motor will run with specified power while the other will be close to zero
    /// 200: One motor will run with specified power forward while the other will run in the opposite direction at the same power level.
    /// </summary>
    public enum TurnRatio
    {
        /// <summary>
        /// Device A and B will run with same power
        /// </summary>
        Foward = 0,
        /// <summary>
        /// Device A will run. B will not run.
        /// </summary>
        SoftRight = 100,
        /// <summary>
        /// Device A will run. B will run in opposite direction.
        /// </summary>
        Right = 200,
        /// <summary>
        /// Device B will run. A will not run.
        /// </summary>
        SoftLeft = -100,
        /// <summary>
        /// Device B will run. A will run in opposite direction.
        /// </summary>
        Left = -200
    }
}
