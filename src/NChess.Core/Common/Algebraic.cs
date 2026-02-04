using System;

namespace NChess.Core.Common
{
    public static class Algebraic
    {
        public static string FromSquare(Square square)
        {
            var file = (char)('a' + (int)square.File);
            var rank = (char)('1' + (int)square.Rank);
            return string.Concat(file, rank);
        }

        public static bool TryParseSquare(string text, out Square square)
        {
            square = default;

            if (string.IsNullOrEmpty(text) || text.Length != 2)
                return false;

            var f = text[0];
            var r = text[1];
            
            if (f >= 'A' && f <= 'H') f = (char)(f - 'A' + 'a');
            if (f < 'a' || f > 'h') return false;
            if (r < '1' || r > '8') return false;

            var file = (File)(f - 'a');
            var rank = (Rank)(r - '1');

            square = Square.From(file, rank);
            return true;
        }

        public static Square ParseSquare(string text)
        {
            if (!TryParseSquare(text, out var square))
                throw new FormatException($"Invalid square '{text}'. Expected like 'e4'.");

            return square;
        }
    }
}