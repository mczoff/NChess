using System;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Extensions;
using NChess.Core.Pieces;

namespace NChess.Core.Engine.Classic
{
    internal sealed class ClassicAttackDetector : IAttackDetector
    {
        public bool IsKingInCheck(Position position, Color kingColor)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            var kingSq = position.FindKingSquare(kingColor);
            var attacker = kingColor == Color.White ? Color.Black : Color.White;

            return IsSquareAttacked(position, kingSq, attacker);
        }

        public bool IsSquareAttacked(Position position, Square square, Color byColor)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            return AttackedByKnight(position, square, byColor)
                   || AttackedByPawn(position, square, byColor)
                   || AttackedByKing(position, square, byColor)
                   || AttackedBySliders(position, square, byColor);
        }

        private static bool AttackedByKnight(Position p, Square target, Color byColor)
        {
            return (target.TryOffset(+1, +2, out var s1) && p.HasKnight(s1, byColor)) ||
                   (target.TryOffset(+2, +1, out var s2) && p.HasKnight(s2, byColor)) ||
                   (target.TryOffset(+2, -1, out var s3) && p.HasKnight(s3, byColor)) ||
                   (target.TryOffset(+1, -2, out var s4) && p.HasKnight(s4, byColor)) ||
                   (target.TryOffset(-1, -2, out var s5) && p.HasKnight(s5, byColor)) ||
                   (target.TryOffset(-2, -1, out var s6) && p.HasKnight(s6, byColor)) ||
                   (target.TryOffset(-2, +1, out var s7) && p.HasKnight(s7, byColor)) ||
                   (target.TryOffset(-1, +2, out var s8) && p.HasKnight(s8, byColor));
        }

        private static bool AttackedByKing(Position p, Square target, Color byColor)
        {
            return (target.TryOffset(+1, +1, out var s1) && p.HasKing(s1, byColor)) ||
                   (target.TryOffset(+1, 0, out var s2) && p.HasKing(s2, byColor)) ||
                   (target.TryOffset(+1, -1, out var s3) && p.HasKing(s3, byColor)) ||
                   (target.TryOffset(0, +1, out var s4) && p.HasKing(s4, byColor)) ||
                   (target.TryOffset(0, -1, out var s5) && p.HasKing(s5, byColor)) ||
                   (target.TryOffset(-1, +1, out var s6) && p.HasKing(s6, byColor)) ||
                   (target.TryOffset(-1, 0, out var s7) && p.HasKing(s7, byColor)) ||
                   (target.TryOffset(-1, -1, out var s8) && p.HasKing(s8, byColor));
        }

        private static bool AttackedByPawn(Position p, Square target, Color byColor)
        {
            // If 'target' is attacked by a pawn of 'byColor', that pawn must be one rank behind target
            // (relative to the pawn's moving direction).
            var dr = byColor == Color.White ? -1 : +1;

            return (target.TryOffset(-1, dr, out var s1) && p.HasPawn(s1, byColor)) ||
                   (target.TryOffset(+1, dr, out var s2) && p.HasPawn(s2, byColor));
        }

        private static bool AttackedBySliders(Position p, Square target, Color byColor)
        {
            // Diagonals: bishop/queen
            if (RayAttacked(p, target, byColor, +1, +1, PieceType.Bishop, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, +1, -1, PieceType.Bishop, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, -1, +1, PieceType.Bishop, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, -1, -1, PieceType.Bishop, PieceType.Queen)) return true;

            // Orthogonals: rook/queen
            if (RayAttacked(p, target, byColor, +1, 0, PieceType.Rook, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, -1, 0, PieceType.Rook, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, 0, +1, PieceType.Rook, PieceType.Queen)) return true;
            if (RayAttacked(p, target, byColor, 0, -1, PieceType.Rook, PieceType.Queen)) return true;

            return false;
        }

        private static bool RayAttacked(Position p, Square start, Color byColor, int df, int dr, PieceType t1, PieceType t2)
        {
            var sq = start;

            while (sq.TryOffset(df, dr, out var next))
            {
                sq = next;

                var piece = p.GetPiece(sq);
                if (!piece.HasValue)
                    continue;

                return piece.Value.Color == byColor && (piece.Value.Type == t1 || piece.Value.Type == t2);
            }

            return false;
        }
    }
}
