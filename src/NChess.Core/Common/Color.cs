namespace NChess.Core.Common
{
    /// <summary>
    /// Represents the side (color) of a chess piece or player.
    /// Stored as <see cref="byte"/> for compact encoding in board data structures.
    /// </summary>
    public enum Color : byte
    {
        /// <summary>
        /// White side. Moves first in the game.
        /// </summary>
        White = 0,

        /// <summary>
        /// Black side. Moves second in the game.
        /// </summary>
        Black = 1
    }
}