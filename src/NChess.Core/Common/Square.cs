using System;
using System.Runtime.CompilerServices;

namespace NChess.Core.Common
{
    public readonly struct Square : IEquatable<Square>, IComparable<Square>
    {
        public int Index { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Square(int index)
        {
            if ((uint)index > 63) throw new ArgumentOutOfRangeException(nameof(index), "Square index must be in range 0..63.");
            Index = index;
        }

        public File File => (File)(Index & 7);
        public Rank Rank => (Rank)(Index >> 3);

        public bool IsLight
            => (((int)File + (int)Rank) & 1) == 1;

        public bool IsDark
            => !IsLight;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Square From(File file, Rank rank)
            => new Square(((int)rank << 3) | (int)file);

        public static bool TryFrom(int index, out Square square)
        {
            if ((uint)index <= 63)
            {
                square = new Square(index);
                return true;
            }

            square = default;
            return false;
        }

        public Square Offset(int delta)
            => new Square(Index + delta);

        public bool TryOffset(int delta, out Square square)
        {
            var next = Index + delta;
            return TryFrom(next, out square);
        }

        //public override string ToString()
        //    => Algebraic.FromSquare(this);

        #region Equality / Compare

        public bool Equals(Square other) => Index == other.Index;

        public override bool Equals(object? obj)
            => obj is Square other && Equals(other);

        public override int GetHashCode() => Index;

        public int CompareTo(Square other) => Index.CompareTo(other.Index);

        public static bool operator ==(Square left, Square right) => left.Equals(right);
        public static bool operator !=(Square left, Square right) => !left.Equals(right);

        #endregion
        
        public static readonly Square A1 = new Square(0);
        public static readonly Square H1 = new Square(7);
        public static readonly Square A8 = new Square(56);
        public static readonly Square H8 = new Square(63);
    }
}