namespace NChess.Core.Engine
{
    public sealed class EngineError
    {
        public EngineErrorCode Code { get; }
        public string Message { get; }

        public EngineError(EngineErrorCode code, string message)
        {
            Code = code;
            Message = message ?? "";
        }
    }
}