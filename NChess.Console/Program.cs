using NChess;

var chess = new Chess();


var pgnGame = await File.ReadAllTextAsync("./Resources/lichess.pgn");

chess.ImportPgn(pgnGame); 
    
Console.WriteLine(chess.Fen);