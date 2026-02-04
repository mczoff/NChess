using NChess.Core.Common;

namespace NChess.Core.Engine.Abstractions
{
    public interface IAttackDetector
    {
        bool IsSquareAttacked(Position position, Square square, Color byColor);
        bool IsKingInCheck(Position position, Color kingColor);
    }
}