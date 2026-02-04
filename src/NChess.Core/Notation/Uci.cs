using System;
using NChess.Core.Common;
using NChess.Core.Moves;
using NChess.Core.Pieces;

namespace NChess.Core.Notation
{
    public static class Uci
    {
        public static bool TryParse(string text, out Move move)
        {
            move = default;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();
            if (text.Length != 4 && text.Length != 5)
                return false;

            if (!Algebraic.TryParseSquare(text.Substring(0, 2), out var from))
                return false;

            if (!Algebraic.TryParseSquare(text.Substring(2, 2), out var to))
                return false;

            if (text.Length == 4)
            {
                move = new Move(from, to);
                return true;
            }

            var promoChar = char.ToLowerInvariant(text[4]);
            PieceType promo;

            switch (promoChar)
            {
                case 'q': promo = PieceType.Queen; break;
                case 'r': promo = PieceType.Rook; break;
                case 'b': promo = PieceType.Bishop; break;
                case 'n': promo = PieceType.Knight; break;
                default: return false;
            }

            move = new Move(from, to, MoveFlags.Promotion, promo);
            return true;
        }

        public static Move Parse(string text)
            => TryParse(text, out var m)
                ? m
                : throw new FormatException($"Invalid UCI move '{text}'. Expected like 'e2e4' or 'e7e8q'.");

        public static string Format(Move move)
        {
            Span<char> buf = stackalloc char[5];

            buf[0] = (char)('a' + (int)move.From.File);
            buf[1] = (char)('1' + (int)move.From.Rank);
            buf[2] = (char)('a' + (int)move.To.File);
            buf[3] = (char)('1' + (int)move.To.Rank);

            if (move.IsPromotion)
            {
                buf[4] = PromotionChar(move.Promotion!.Value);
                return new string(buf);
            }

            return new string(buf.Slice(0, 4));
        }

        private static char PromotionChar(PieceType type)
        {
            return type switch
            {
                PieceType.Queen => 'q',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}
