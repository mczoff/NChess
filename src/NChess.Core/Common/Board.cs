using System;
using System.Collections.Generic;
using System.Diagnostics;
using NChess.Core.Pieces;

namespace NChess.Core.Common
{
    [DebuggerDisplay("{DebuggerView,nq}")]
    public sealed class Board
    {
        private readonly Piece?[] _squares = new Piece?[64];

        private string DebuggerView => ToAscii();
        
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
        
        public string ToAscii()
        {
            var sb = new System.Text.StringBuilder(128);

            for (int rank = 7; rank >= 0; rank--)
            {
                sb.Append(rank + 1).Append("  ");

                for (int file = 0; file < 8; file++)
                {
                    var sq = Square.From((File)file, (Rank)rank);
                    var p = _squares[sq.Index];

                    sb.Append(p.HasValue ? PieceToChar(p.Value) : '.');
                    sb.Append(' ');
                }

                sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append("   a b c d e f g h");

            return sb.ToString();
        }
        
        private static char PieceToChar(Piece p)
        {
            char c;
            switch (p.Type)
            {
                case PieceType.Pawn:   c = 'p'; break;
                case PieceType.Knight: c = 'n'; break;
                case PieceType.Bishop: c = 'b'; break;
                case PieceType.Rook:   c = 'r'; break;
                case PieceType.Queen:  c = 'q'; break;
                case PieceType.King:   c = 'k'; break;
                default:               c = '?'; break;
            }

            return p.Color == Color.White ? char.ToUpperInvariant(c) : c;
        }
    }
}