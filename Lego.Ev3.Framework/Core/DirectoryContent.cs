namespace Lego.Ev3.Framework.Core
{
    public class DirectoryContent
    {
        public Directory Root { get; internal set; }

        public Directory Parent { get; internal set; }

        public Directory[] Directories { get; internal set; }

        public File[] Files { get; internal set; }

        public int ItemCount { get { return Files.Length + Directories.Length; } }

    }
}
