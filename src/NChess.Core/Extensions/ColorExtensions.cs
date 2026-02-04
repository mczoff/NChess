using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Extensions
{
    /// <summary>
    /// Provides helper and formatting extension methods for <see cref="Color"/>.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts the color to its Forsyth–Edwards Notation (FEN) side-to-move character.
        /// </summary>
        /// <param name="color">The color value.</param>
        /// <returns>
        /// <c>'w'</c> for <see cref="Color.White"/> or <c>'b'</c> for <see cref="Color.Black"/>.
        /// </returns>
        public static char ToFenChar(this Color color)
            => color == Color.White ? 'w' : 'b';

        /// <summary>
        /// Converts the color to a short one-letter uppercase string.
        /// </summary>
        /// <param name="color">The color value.</param>
        /// <returns>
        /// <c>"W"</c> for <see cref="Color.White"/> or <c>"B"</c> for <see cref="Color.Black"/>.
        /// </returns>
        public static string ToShortString(this Color color)
            => color == Color.White ? "W" : "B";

        /// <summary>
        /// Gets the opposite color.
        /// </summary>
        /// <param name="color">The source color.</param>
        /// <returns>The opposite side color.</returns>
        public static Color Opposite(this Color color)
            => color == Color.White ? Color.Black : Color.White;
    }
}