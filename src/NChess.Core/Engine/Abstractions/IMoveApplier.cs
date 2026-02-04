using NChess.Core.Common;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Abstractions
{
    public interface IMoveApplier
    {
        EngineResult<MoveUndo> MakeMove(Position position, Move move);
        void UndoMove(Position position, MoveUndo undo);
    }
}