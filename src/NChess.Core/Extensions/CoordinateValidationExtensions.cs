using NChess.Core.Common;

namespace NChess.Core.Extensions
{
    /// <summary>
    /// Provides validation helpers for board coordinates.
    /// </summary>
    public static class CoordinateValidationExtensions
    {
        /// <summary>
        /// Determines whether the specified integer value is a valid zero-based file index.
        /// </summary>
        /// <param name="file">The file index to validate.</param>
        /// <returns>
        /// <c>true</c> if the value is in range [0, 7]; otherwise <c>false</c>.
        /// </returns>
        public static bool IsValidFile(this int file)
            => (uint)file < BoardConstants.Size;

        /// <summary>
        /// Determines whether the specified integer value is a valid zero-based rank index.
        /// </summary>
        /// <param name="rank">The rank index to validate.</param>
        /// <returns>
        /// <c>true</c> if the value is in range [0, 7]; otherwise <c>false</c>.
        /// </returns>
        public static bool IsValidRank(this int rank)
            => (uint)rank < BoardConstants.Size;

        /// <summary>
        /// Determines whether the specified file value is defined.
        /// </summary>
        public static bool IsDefined(this File file)
            => (byte)file < BoardConstants.Size;

        /// <summary>
        /// Determines whether the specified rank value is defined.
        /// </summary>
        public static bool IsDefined(this Rank rank)
            => (byte)rank < BoardConstants.Size;
    }
}