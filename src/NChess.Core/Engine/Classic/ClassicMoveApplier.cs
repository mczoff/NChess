using System;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Moves;
using NChess.Core.Pieces;

namespace NChess.Core.Engine.Classic
{
    internal sealed class ClassicMoveApplier : IMoveApplier
    {
        public EngineResult<MoveUndo> MakeMove(Position position, Move move)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            if (!position.TryGetPiece(move.From, out var moved))
                return EngineResult<MoveUndo>.Illegal("No piece on the source square.");

            if (moved.Color != position.SideToMove)
                return EngineResult<MoveUndo>.Illegal("It is not the moving side's turn.");

            if (position.TryGetPiece(move.To, out var target) && target.Color == moved.Color)
                return EngineResult<MoveUndo>.Illegal("Cannot capture own piece.");
            
            Piece? captured;
            Square? epCapturedSquare = null;

            if (move.IsEnPassant)
            {
                epCapturedSquare = GetEnPassantCapturedSquare(moved, move);
                captured = position.GetPiece(epCapturedSquare.Value);
                if (!captured.HasValue)
                    return EngineResult<MoveUndo>.Illegal("En-passant capture square is empty.");
            }
            else
            {
                captured = position.GetPiece(move.To);
            }

            var undo = new MoveUndo(
                move.From,
                move.To,
                moved,
                captured,
                position.SideToMove,
                position.Castling,
                position.EnPassantSquare,
                position.HalfmoveClock,
                position.FullmoveNumber
            );
            
            if (move.IsCastling)
            {
                ApplyCastling(position, moved, move);
            }
            else
            {
                position.ClearSquare(move.From);

                if (move.IsEnPassant && epCapturedSquare.HasValue)
                    position.ClearSquare(epCapturedSquare.Value);

                if (move.IsPromotion)
                {
                    if (!move.Promotion.HasValue)
                        return EngineResult<MoveUndo>.Illegal("Promotion piece is not specified.");

                    position.SetPiece(move.To, new Piece(move.Promotion.Value, moved.Color));
                }
                else
                {
                    position.SetPiece(move.To, moved);
                }
            }

            var nextSide = position.SideToMove == Color.White ? Color.Black : Color.White;

            var nextCastling = UpdateCastling(position.Castling, moved, move.From, move.To, captured);
            var nextEp = ComputeEnPassantSquare(moved, move);
            var nextHalfmove = ComputeHalfmoveClock(position.HalfmoveClock, moved, captured, move);
            var nextFullmove = ComputeFullmoveNumber(position.FullmoveNumber, position.SideToMove);

            position.SetState(nextSide, nextCastling, nextEp, nextHalfmove, nextFullmove);

            return EngineResult<MoveUndo>.Ok(undo);
        }

        public void UndoMove(Position position, MoveUndo undo)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));
            if (undo == null) throw new ArgumentNullException(nameof(undo));

            position.SetState(
                undo.PrevSideToMove,
                undo.PrevCastling,
                undo.PrevEnPassant,
                undo.PrevHalfmoveClock,
                undo.PrevFullmoveNumber
            );
            
            if (undo.MovedPiece.Type == PieceType.King && Math.Abs((int)undo.To.File - (int)undo.From.File) == 2)
            {
                UndoCastling(position, undo);
                return;
            }
            
            if (undo.MovedPiece.Type == PieceType.Pawn &&
                undo.From.File != undo.To.File &&
                !undo.CapturedPiece.HasValue)
            {
                UndoEnPassant(position, undo);
                return;
            }

            // Обычный/взятие/промо
            position.ClearSquare(undo.To);
            position.SetPiece(undo.From, undo.MovedPiece);

            if (undo.CapturedPiece.HasValue)
                position.SetPiece(undo.To, undo.CapturedPiece.Value);
        }

        private static void ApplyCastling(Position position, Piece king, Move move)
        {
            // Move king
            position.ClearSquare(move.From);
            position.SetPiece(move.To, king);

            var rank = move.From.Rank;
            var kingSide = move.To.File > move.From.File;

            var rookFrom = Square.From(kingSide ? File.H : File.A, rank);
            var rookTo = Square.From(kingSide ? File.F : File.D, rank);

            if (!position.TryGetPiece(rookFrom, out var rook) || rook.Type != PieceType.Rook)
                throw new InvalidOperationException("Castling rook not found on rookFrom.");

            position.ClearSquare(rookFrom);
            position.SetPiece(rookTo, rook);
        }

        private static void UndoCastling(Position position, MoveUndo undo)
        {
            // Move king back
            position.ClearSquare(undo.To);
            position.SetPiece(undo.From, undo.MovedPiece);

            var rank = undo.From.Rank;
            var kingSide = undo.To.File > undo.From.File;

            var rookFrom = Square.From(kingSide ? File.H : File.A, rank);
            var rookTo = Square.From(kingSide ? File.F : File.D, rank);

            if (!position.TryGetPiece(rookTo, out var rook) || rook.Type != PieceType.Rook)
                throw new InvalidOperationException("Castling rook not found on rookTo during undo.");

            position.ClearSquare(rookTo);
            position.SetPiece(rookFrom, rook);
        }

        private static Square GetEnPassantCapturedSquare(Piece moved, Move move)
        {
            return Square.From(move.To.File, move.From.Rank);
        }

        private static void UndoEnPassant(Position position, MoveUndo undo)
        {
            position.ClearSquare(undo.To);
            position.SetPiece(undo.From, undo.MovedPiece);
            
            var capturedSq = Square.From(undo.To.File, undo.From.Rank);
            
            if (!undo.CapturedPiece.HasValue)
                throw new InvalidOperationException("En-passant undo requires captured piece to be saved.");

            position.SetPiece(capturedSq, undo.CapturedPiece.Value);
        }
        
        private static int ComputeHalfmoveClock(int current, Piece moved, Piece? captured, Move move)
        {
            if (moved.Type == PieceType.Pawn) return 0;
            if (captured.HasValue || move.IsCapture) return 0;
            return current + 1;
        }

        private static int ComputeFullmoveNumber(int current, Color sideBefore)
            => sideBefore == Color.Black ? current + 1 : current;

        private static Square? ComputeEnPassantSquare(Piece moved, Move move)
        {
            if (moved.Type != PieceType.Pawn) return null;

            var from = move.From;
            var to = move.To;

            if (from.File != to.File) return null;

            var d = (int)to.Rank - (int)from.Rank;
            
            if (d == 2) return Square.From(from.File, (Rank)((int)from.Rank + 1));
            if (d == -2) return Square.From(from.File, (Rank)((int)from.Rank - 1));

            return null;
        }

        private static CastlingRights UpdateCastling(
            CastlingRights rights,
            Piece moved,
            Square from,
            Square to,
            Piece? captured)
        {
            var r = rights;

            if (moved.Type == PieceType.King)
            {
                r = moved.Color == Color.White
                    ? r.WithWhiteKingSide(false).WithWhiteQueenSide(false)
                    : r.WithBlackKingSide(false).WithBlackQueenSide(false);
            }

            if (moved.Type == PieceType.Rook)
            {
                if (moved.Color == Color.White)
                {
                    if (from.File == File.A && from.Rank == Rank.One) r = r.WithWhiteQueenSide(false);
                    if (from.File == File.H && from.Rank == Rank.One) r = r.WithWhiteKingSide(false);
                }
                else
                {
                    if (from.File == File.A && from.Rank == Rank.Eight) r = r.WithBlackQueenSide(false);
                    if (from.File == File.H && from.Rank == Rank.Eight) r = r.WithBlackKingSide(false);
                }
            }

            if (captured.HasValue && captured.Value.Type == PieceType.Rook)
            {
                var c = captured.Value;

                if (c.Color == Color.White)
                {
                    if (to.File == File.A && to.Rank == Rank.One) r = r.WithWhiteQueenSide(false);
                    if (to.File == File.H && to.Rank == Rank.One) r = r.WithWhiteKingSide(false);
                }
                else
                {
                    if (to.File == File.A && to.Rank == Rank.Eight) r = r.WithBlackQueenSide(false);
                    if (to.File == File.H && to.Rank == Rank.Eight) r = r.WithBlackKingSide(false);
                }
            }

            return r;
        }
    }
}