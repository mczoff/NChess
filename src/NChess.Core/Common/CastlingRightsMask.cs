using System;

namespace NChess.Core.Common
{
    [Flags]
    public enum CastlingRightsMask : byte
    {
        None = 0,

        WhiteKingSide  = 1 << 0,
        WhiteQueenSide = 1 << 1,
        BlackKingSide  = 1 << 2,
        BlackQueenSide = 1 << 3
    }
}