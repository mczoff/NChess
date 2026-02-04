using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Moves
{
    public sealed class MoveUndo
    {
        public Square From { get; }
        public Square To { get; }

        public Piece MovedPiece { get; }
        public Piece? CapturedPiece { get; }

        public Color PrevSideToMove { get; }
        public CastlingRights PrevCastling { get; }
        public Square? PrevEnPassant { get; }
        public int PrevHalfmoveClock { get; }
        public int PrevFullmoveNumber { get; }

        public MoveUndo(
            Square from,
            Square to,
            Piece movedPiece,
            Piece? capturedPiece,
            Color prevSideToMove,
            CastlingRights prevCastling,
            Square? prevEnPassant,
            int prevHalfmoveClock,
            int prevFullmoveNumber)
        {
            From = from;
            To = to;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;

            PrevSideToMove = prevSideToMove;
            PrevCastling = prevCastling;
            PrevEnPassant = prevEnPassant;
            PrevHalfmoveClock = prevHalfmoveClock;
            PrevFullmoveNumber = prevFullmoveNumber;
        }
    }
}