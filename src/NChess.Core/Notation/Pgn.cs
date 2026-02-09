using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NChess.Core.Common;
using NChess.Core.Engine.Abstractions;
using NChess.Core.Fen;
using NChess.Core.Moves;

namespace NChess.Core.Notation
{
    public static class Pgn
    {
        private enum TokenKind { San, Comment, Result }
        
        private readonly struct Token
        {
            public TokenKind Kind { get; }
            public string Value { get; }
            public Token(TokenKind kind, string value) { Kind = kind; Value = value; }
        }
        
        private static readonly Regex TagRegex = new Regex(@"\[(\w+)\s+""([^""]*)""\]");
        
        public static IReadOnlyList<Move> Import(
            string pgn,
            IChessEngine engine,
            out string startFen,
            out IReadOnlyDictionary<string, string> tags,
            out string result)
        {
            if (string.IsNullOrWhiteSpace(pgn))
                throw new ArgumentException("PGN is empty.", nameof(pgn));
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            // 1) tags
            var tagDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in TagRegex.Matches(pgn))
                tagDict[m.Groups[1].Value] = m.Groups[2].Value;

            tags = tagDict;

            startFen = tagDict.TryGetValue("FEN", out var fen) && !string.IsNullOrWhiteSpace(fen)
                ? fen
                : FenConstants.StartPosition;

            result = tagDict.TryGetValue("Result", out var r) ? r : "*";
            
            var movetext = TagRegex.Replace(pgn, " ");
            
            var tokens = TokenizeMovetext(movetext);
            
            var position = new Position();
            FenUtils.Load(startFen, position);

            var moves = new List<Move>(256);
            int lastMoveIndex = -1;

            foreach (var t in tokens)
            {
                if (t.Kind == TokenKind.Result)
                {
                    result = t.Value;
                    break;
                }

                if (t.Kind == TokenKind.Comment)
                {
                    if (lastMoveIndex >= 0)
                    {
                        var prev = moves[lastMoveIndex];
                        var merged = MergeComments(prev.Comment, t.Value);
                        moves[lastMoveIndex] = prev.WithComment(merged);
                    }
                    continue;
                }
                
                var san = t.Value;
                
                if (san.EndsWith(".") || san.Contains("..."))
                    continue;

                if (!San.TryParse(position, engine, san, out var move))
                    throw new InvalidOperationException($"Invalid SAN in PGN: {san}");
                
                var res = engine.MakeMove(position, move);
                if (!res.IsOk)
                    throw new InvalidOperationException($"Illegal move in PGN: {san}");

                moves.Add(move);
                lastMoveIndex = moves.Count - 1;
            }

            return moves;
        }
        
        private static List<Token> TokenizeMovetext(string s)
        {
            var tokens = new List<Token>(256);
            int i = 0;

            while (i < s.Length)
            {
                if (char.IsWhiteSpace(s[i])) { i++; continue; }
                
                if (s[i] == ';')
                {
                    while (i < s.Length && s[i] != '\n') i++;
                    continue;
                }
                
                if (s[i] == '{')
                {
                    i++;
                    var start = i;
                    while (i < s.Length && s[i] != '}') i++;
                    var comment = s.Substring(start, Math.Max(0, i - start)).Trim();
                    if (comment.Length > 0)
                        tokens.Add(new Token(TokenKind.Comment, comment));
                    if (i < s.Length && s[i] == '}') i++;
                    continue;
                }

                // variations '( ... )' — пропускаем целиком (вложенность учитываем)
                if (s[i] == '(')
                {
                    int depth = 1;
                    i++;
                    while (i < s.Length && depth > 0)
                    {
                        if (s[i] == '(') depth++;
                        else if (s[i] == ')') depth--;
                        i++;
                    }
                    continue;
                }

                // NAG $n — пропускаем
                if (s[i] == '$')
                {
                    i++;
                    while (i < s.Length && char.IsDigit(s[i])) i++;
                    continue;
                }

                // обычный токен до пробела/коммента/вариации
                var j = i;
                while (j < s.Length && !char.IsWhiteSpace(s[j]) && s[j] != '{' && s[j] != '(')
                    j++;

                var tok = s.Substring(i, j - i);
                i = j;

                if (tok == "1-0" ||
                    tok == "0-1" ||
                    tok == "1/2-1/2" ||
                    tok == "*")
                {
                    tokens.Add(new Token(TokenKind.Result, tok));
                    continue;
                }
                
                tok = tok.Trim();
                
                if (tok.EndsWith(".") || tok.EndsWith("..."))
                    continue;

                tokens.Add(new Token(TokenKind.San, tok));
            }

            return tokens;
        }

        private static string MergeComments(string? existing, string add)
            => string.IsNullOrWhiteSpace(existing) ? add : existing + " " + add;
        
        public static string Export(
            string startFen,
            IEnumerable<Move> moves,
            IChessEngine engine)
        {
            var sb = new StringBuilder();
            var position = new Position();

            FenUtils.Load(startFen, position);

            int ply = 0;

            foreach (var move in moves)
            {
                if (ply % 2 == 0)
                    sb.Append($"{(ply / 2) + 1}. ");

                var san = San.Format(position, engine, move);
                sb.Append(san);

                var c = move.Comment;
                if (!string.IsNullOrWhiteSpace(c))
                    sb.Append(" {")
                        .Append(EscapeComment(c))
                        .Append('}');

                sb.Append(' ');

                var res = engine.MakeMove(position, move);
                if (!res.IsOk)
                    throw new InvalidOperationException(
                        $"Illegal move during PGN export: {move}");

                ply++;
            }

            sb.Append('*');
            return sb.ToString().Trim();
        }

        private static string EscapeComment(string c)
            => c.Replace("{", "(").Replace("}", ")");
    }
}