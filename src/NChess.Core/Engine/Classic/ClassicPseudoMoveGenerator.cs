using System;
using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Engine.Classic.Rules;
using NChess.Core.Moves;
using NChess.Core.Pieces;

namespace NChess.Core.Engine.Classic
{
    public class ClassicPseudoMoveGenerator: IPseudoMoveGenerator
    {
        private readonly IAttackDetector _attacks;
        private readonly EngineConfiguration _config;

        private readonly StepRules _steps = new StepRules();
        private readonly RayRules _rays = new RayRules();
        private readonly PawnRules _pawns = new PawnRules();
        private readonly CastlingRules _castling;
        
        public ClassicPseudoMoveGenerator(IAttackDetector attacks, EngineConfiguration config)
        {
            _attacks = attacks ?? throw new ArgumentNullException(nameof(attacks));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _castling = new CastlingRules(_attacks, _config);
        }
        
        public IEnumerable<Move> Generate(Position position)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            var us = position.SideToMove;

            for (int rank = 0; rank < 8; rank++)
            for (int file = 0; file < 8; file++)
            {
                var from = Square.From((File)file, (Rank)rank);

                if (!position.TryGetPiece(from, out var piece))
                    continue;

                if (piece.Color != us)
                    continue;

                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        foreach (var m in _pawns.Generate(position, from, us))
                            yield return m;
                        break;

                    case PieceType.Knight:
                        foreach (var m in _steps.Generate(position, from, us, StepRules.KnightJumps))
                            yield return m;
                        break;

                    case PieceType.Bishop:
                        foreach (var m in _rays.Generate(position, from, us, RayRules.Diagonals))
                            yield return m;
                        break;

                    case PieceType.Rook:
                        foreach (var m in _rays.Generate(position, from, us, RayRules.Orthogonals))
                            yield return m;
                        break;

                    case PieceType.Queen:
                        foreach (var m in _rays.Generate(position, from, us, RayRules.QueenDirs))
                            yield return m;
                        break;

                    case PieceType.King:
                        foreach (var m in _steps.Generate(position, from, us, StepRules.KingSteps))
                            yield return m;

                        foreach (var m in _castling.Generate(position, us))
                            yield return m;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}