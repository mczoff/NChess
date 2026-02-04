namespace NChess.Core.Moves
{
    [System.Flags]
    public enum MoveFlags : ushort
    {
        None        = 0,

        Capture     = 1 << 0,
        Promotion   = 1 << 1,
        EnPassant   = 1 << 2,
        Castling    = 1 << 3,

        Check       = 1 << 4,
        Checkmate   = 1 << 5
    }
}