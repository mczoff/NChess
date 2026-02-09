using System.Linq;
using System.Text;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Moves;
using NChess.Core.Pieces;

namespace NChess.Core.Notation
{
    public static class San
    {
        public static bool TryParse(Position position, IChessEngine engine, string san, out Move move)
        {
            move = default;
            if (string.IsNullOrWhiteSpace(san))
                return false;

            var normalized = Normalize(san);

            foreach (var legalMove in engine.GenerateLegalMoves(position))
            {
                var s = Format(position, engine, legalMove);

                if (Normalize(s) != normalized)
                    continue;

                move = legalMove;
                return true;
            }

            return false;
        }

        private static string Normalize(string s)
        {
            return s.TrimEnd('+', '#', '!', '?');
        }

        public static string Format(Position position, IChessEngine engine, Move move)
        {
            if (move.IsCastling)
                return move.To.File > move.From.File ? "O-O" : "O-O-O";

            var moved = position.GetPiece(move.From)!.Value;

            var sb = new StringBuilder(12);

            if (moved.Type != PieceType.Pawn)
            {
                sb.Append(PieceLetter(moved.Type));

                var dis = GetDisambiguation(position, engine, move, moved.Type, moved.Color);
                if (dis != null)
                    sb.Append(dis);
            }
            else
            {
                if (move.IsCapture)
                    sb.Append((char)('a' + (int)move.From.File));
            }

            if (move.IsCapture)
                sb.Append('x');

            sb.Append(Algebraic.FromSquare(move.To));

            if (move.IsPromotion)
            {
                sb.Append('=');
                sb.Append(PieceLetter(move.Promotion!.Value));
            }

            return sb.ToString();
        }

        private static bool IsCheck(Position position, IChessEngine engine)
            => engine.Attacks.IsKingInCheck(position, position.SideToMove);

        private static string GetDisambiguation(
            Position position,
            IChessEngine engine,
            Move move,
            PieceType type,
            Color color)
        {
            var candidates = engine.GenerateLegalMoves(position)
                .Where(m =>
                {
                    if (m.To != move.To) return false;
                    if (m.From == move.From) return false;
                    var p = position.GetPiece(m.From);
                    return p.HasValue && p.Value.Type == type && p.Value.Color == color;
                })
                .ToList();

            if (candidates.Count == 0)
                return null;
            
            var needFile = candidates.Any(m => m.From.File != move.From.File);
            var needRank = candidates.Any(m => m.From.Rank != move.From.Rank);
            
            if (needFile)
            {
                var fileChar = (char)('a' + (int)move.From.File);
                
                var sameFileExists = candidates.Any(m => m.From.File == move.From.File);
                if (!sameFileExists)
                    return fileChar.ToString();
                
                if (needRank)
                {
                    var rankChar = (char)('1' + (int)move.From.Rank);
                    return fileChar.ToString() + rankChar;
                }

                return fileChar.ToString();
            }
            
            if (needRank)
            {
                var rankChar = (char)('1' + (int)move.From.Rank);
                return rankChar.ToString();
            }

            return null;
        }
        
        private static char PieceLetter(PieceType t)
        {
            return t switch
            {
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Rook => 'R',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => '?'
            };
        }
    }
}