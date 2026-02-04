namespace NChess.Core.Common
{
    /// <summary>
    /// Represents a board rank (row) on a chess board.
    /// Indexed from <c>0</c> (rank 1) to <c>7</c> (rank 8).
    /// </summary>
    /// <remarks>
    /// The numeric value is zero-based and suitable for array-based board indexing.
    /// Rank numbering follows standard chess convention where rank 1 is White's home rank.
    /// </remarks>
    public enum Rank : byte
    {
        /// <summary>Rank 1.</summary>
        One = 0,

        /// <summary>Rank 2.</summary>
        Two = 1,

        /// <summary>Rank 3.</summary>
        Three = 2,

        /// <summary>Rank 4.</summary>
        Four = 3,

        /// <summary>Rank 5.</summary>
        Five = 4,

        /// <summary>Rank 6.</summary>
        Six = 5,

        /// <summary>Rank 7.</summary>
        Seven = 6,

        /// <summary>Rank 8.</summary>
        Eight = 7
    }
}