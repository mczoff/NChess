using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Extensions;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Classic.Rules
{
    internal sealed class StepRules
    {
        public static readonly (int df, int dr)[] KnightJumps =
        {
            (+1, +2), (+2, +1), (+2, -1), (+1, -2), (-1, -2), (-2, -1), (-2, +1), (-1, +2)
        };

        public static readonly (int df, int dr)[] KingSteps =
        {
            (+1, +1), (+1, 0), (+1, -1), (0, +1), (0, -1), (-1, +1), (-1, 0), (-1, -1)
        };

        public IEnumerable<Move> Generate(Position position, Square from, Color us, (int df, int dr)[] steps)
        {
            for (var i = 0; i < steps.Length; i++)
            {
                if (!from.TryOffset(steps[i].df, steps[i].dr, out var to))
                    continue;

                var dst = position.GetPiece(to);
                if (!dst.HasValue)
                {
                    yield return new Move(from, to);
                }
                else if (dst.Value.Color != us)
                {
                    yield return new Move(from, to, MoveFlags.Capture);
                }
            }
        }
    }
}