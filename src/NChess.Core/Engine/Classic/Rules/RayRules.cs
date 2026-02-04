using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Extensions;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Classic.Rules
{
    internal sealed class RayRules
    {
        public static readonly (int df, int dr)[] Diagonals =
        {
            (+1,+1),(+1,-1),(-1,+1),(-1,-1)
        };

        public static readonly (int df, int dr)[] Orthogonals =
        {
            (+1,0),(-1,0),(0,+1),(0,-1)
        };

        public static readonly (int df, int dr)[] QueenDirs =
        {
            (+1,+1),(+1,-1),(-1,+1),(-1,-1),(+1,0),(-1,0),(0,+1),(0,-1)
        };

        public IEnumerable<Move> Generate(Position position, Square from, Color us, (int df, int dr)[] dirs)
        {
            for (var d = 0; d < dirs.Length; d++)
            {
                var sq = from;

                while (sq.TryOffset(dirs[d].df, dirs[d].dr, out var next))
                {
                    sq = next;

                    var dst = position.GetPiece(sq);
                    if (!dst.HasValue)
                    {
                        yield return new Move(from, sq);
                        continue;
                    }

                    if (dst.Value.Color != us)
                        yield return new Move(from, sq, MoveFlags.Capture);

                    break;
                }
            }
        }
    }
}