namespace NChess.Core.Moves
{
    public class MoveEntry
    {
        public Move Move { get; }
        public MoveUndo Undo { get; }
        public string San { get; }

        public MoveEntry(Move move, MoveUndo undo, string san)
        {
            Move = move;
            Undo = undo;
            San = san;
        }

        public override string ToString() => San;
    }
}