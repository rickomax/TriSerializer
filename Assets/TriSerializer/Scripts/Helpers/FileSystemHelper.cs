using System;
using System.IO;
using System.Security.Cryptography;

namespace TriSerializer
{
    /// <summary>
    /// Provides utility methods for file system operations and data processing.
    /// </summary>
    public static class FileSystemHelper
    {
        /// <summary>
        /// A read-only array of characters that are invalid in file paths, initialized with the system's invalid path characters.
        /// </summary>
        private static readonly char[] InvalidChars = Path.GetInvalidPathChars();

        /// <summary>
        /// Determines whether the specified file path is valid by checking for invalid characters.
        /// </summary>
        /// <param name="path">The file path to validate.</param>
        /// <returns>
        /// <c>true</c> if the path does not contain any invalid characters; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPathValid(string path)
        {
            if (path.IndexOfAny(InvalidChars) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Computes the SHA-256 hash of the provided stream's content.
        /// </summary>
        /// <param name="stream">The input stream to hash.</param>
        /// <returns>
        /// A lowercase hexadecimal string representing the SHA-256 hash of the stream's content, without hyphens.
        /// </returns>
        /// <remarks>
        /// The stream's position is reset to 0 before computing the hash. The method uses a SHA256 instance
        /// created via <see cref="SHA256.Create"/> and disposes of it after use.
        /// </remarks>
        public static string CalculateSHA256(Stream stream)
        {
            using (var sha256 = SHA256.Create())
            {
                stream.Position = 0;
                var hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}