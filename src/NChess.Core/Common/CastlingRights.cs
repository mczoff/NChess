using System;

namespace NChess.Core.Common
{
    public readonly struct CastlingRights : IEquatable<CastlingRights>
    {
        private readonly CastlingRightsMask _castlingRightsMask;

        private CastlingRights(CastlingRightsMask castlingRightsMask)
        {
            _castlingRightsMask = castlingRightsMask;
        }

        public static CastlingRights None => new CastlingRights(CastlingRightsMask.None);

        public static CastlingRights All =>
            new CastlingRights(
                CastlingRightsMask.WhiteKingSide |
                CastlingRightsMask.WhiteQueenSide |
                CastlingRightsMask.BlackKingSide |
                CastlingRightsMask.BlackQueenSide
            );

        public bool WhiteKingSide => (_castlingRightsMask & CastlingRightsMask.WhiteKingSide) != 0;
        public bool WhiteQueenSide => (_castlingRightsMask & CastlingRightsMask.WhiteQueenSide) != 0;
        public bool BlackKingSide => (_castlingRightsMask & CastlingRightsMask.BlackKingSide) != 0;
        public bool BlackQueenSide => (_castlingRightsMask & CastlingRightsMask.BlackQueenSide) != 0;

        public CastlingRights WithWhiteKingSide(bool enabled)
            => Set(CastlingRightsMask.WhiteKingSide, enabled);

        public CastlingRights WithWhiteQueenSide(bool enabled)
            => Set(CastlingRightsMask.WhiteQueenSide, enabled);

        public CastlingRights WithBlackKingSide(bool enabled)
            => Set(CastlingRightsMask.BlackKingSide, enabled);

        public CastlingRights WithBlackQueenSide(bool enabled)
            => Set(CastlingRightsMask.BlackQueenSide, enabled);

        private CastlingRights Set(CastlingRightsMask flag, bool enabled)
        {
            var next = enabled
                ? (_castlingRightsMask | flag)
                : (_castlingRightsMask & ~flag);

            return new CastlingRights(next);
        }

        public override string ToString()
        {
            Span<char> buf = stackalloc char[4];
            var i = 0;

            if (WhiteKingSide) buf[i++] = 'K';
            if (WhiteQueenSide) buf[i++] = 'Q';
            if (BlackKingSide) buf[i++] = 'k';
            if (BlackQueenSide) buf[i++] = 'q';

            return i == 0 ? "-" : new string(buf.Slice(0, i).ToArray());
        }

        public bool Equals(CastlingRights other) => _castlingRightsMask == other._castlingRightsMask;

        public override bool Equals(object? obj)
            => obj is CastlingRights other && Equals(other);

        public override int GetHashCode() => (int)_castlingRightsMask;

        public static bool operator ==(CastlingRights left, CastlingRights right)
            => left.Equals(right);

        public static bool operator !=(CastlingRights left, CastlingRights right)
            => !left.Equals(right);
    }
}