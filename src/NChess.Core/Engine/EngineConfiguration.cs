namespace NChess.Core.Engine
{
    public sealed class EngineConfiguration
    {
        public bool EnableThreefoldRepetition { get; }
        public bool EnableFiftyMoveRule { get; }
        public bool EnableInsufficientMaterial { get; }
        public bool StrictCastlingRules { get; }

        public EngineConfiguration(
            bool enableThreefoldRepetition = true,
            bool enableFiftyMoveRule = true,
            bool enableInsufficientMaterial = true,
            bool strictCastlingRules = true)
        {
            EnableThreefoldRepetition = enableThreefoldRepetition;
            EnableFiftyMoveRule = enableFiftyMoveRule;
            EnableInsufficientMaterial = enableInsufficientMaterial;
            StrictCastlingRules = strictCastlingRules;
        }

        public static EngineConfiguration Default { get; } = new EngineConfiguration();
    }
}