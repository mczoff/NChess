using System;
using System.Collections.Generic;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Moves;

namespace NChess.Core.Engine.Classic.Rules
{
    internal sealed class CastlingRules
    {
        private readonly IAttackDetector _attacks;
        private readonly EngineConfiguration _config;

        public CastlingRules(IAttackDetector attacks, EngineConfiguration config)
        {
            _attacks = attacks ?? throw new ArgumentNullException(nameof(attacks));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IEnumerable<Move> Generate(Position position, Color us)
        {
            var rights = position.Castling.ToString();
            if (string.IsNullOrEmpty(rights) || rights == "-")
                yield break;

            var them = us == Color.White ? Color.Black : Color.White;

            if (us == Color.White)
            {
                var e1 = Square.From(File.E, Rank.One);

                if (rights.IndexOf('K') >= 0)
                {
                    var f1 = Square.From(File.F, Rank.One);
                    var g1 = Square.From(File.G, Rank.One);

                    if (!position.GetPiece(f1).HasValue && !position.GetPiece(g1).HasValue)
                    {
                        if (!_config.StrictCastlingRules || PathSafe(position, them, e1, f1, g1))
                            yield return new Move(e1, g1, MoveFlags.Castling);
                    }
                }

                if (rights.IndexOf('Q') >= 0)
                {
                    var d1 = Square.From(File.D, Rank.One);
                    var c1 = Square.From(File.C, Rank.One);
                    var b1 = Square.From(File.B, Rank.One);

                    if (!position.GetPiece(d1).HasValue && !position.GetPiece(c1).HasValue &&
                        !position.GetPiece(b1).HasValue)
                    {
                        if (!_config.StrictCastlingRules || PathSafe(position, them, e1, d1, c1))
                            yield return new Move(e1, c1, MoveFlags.Castling);
                    }
                }
            }
            else
            {
                var e8 = Square.From(File.E, Rank.Eight);

                if (rights.IndexOf('k') >= 0)
                {
                    var f8 = Square.From(File.F, Rank.Eight);
                    var g8 = Square.From(File.G, Rank.Eight);

                    if (!position.GetPiece(f8).HasValue && !position.GetPiece(g8).HasValue)
                    {
                        if (!_config.StrictCastlingRules || PathSafe(position, them, e8, f8, g8))
                            yield return new Move(e8, g8, MoveFlags.Castling);
                    }
                }

                if (rights.IndexOf('q') >= 0)
                {
                    var d8 = Square.From(File.D, Rank.Eight);
                    var c8 = Square.From(File.C, Rank.Eight);
                    var b8 = Square.From(File.B, Rank.Eight);

                    if (!position.GetPiece(d8).HasValue && !position.GetPiece(c8).HasValue &&
                        !position.GetPiece(b8).HasValue)
                    {
                        if (!_config.StrictCastlingRules || PathSafe(position, them, e8, d8, c8))
                            yield return new Move(e8, c8, MoveFlags.Castling);
                    }
                }
            }
        }

        private bool PathSafe(Position position, Color attacker, Square kingFrom, Square pass, Square kingTo)
            => !_attacks.IsSquareAttacked(position, kingFrom, attacker)
               && !_attacks.IsSquareAttacked(position, pass, attacker)
               && !_attacks.IsSquareAttacked(position, kingTo, attacker);
    }
}