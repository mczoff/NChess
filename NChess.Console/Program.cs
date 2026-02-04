using NChess;

var chess = new Chess();

chess.TryMoveUci("e2e4");

var legalMoves = chess.LegalMoves().ToList();
var legalMovesUci = chess.LegalMovesUci().ToList();
var legalMovesSan = chess.LegalMovesSan().ToList();

Console.WriteLine(chess.Fen);