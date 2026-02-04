namespace NChess.Core.Common
{
    /// <summary>
    /// Represents a board file (column) on a chess board.
    /// Indexed from <c>0</c> (file A) to <c>7</c> (file H).
    /// </summary>
    /// <remarks>
    /// The numeric value is zero-based and suitable for array-based board indexing.
    /// </remarks>
    public enum File : byte
    {
        /// <summary>File A.</summary>
        A = 0,

        /// <summary>File B.</summary>
        B = 1,

        /// <summary>File C.</summary>
        C = 2,

        /// <summary>File D.</summary>
        D = 3,

        /// <summary>File E.</summary>
        E = 4,

        /// <summary>File F.</summary>
        F = 5,

        /// <summary>File G.</summary>
        G = 6,

        /// <summary>File H.</summary>
        H = 7
    }
}