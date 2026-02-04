namespace NChess.Core.Engine
{
    public readonly struct EngineResult<T>
    {
        public T Value { get; }
        public EngineError Error { get; }

        public bool IsOk => Error == null;

        private EngineResult(T value, EngineError error)
        {
            Value = value;
            Error = error;
        }

        public static EngineResult<T> Ok(T value) => new EngineResult<T>(value, null);
        public static EngineResult<T> Fail(EngineError error) => new EngineResult<T>(default(T), error);

        public static EngineResult<T> Illegal(string msg)
            => Fail(new EngineError(EngineErrorCode.IllegalMove, msg));
    }
}