namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Directory Information
    /// </summary>
    public struct DirectoryInfo
    {
        /// <summary>
        /// Amount of items in the directory
        /// </summary>
        public int Items { get; }

        /// <summary>
        /// Total size of the directory in KB
        /// </summary>
        public int Size { get; set; }

        internal DirectoryInfo(int items, int size) 
        {
            Items = items;
            Size = size;
        }
    }
}
