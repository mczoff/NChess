using NChess.Core.Common;
using NChess.Core.Pieces;

namespace NChess.Core.Extensions
{
    public static class SquareExtensions
    {
        public static bool TryOffset(this Square square, int df, int dr, out Square result)
        {
            var file = (int)square.File + df;
            var rank = (int)square.Rank + dr;

            if ((uint)file < BoardConstants.Size && (uint)rank < BoardConstants.Size)
            {
                result = Square.From((File)file, (Rank)rank);
                return true;
            }

            result = default;
            return false;
        }
    }
}