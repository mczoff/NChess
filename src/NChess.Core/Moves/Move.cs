using System;
using System.Runtime.CompilerServices;
using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Moves
{
    public readonly struct Move
    {
        public Square From { get; }
        public Square To { get; }
        
        public PieceType? Promotion { get; }
        
        public MoveFlags Flags { get; }

        public MoveKind Kind
        {
            get
            {
                if ((Flags & MoveFlags.Castling) != 0) return MoveKind.Castling;
                if ((Flags & MoveFlags.EnPassant) != 0) return MoveKind.EnPassant;
                if ((Flags & MoveFlags.Promotion) != 0) return MoveKind.Promotion;
                if ((Flags & MoveFlags.Capture) != 0) return MoveKind.Capture;
                return MoveKind.Quiet;
            }
        }
        
        public bool IsCapture => (Flags & MoveFlags.Capture) != 0;
        public bool IsPromotion => (Flags & MoveFlags.Promotion) != 0;
        public bool IsEnPassant => (Flags & MoveFlags.EnPassant) != 0;
        public bool IsCastling => (Flags & MoveFlags.Castling) != 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move(Square from, Square to, MoveFlags flags = MoveFlags.None, PieceType? promotion = null)
        {
            if (promotion.HasValue && (flags & MoveFlags.Promotion) == 0)
                throw new ArgumentException("Promotion piece specified but Promotion flag is not set.", nameof(promotion));
            
            if ((flags & MoveFlags.Promotion) != 0)
            {
                if (!promotion.HasValue)
                    throw new ArgumentException("Promotion flag is set but promotion piece is not specified.", nameof(promotion));

                if (!IsValidPromotion(promotion.Value))
                    throw new ArgumentOutOfRangeException(nameof(promotion), "Promotion must be Queen, Rook, Bishop, or Knight.");
            }

            From = from;
            To = to;
            Flags = flags;
            Promotion = promotion;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidPromotion(PieceType type)
            => type == PieceType.Queen
               || type == PieceType.Rook
               || type == PieceType.Bishop
               || type == PieceType.Knight;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char PromotionToChar(PieceType type)
        {
            if (IsValidPromotion(type))
                throw new ArgumentException("Promotion piece specified but Promotion flag is not set.", nameof(type));

            switch (type)
            {
                case PieceType.Queen:  return 'q';
                case PieceType.Rook:   return 'r';
                case PieceType.Bishop: return 'b';
                case PieceType.Knight: return 'n';
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Invalid promotion piece.");
            }
        }
        
        public bool Equals(Move other)
            => From == other.From
               && To == other.To
               && Promotion == other.Promotion
               && Flags == other.Flags;

        public override bool Equals(object obj)
            => obj is Move other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + From.GetHashCode();
                hash = (hash * 31) + To.GetHashCode();
                hash = (hash * 31) + (Promotion.HasValue ? (int)Promotion.Value : -1);
                hash = (hash * 31) + (int)Flags;
                return hash;
            }
        }

        public static bool operator ==(Move left, Move right) => left.Equals(right);
        public static bool operator !=(Move left, Move right) => !left.Equals(right);
    }
}