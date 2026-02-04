using System;
using NChess.Core.Common;

namespace NChess.Core.Pieces
{
    /// <summary>
    /// Immutable value type that represents a chess piece as a combination of
    /// <see cref="PieceType"/> and <see cref="Common.Color"/>.
    /// </summary>
    /// <remarks>
    /// Designed as a lightweight value object suitable for board representations and move generation.
    /// </remarks>
    public readonly struct Piece : IEquatable<Piece>
    {
        /// <summary>
        /// Gets the piece type (pawn, knight, bishop, rook, queen, king).
        /// </summary>
        public PieceType Type { get; }
        
        /// <summary>
        /// Gets the piece color (e.g. white or black).
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Initializes a new <see cref="Piece"/> with the specified <paramref name="type"/> and <paramref name="color"/>.
        /// </summary>
        /// <param name="type">The piece type.</param>
        /// <param name="color">The piece color.</param>
        public Piece(PieceType type, Color color)
        {
            Type = type;
            Color = color;
        }

        /// <summary>
        /// Returns a human-readable representation, e.g. "White Queen".
        /// </summary>
        public override string ToString()
            => $"{Color} / {Type}";

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="Piece"/>.
        /// </summary>
        public readonly bool Equals(Piece other)
            => Type == other.Type && Color == other.Color;

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Piece other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine((byte)Type, (byte)Color);

        /// <summary>
        /// Compares two pieces for equality.
        /// </summary>
        public static bool operator ==(Piece left, Piece right)
            => left.Equals(right);

        /// <summary>
        /// Compares two pieces for inequality.
        /// </summary>
        public static bool operator !=(Piece left, Piece right)
            => !left.Equals(right);
    }
}