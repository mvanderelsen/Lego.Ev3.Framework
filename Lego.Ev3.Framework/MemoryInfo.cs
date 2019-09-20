namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Memory information
    /// </summary>
    public struct MemoryInfo
    {
        /// <summary>
        /// Total memory size in KB
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Free memory size in KB
        /// </summary>
        public int Free { get; }

        internal MemoryInfo(int[] values)
        {
            Total = values[0];
            Free = values[1];
        }

        /// <summary>
        /// returns formatted Total - free string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Total: {File.FileSize(Total * 1024)} Free: {File.FileSize(Free * 1024)}";
        }
    }
}
