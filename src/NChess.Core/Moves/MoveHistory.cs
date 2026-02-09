using System.Collections.Generic;
using System.Linq;

namespace NChess.Core.Moves
{
    public sealed class MoveHistory
    {
        private readonly Stack<MoveEntry> _stack;

        public int Count => _stack.Count;

        public MoveHistory(int capacity = 128)
        {
            _stack = new Stack<MoveEntry>(capacity);
        }

        public void Clear() => _stack.Clear();

        public void Push(in MoveEntry entry) => _stack.Push(entry);

        public MoveEntry Pop() => _stack.Pop();

        public bool TryPeek(out MoveEntry entry)
        {
            if (_stack.Count == 0)
            {
                entry = default;
                return false;
            }

            entry = _stack.Peek();
            return true;
        }
        
        public IReadOnlyList<MoveEntry> ToList()
            => _stack.Reverse().ToList();

        public IEnumerable<MoveEntry> EnumerateFromStart()
            => _stack.Reverse();
    }
}