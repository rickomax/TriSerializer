using System.IO;

namespace TriSerializer
{
    public static class FileSystemHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidPathChars();

        public static bool IsPathValid(string path)
        {
            if (path.IndexOfAny(InvalidChars) >= 0)
            {
                return false;
            }
            return true;
        }
    }
}