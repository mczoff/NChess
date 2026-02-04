namespace NChess.Core.Pieces
{
    /// <summary>
    /// Represents the type of a chess piece independent of its color.
    /// Values are stored as <see cref="byte"/> for compact board encoding.
    /// </summary>
    public enum PieceType : byte
    {
        /// <summary>
        /// Pawn piece.
        /// Moves forward one square (two from initial rank) and captures diagonally.
        /// Supports special moves: en passant and promotion.
        /// </summary>
        Pawn = 0,

        /// <summary>
        /// Knight piece.
        /// Moves in an L-shape (two squares in one direction and one perpendicular).
        /// Can jump over other pieces.
        /// </summary>
        Knight = 1,

        /// <summary>
        /// Bishop piece.
        /// Moves any number of squares diagonally.
        /// </summary>
        Bishop = 2,

        /// <summary>
        /// Rook piece.
        /// Moves any number of squares horizontally or vertically.
        /// Participates in castling with the king.
        /// </summary>
        Rook = 3,

        /// <summary>
        /// Queen piece.
        /// Moves any number of squares horizontally, vertically, or diagonally.
        /// </summary>
        Queen = 4,

        /// <summary>
        /// King piece.
        /// Moves one square in any direction.
        /// Participates in castling. Check and checkmate rules are centered on this piece.
        /// </summary>
        King = 5
    }
}