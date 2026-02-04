using NChess.Core.Engine.Abstractions;
using NChess.Core.Engine.Classic;

namespace NChess.Core.Engine
{
    public static class EngineFactory
    {
        public static IChessEngine CreateDefault()
            => new ClassicChessEngine(new EngineConfiguration());
    }
}