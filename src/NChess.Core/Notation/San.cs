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
            {
                return move.To.File > move.From.File ? "O-O" : "O-O-O";
            }

            var sb = new StringBuilder(8);

            var piece = position.GetPiece(move.From)!.Value;

            if (piece.Type != PieceType.Pawn)
                sb.Append(PieceLetter(piece.Type));

            if (piece.Type == PieceType.Pawn && move.IsCapture)
                sb.Append((char)('a' + (int)move.From.File));

            if (move.IsCapture)
                sb.Append('x');

            sb.Append(Algebraic.FromSquare(move.To));

            if (move.IsPromotion)
            {
                sb.Append('=');
                sb.Append(PieceLetter(move.Promotion!.Value));
            }

            var engineResult = engine.MakeMove(position, move);

            if (engineResult.IsOk)
            {
                var undo = engineResult.Value;
                var inCheck = !engine.GenerateLegalMoves(position).GetEnumerator().MoveNext()
                    ? "#"
                    : engine.GenerateLegalMoves(position) != null && IsCheck(position, engine)
                        ? "+"
                        : "";

                engine.UndoMove(position, undo);
                sb.Append(inCheck);
            }

            return sb.ToString();
        }

        private static bool IsCheck(Position position, IChessEngine engine)
            => engine.Attacks.IsKingInCheck(position, position.SideToMove);

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