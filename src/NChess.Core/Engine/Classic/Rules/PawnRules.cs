using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Extensions;
using NChess.Core.Moves;
using NChess.Core.Pieces;

namespace NChess.Core.Engine.Classic.Rules
{
    internal sealed class PawnRules
    {
        public IEnumerable<Move> Generate(Position position, Square from, Color us)
        {
            var dir = us == Color.White ? +1 : -1;
            var startRank = us == Color.White ? Rank.Two : Rank.Seven;
            var promoRank = us == Color.White ? Rank.Eight : Rank.One;
            var them = us == Color.White ? Color.Black : Color.White;

            if (from.TryOffset(0, dir, out var to1) && !position.GetPiece(to1).HasValue)
            {
                if (to1.Rank == promoRank)
                {
                    foreach (var m in Promotions(from, to1, isCapture: false))
                        yield return m;
                }
                else
                {
                    yield return new Move(from, to1);

                    if (from.Rank == startRank &&
                        from.TryOffset(0, 2 * dir, out var to2) &&
                        !position.GetPiece(to2).HasValue)
                    {
                        yield return new Move(from, to2);
                    }
                }
            }

            for (int df = -1; df <= 1; df += 2)
            {
                if (!from.TryOffset(df, dir, out var to))
                    continue;

                var target = position.GetPiece(to);

                if (target.HasValue && target.Value.Color == them)
                {
                    if (to.Rank == promoRank)
                    {
                        foreach (var m in Promotions(from, to, isCapture: true))
                            yield return m;
                    }
                    else
                    {
                        yield return new Move(from, to, MoveFlags.Capture);
                    }

                    continue;
                }

                if (position.EnPassantSquare.HasValue && to.Equals(position.EnPassantSquare.Value))
                {
                    yield return new Move(from, to, MoveFlags.EnPassant | MoveFlags.Capture);
                }
            }
        }

        private static IEnumerable<Move> Promotions(Square from, Square to, bool isCapture)
        {
            var flags = MoveFlags.Promotion | (isCapture ? MoveFlags.Capture : MoveFlags.None);

            yield return new Move(from, to, flags, PieceType.Queen);
            yield return new Move(from, to, flags, PieceType.Rook);
            yield return new Move(from, to, flags, PieceType.Bishop);
            yield return new Move(from, to, flags, PieceType.Knight);
        }
    }
}