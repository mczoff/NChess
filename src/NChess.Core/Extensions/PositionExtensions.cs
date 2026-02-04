using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Extensions
{
    public static class PositionExtensions
    {
        public static bool HasPiece(this Position position, Square square, Color color, PieceType pieceType)
        {
            var piece = position.GetPiece(square);
            return piece.HasValue && piece.Value.Color == color && piece.Value.Type == pieceType;
        }

        public static bool HasKnight(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.Knight);

        public static bool HasKing(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.King);

        public static bool HasPawn(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.Pawn);

        public static bool HasBishop(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.Bishop);

        public static bool HasQueen(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.Queen);

        public static bool HasRook(this Position position, Square square, Color color)
            => position.HasPiece(square, color, PieceType.Rook);

        public static bool HasSlider(this Position position, Square square, Color color, PieceType pieceType1,
            PieceType pieceType2)
            => position.HasPiece(square, color, pieceType1) || position.HasPiece(square, color, pieceType2);
    }
}