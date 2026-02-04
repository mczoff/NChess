using System;
using System.Collections.Generic;
using NChess.Core.Pieces;

namespace NChess.Core.Common
{
    public sealed class PositionSnapshot
    {
        public IReadOnlyList<(Square Square, Piece Piece)> Pieces { get; }

        public Color SideToMove { get; }
        public CastlingRights Castling { get; }
        public Square? EnPassantSquare { get; }
        public int HalfmoveClock { get; }
        public int FullmoveNumber { get; }

        public PositionSnapshot(
            IReadOnlyList<(Square Square, Piece Piece)> pieces,
            Color sideToMove,
            CastlingRights castling,
            Square? enPassantSquare,
            int halfmoveClock,
            int fullmoveNumber)
        {
            Pieces = pieces ?? throw new ArgumentNullException(nameof(pieces));
            SideToMove = sideToMove;
            Castling = castling;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = halfmoveClock;
            FullmoveNumber = fullmoveNumber;
        }

        public static PositionSnapshot Capture(Position position)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            var list = new List<(Square, Piece)>();
            foreach (var p in position.Pieces())
                list.Add(p);

            return new PositionSnapshot(
                list,
                position.SideToMove,
                position.Castling,
                position.EnPassantSquare,
                position.HalfmoveClock,
                position.FullmoveNumber
            );
        }
    }
}