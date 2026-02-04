using System;
using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Extensions;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Classic
{
    public sealed class ClassicChessEngine : IChessEngine
    {
        public IAttackDetector Attacks { get; }
        
        private readonly IMoveApplier _applier;
        private readonly IPseudoMoveGenerator _pseudoMoveGenerator;

        public EngineConfiguration Configuration { get; }

        public ClassicChessEngine(EngineConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Attacks = new ClassicAttackDetector();
            
            _applier = new ClassicMoveApplier();
            _pseudoMoveGenerator = new ClassicPseudoMoveGenerator(Attacks, configuration);
        }

        public IEnumerable<Move> GenerateLegalMoves(Position position)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            foreach (var preudoMove in GeneratePseudoLegalMoves(position))
            {
                var res = MakeMove(position, preudoMove);
                if (!res.IsOk)
                    continue;

                var undo = res.Value;

                var movedSide = position.SideToMove.Opposite();
                var illegal = Attacks.IsKingInCheck(position, movedSide);

                UndoMove(position, undo);

                if (!illegal)
                    yield return preudoMove;
            }
        }


        public EngineResult<MoveUndo> MakeMove(Position position, Move move)
            => _applier.MakeMove(position, move);

        public void UndoMove(Position position, MoveUndo undo)
            => _applier.UndoMove(position, undo);

        private IEnumerable<Move> GeneratePseudoLegalMoves(Position position)
            => _pseudoMoveGenerator.Generate(position);
    }
}