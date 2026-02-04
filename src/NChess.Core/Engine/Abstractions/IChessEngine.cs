using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Abstractions
{
    public interface IChessEngine
    {
        IAttackDetector Attacks { get; }
        EngineConfiguration Configuration { get; }
        
        IEnumerable<Move> GenerateLegalMoves(Position position);
        
        EngineResult<MoveUndo> MakeMove(Position position, Move move);
        
        void UndoMove(Position position, MoveUndo undo);
    }
}