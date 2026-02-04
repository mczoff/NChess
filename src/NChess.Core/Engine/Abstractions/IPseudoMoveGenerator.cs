using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Abstractions
{
    public interface IPseudoMoveGenerator
    {
        IEnumerable<Move> Generate(Position position);
    }
}