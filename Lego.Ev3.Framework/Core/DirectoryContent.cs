namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// Directory Content
    /// </summary>
    public class DirectoryContent
    {
        /// <summary>
        /// Root directory
        /// </summary>
        public Directory Root { get; internal set; }

        /// <summary>
        /// Parent directory
        /// </summary>
        public Directory Parent { get; internal set; }

        /// <summary>
        /// Directories
        /// </summary>
        public Directory[] Directories { get; internal set; }

        /// <summary>
        /// Files
        /// </summary>
        public File[] Files { get; internal set; }

        /// <summary>
        /// Total item count of directories and files
        /// </summary>
        public int ItemCount { get { return Files.Length + Directories.Length; } }

    }
}
