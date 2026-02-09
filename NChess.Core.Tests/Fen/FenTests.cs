using NChess.Core.Common;
using NChess.Core.Fen;
using NChess.Core.Pieces;
using File = NChess.Core.Common.File;

namespace NChess.Core.Tests.Fen;

[TestFixture]
public sealed class FenTests
{
    [Test]
    public void Load_NullFen_ThrowsArgumentNullException()
    {
        var pos = new Position();
        Assert.Throws<ArgumentNullException>(() => FenUtils.Load(null, pos));
    }

    [Test]
    public void Load_NullPosition_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => FenUtils.Load(FenConstants.StartPosition, null));
    }

    [Test]
    public void Save_NullPosition_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => FenUtils.Save(null));
    }

    [Test]
    public void StartPosition_LoadSave_RoundTrip_IsStable()
    {
        // Arrange
        var pos1 = new Position();
        FenUtils.Load(FenConstants.StartPosition, pos1);

        // Act
        var fen1 = FenUtils.Save(pos1);

        var pos2 = new Position();
        FenUtils.Load(fen1, pos2);
        var fen2 = FenUtils.Save(pos2);

        // Assert
        Assert.That(fen2, Is.EqualTo(fen1));
    }

    [Test]
    public void Load_MinimalFen_UsesDefaultHalfmoveAndFullmove()
    {
        // Arrange
        var fen = "8/8/8/8/8/8/8/8 w - -";

        // Act
        var pos = new Position();
        FenUtils.Load(fen, pos);

        // Assert
        Assert.That(pos.HalfmoveClock, Is.EqualTo(0));
        Assert.That(pos.FullmoveNumber, Is.EqualTo(1));

        // Save должен дописать поля
        Assert.That(FenUtils.Save(pos), Is.EqualTo("8/8/8/8/8/8/8/8 w - - 0 1"));
    }

    [Test]
    public void Load_WithStateFields_SetsStateCorrectly()
    {
        // Arrange
        var fen = "8/8/8/8/8/8/8/8 b Kq e3 12 34";

        // Act
        var pos = new Position();
        FenUtils.Load(fen, pos);

        // Assert
        Assert.That(pos.SideToMove, Is.EqualTo(Color.Black));
        Assert.That(pos.Castling.ToString(), Is.EqualTo("Kq"));
        Assert.That(pos.EnPassantSquare, Is.EqualTo(Algebraic.ParseSquare("e3")));
        Assert.That(pos.HalfmoveClock, Is.EqualTo(12));
        Assert.That(pos.FullmoveNumber, Is.EqualTo(34));
    }

    [Test]
    public void Load_TooFewFields_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w -", pos));
    }

    [Test]
    public void Load_EmptyBoardField_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load(" w - - 0 1", pos));
    }

    [Test]
    public void Load_InvalidSideToMove_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 x - - 0 1", pos));
    }

    [Test]
    public void Load_InvalidCastlingSymbol_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w KX - 0 1", pos));
    }

    [Test]
    public void Load_InvalidHalfmove_ThrowsFormatException()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - - -1 1", pos));
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - - xx 1", pos));
    }

    [Test]
    public void Load_InvalidFullmove_ThrowsFormatException()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - - 0 -5", pos));
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - - 0 xx", pos));
    }

    [Test]
    public void Load_InvalidPieceChar_ThrowsFormatException()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/7Z w - - 0 1", pos));
    }

    [Test]
    public void Load_BoardRankNot8Squares_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/7 w - - 0 1", pos));
    }

    [Test]
    public void Load_FileIndexOverflow_ThrowsFormatException()
    {
        var pos = new Position();
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/9 w - - 0 1", pos));
    }

    [Test]
    public void Load_TooManyRanks_ThrowsFormatException()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8/8 w - - 0 1", pos));
    }

    [Test]
    public void Load_TooManyPiecesInRank_ThrowsFormatException()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8p/8/8/8/8/8/8/8 w - - 0 1", pos));
    }

    [Test]
    public void Load_EnPassant_InvalidSquare_Throws()
    {
        var pos = new Position();

        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - e9 0 1", pos));
        Assert.Throws<FormatException>(() => FenUtils.Load("8/8/8/8/8/8/8/8 w - z3 0 1", pos));
    }

    [Test]
    public void Save_WritesPiecesCorrectly_ForSimplePosition()
    {
        //Arrange
        var pos = new Position();
        pos.Clear();
        pos.SetPiece(Square.From(File.E, Rank.One), new Piece(PieceType.King, Color.White));
        pos.SetPiece(Square.From(File.E, Rank.Eight), new Piece(PieceType.King, Color.Black));
        pos.SetState(Color.White, CastlingRights.None, null, 0, 1);

        // Act
        var fen = FenUtils.Save(pos);

        // Assert 
        Assert.That(fen, Is.EqualTo("4k3/8/8/8/8/8/8/4K3 w - - 0 1"));
    }

    [Test]
    public void Load_AllowsExtraSpaces_NormalizesOnSave()
    {
        // Arrange
        var raw = "   8/8/8/8/8/8/8/8    w   -   -   0   1   ";

        // Act
        var pos = new Position();
        FenUtils.Load(raw, pos);
        var saved = FenUtils.Save(pos);

        // Assert
        Assert.That(saved, Is.EqualTo("8/8/8/8/8/8/8/8 w - - 0 1"));
    }

    [Test]
    public void Load_CastlingOrderDoesNotMatter()
    {
        // Arrange
        var fen1 = "8/8/8/8/8/8/8/8 w KQkq - 0 1";
        var fen2 = "8/8/8/8/8/8/8/8 w qkQK - 0 1";

        // Act
        var p1 = new Position();
        var p2 = new Position();
        FenUtils.Load(fen1, p1);
        FenUtils.Load(fen2, p2);

        // Assert: 
        Assert.That(FenUtils.Save(p2), Is.EqualTo(FenUtils.Save(p1)));
    }

    [Test]
    public void Load_CastlingDuplicates_DoNotBreak()
    {
        // Arrange: 
        var fen = "8/8/8/8/8/8/8/8 w KKKqq - 0 1";

        // Act
        var pos = new Position();
        FenUtils.Load(fen, pos);
        var saved = FenUtils.Save(pos);

        // Assert
        Assert.That(saved, Is.EqualTo("8/8/8/8/8/8/8/8 w Kq - 0 1"));
    }

    [Test]
    public void Save_AlwaysProducesSixFields()
    {
        // Arrange
        var pos = new Position();
        FenUtils.Load("8/8/8/8/8/8/8/8 w - -", pos);

        // Act
        var fen = FenUtils.Save(pos);
        var parts = fen.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.That(parts.Length, Is.EqualTo(6));
        Assert.That(parts[0], Is.Not.Empty); // board
        Assert.That(parts[1], Is.EqualTo("w").Or.EqualTo("b"));
        Assert.That(parts[2], Is.Not.Empty); // castling or "-"
        Assert.That(parts[3], Is.Not.Empty); // ep or "-"
        Assert.That(int.TryParse(parts[4], out _), Is.True);
        Assert.That(int.TryParse(parts[5], out _), Is.True);
    }

    [Test]
    public void LoadSave_IsIdempotent()
    {
        // Arrange
        var raw = " 8/8/8/8/8/8/8/8  b  qkQK  e3  12  34 ";

        // Act
        var p1 = new Position();
        FenUtils.Load(raw, p1);
        var canon1 = FenUtils.Save(p1);

        var p2 = new Position();
        FenUtils.Load(canon1, p2);
        var canon2 = FenUtils.Save(p2);

        // Assert
        Assert.That(canon2, Is.EqualTo(canon1));
    }

    [Test]
    public void Board_MixedDigitsAndPieces_RoundTrip()
    {
        // Arrange
        var fen = "3p2N1/8/8/8/8/8/8/1k2K3 w - - 0 1";

        // Act
        var pos = new Position();
        FenUtils.Load(fen, pos);
        var saved = FenUtils.Save(pos);

        // Assert
        var pos2 = new Position();
        FenUtils.Load(saved, pos2);
        var saved2 = FenUtils.Save(pos2);

        Assert.That(saved2, Is.EqualTo(saved));
    }
}