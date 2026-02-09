using System;
using System.Collections.Generic;
using NChess.Core.Pieces;

namespace NChess.Core.Common
{
    public class Position
    {
        private Board _board = new Board();
        public Color SideToMove { get; internal set; } = Color.White;

        public CastlingRights Castling { get; private set; }
        public Square? EnPassantSquare { get; private set; }
        public int HalfmoveClock { get; private set; } = 0;
        public int FullmoveNumber { get; private set; } = 1;


        public void Clear()
        {
            _board.Clear();
            ResetToEmpty();
        }

        private void ResetToEmpty()
        {
            SideToMove = Color.White;
            Castling = CastlingRights.None;
            EnPassantSquare = null;
            HalfmoveClock = 0;
            FullmoveNumber = 1;
        }

        public void SetPiece(Square square, Piece piece) => _board[square] = piece;
        public void ClearSquare(Square square) => _board.Clear(square);
        public IEnumerable<(Square Square, Piece Piece)> Pieces() => _board.Pieces();

        public Piece? GetPiece(Square square) => _board[square];

        public bool TryGetPiece(Square square, out Piece piece)
        {
            var p = _board[square];
            if (p.HasValue)
            {
                piece = p.Value;
                return true;
            }

            piece = default;
            return false;
        }

        public void SetState(
            Color sideToMove,
            CastlingRights castling,
            Square? enPassantSquare,
            int halfmoveClock,
            int fullmoveNumber)
        {
            if (halfmoveClock < 0)
                throw new ArgumentOutOfRangeException(nameof(halfmoveClock), "HalfmoveClock must be >= 0.");
            if (fullmoveNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(fullmoveNumber), "FullmoveNumber must be >= 1.");

            SideToMove = sideToMove;
            Castling = castling;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = halfmoveClock;
            FullmoveNumber = fullmoveNumber;
        }

        internal void SetSideToMove(Color color) => SideToMove = color;
        internal void SetCastling(CastlingRights rights) => Castling = rights;
        internal void SetEnPassant(Square? square) => EnPassantSquare = square;

        internal void SetClocks(int halfmoveClock, int fullmoveNumber)
        {
            if (halfmoveClock < 0) throw new ArgumentOutOfRangeException(nameof(halfmoveClock));
            if (fullmoveNumber < 1) throw new ArgumentOutOfRangeException(nameof(fullmoveNumber));

            HalfmoveClock = halfmoveClock;
            FullmoveNumber = fullmoveNumber;
        }

        public Square FindKingSquare(Color color)
        {
            for (var rank = 0; rank < 8; rank++)
            for (var file = 0; file < 8; file++)
            {
                var square = Square.From((File)file, (Rank)rank);
                var piece = GetPiece(square);

                if (piece.HasValue &&
                    piece.Value.Color == color &&
                    piece.Value.Type == PieceType.King)
                {
                    return square;
                }
            }

            throw new InvalidOperationException($"King not found for color {color}.");
        }

        public PositionSnapshot Snapshot() => PositionSnapshot.Capture(this);

        public void Restore(PositionSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

            _board.Clear();
            foreach (var item in snapshot.Pieces)
                _board[item.Square] = item.Piece;

            SideToMove = snapshot.SideToMove;
            Castling = snapshot.Castling;
            EnPassantSquare = snapshot.EnPassantSquare;
            HalfmoveClock = snapshot.HalfmoveClock;
            FullmoveNumber = snapshot.FullmoveNumber;
        }
    }
}