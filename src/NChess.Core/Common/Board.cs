using System;
using System.Collections.Generic;
using NChess.Core.Pieces;

namespace NChess.Core.Common
{
    public sealed class Board
    {
        private readonly Piece?[] _squares = new Piece?[64];

        public Piece? this[Square square]
        {
            get => _squares[square.Index];
            set => _squares[square.Index] = value;
        }

        public void Clear(Square square)
        {
            _squares[square.Index] = null;
        }

        public void Clear()
        {
            Array.Clear(_squares, 0, _squares.Length);
        }

        public bool IsEmpty(Square square)
        {
            return !_squares[square.Index].HasValue;
        }

        public IEnumerable<(Square Square, Piece Piece)> Pieces()
        {
            for (var i = 0; i < 64; i++)
            {
                var piece = _squares[i];
                if (piece.HasValue)
                    yield return (new Square(i), piece.Value);
            }
        }

        public IEnumerable<(Square Square, Piece Piece)> Pieces(Color color)
        {
            for (var i = 0; i < 64; i++)
            {
                var piece = _squares[i];
                if (piece.HasValue && piece.Value.Color == color)
                    yield return (new Square(i), piece.Value);
            }
        }

        public Square FindKing(Color color)
        {
            for (var i = 0; i < 64; i++)
            {
                var piece = _squares[i];
                if (piece.HasValue &&
                    piece.Value.Color == color &&
                    piece.Value.Type == PieceType.King)
                {
                    return new Square(i);
                }
            }

            throw new InvalidOperationException($"King of color {color} not found on board.");
        }
    }
}