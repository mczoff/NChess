using System;
using System.Text;
using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Fen
{
    public static class FenUtils
    {
        public static void Load(string fen, Position position)
        {
            if (fen == null) throw new ArgumentNullException(nameof(fen));
            if (position == null) throw new ArgumentNullException(nameof(position));

            var parts = fen.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                throw new FormatException("Invalid FEN: expected at least 4 fields.");

            position.Clear();

            ParseBoard(parts[0], position);

            var side = ParseSideToMove(parts[1]);
            var castling = ParseCastling(parts[2]);
            var ep = ParseEnPassant(parts[3]);

            var halfmove = parts.Length > 4 ? ParseInt(parts[4], "halfmove clock") : 0;
            var fullmove = parts.Length > 5 ? ParseInt(parts[5], "fullmove number") : 1;

            position.SetState(side, castling, ep, halfmove, fullmove);
        }

        public static string Save(Position position)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));

            var sb = new StringBuilder(96);

            sb.Append(WriteBoard(position));
            sb.Append(' ');
            sb.Append(position.SideToMove == Color.White ? 'w' : 'b');
            sb.Append(' ');

            var castling = position.Castling.ToString();
            sb.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

            sb.Append(' ');
            sb.Append(position.EnPassantSquare.HasValue
                ? Algebraic.FromSquare(position.EnPassantSquare.Value)
                : "-");

            sb.Append(' ');
            sb.Append(position.HalfmoveClock);
            sb.Append(' ');
            sb.Append(position.FullmoveNumber);

            return sb.ToString();
        }

        private static string WriteBoard(Position position)
        {
            var sb = new StringBuilder(72);

            for (var rankIndex = 7; rankIndex >= 0; rankIndex--)
            {
                var empty = 0;

                for (var fileIndex = 0; fileIndex < 8; fileIndex++)
                {
                    var file = (File)fileIndex;
                    var rank = (Rank)rankIndex;
                    var square = Square.From(file, rank);

                    var piece = position.GetPiece(square);
                    if (!piece.HasValue)
                    {
                        empty++;
                        continue;
                    }

                    if (empty > 0)
                    {
                        sb.Append((char)('0' + empty));
                        empty = 0;
                    }

                    sb.Append(WritePiece(piece.Value));
                }

                if (empty > 0)
                    sb.Append((char)('0' + empty));

                if (rankIndex != 0)
                    sb.Append('/');
            }

            return sb.ToString();
        }

        private static void ParseBoard(string text, Position position)
        {
            if (string.IsNullOrEmpty(text))
                throw new FormatException("Invalid board field: empty.");

            var rankIndex = 7;
            var fileIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (c == '/')
                {
                    if (fileIndex != 8)
                        throw new FormatException("Invalid board field: rank does not contain 8 squares.");

                    if (rankIndex == 0)
                        throw new FormatException("Invalid board field: too many ranks.");

                    rankIndex--;
                    fileIndex = 0;
                    continue;
                }

                if (c >= '1' && c <= '8')
                {
                    fileIndex += (c - '0');
                    if (fileIndex > 8)
                        throw new FormatException("Invalid board field: file index overflow.");
                    continue;
                }

                if (fileIndex >= 8)
                    throw new FormatException("Invalid board field: too many pieces in rank.");

                var piece = ParsePiece(c);

                var file = (File)fileIndex;
                var rank = (Rank)rankIndex;
                var square = Square.From(file, rank);

                position.SetPiece(square, piece);
                fileIndex++;
            }

            if (rankIndex != 0 || fileIndex != 8)
                throw new FormatException("Invalid board field: expected 8 ranks of 8 squares.");
        }

        private static int ParseInt(string text, string name)
        {
            if (!int.TryParse(text, out var value) || value < 0)
                throw new FormatException($"Invalid {name}: {text}");
            return value;
        }

        private static Color ParseSideToMove(string text) => text == "w"
            ? Color.White
            : text == "b"
                ? Color.Black
                : throw new FormatException($"Invalid side to move: {text}");

        private static CastlingRights ParseCastling(string text)
        {
            var rights = CastlingRights.None;

            if (text == "-")
                return rights;

            foreach (var c in text)
            {
                rights = c switch
                {
                    'K' => rights.WithWhiteKingSide(true),
                    'Q' => rights.WithWhiteQueenSide(true),
                    'k' => rights.WithBlackKingSide(true),
                    'q' => rights.WithBlackQueenSide(true),
                    _ => throw new FormatException($"Invalid castling symbol: {c}")
                };
            }

            return rights;
        }

        private static Square? ParseEnPassant(string text)
        {
            if (text == "-")
                return null;

            return Algebraic.ParseSquare(text);
        }

        private static Piece ParsePiece(char c)
        {
            var color = char.IsUpper(c) ? Color.White : Color.Black;

            var type = char.ToLowerInvariant(c) switch
            {
                'p' => PieceType.Pawn,
                'n' => PieceType.Knight,
                'b' => PieceType.Bishop,
                'r' => PieceType.Rook,
                'q' => PieceType.Queen,
                'k' => PieceType.King,
                _ => throw new FormatException($"Invalid piece character: {c}")
            };

            return new Piece(type, color);
        }

        private static char WritePiece(Piece piece)
        {
            var c = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Rook => 'r',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => throw new ArgumentOutOfRangeException(nameof(piece))
            };

            return piece.Color == Color.White ? char.ToUpperInvariant(c) : c;
        }
    }
}