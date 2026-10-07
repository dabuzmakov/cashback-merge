using System.Text;

namespace CashBackMerge.Game.Models;

public class Board
{
    private readonly Tile?[,] _board;
    public int Size { get; }

    public Board(int boardSize)
    {
        _board = new Tile?[boardSize, boardSize];
        Size = boardSize;
    }

    public Tile? this[int row, int column]
    {
        get => _board[row, column];
        set => _board[row, column] = value;
    }

    public void Move(Direction direction)
    {
        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var outer = 0; outer < Size; outer++)
        {
            var queue = CreateMergeQueue(direction, outer);
            var merged = MergeLine(queue);

            ClearLine(direction, outer);

            var pointer = new BufferPointer(Size, inverted);
            foreach (var tile in merged)
            {
                var slidingIndex = pointer.Next();
                var row = swapped ? slidingIndex : outer;
                var col = swapped ? outer : slidingIndex;

                this[row, col] = tile;
            }
        }
    }

    private Queue<Tile> CreateMergeQueue(Direction direction, int fixedIndex)
    {
        var mergeQueue = new Queue<Tile>(Size);

        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var i = 0; i < Size; i++)
        {
            var slidingIndex = inverted ? Size - 1 - i : i;

            var row = swapped ? slidingIndex : fixedIndex;
            var col = swapped ? fixedIndex : slidingIndex;

            if (this[row, col] != null)
                mergeQueue.Enqueue(this[row, col]!.Value);
        }

        return mergeQueue;
    }

    private List<Tile> MergeLine(Queue<Tile> mergeQueue)
    {
        var merged = new List<Tile>();

        while (mergeQueue.Count != 0)
        {
            var currentTile = mergeQueue.Dequeue();

            if (!mergeQueue.TryPeek(out var tile) || tile.Cashback != currentTile.Cashback)
            {
                merged.Add(new Tile(currentTile.Cashback));
            }
            else
            {
                mergeQueue.Dequeue();
                merged.Add(new Tile(currentTile.Cashback * 2));
            }
        }

        return merged;
    }

    private void ClearLine(Direction direction, int fixedIndex)
    {
        var swapped = direction is Direction.Up or Direction.Down;

        for (var i = 0; i < Size; i++)
        {
            var row = swapped ? i : fixedIndex;
            var col = swapped ? fixedIndex : i;

            this[row, col] = null;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                var value = _board[row, column]?.Cashback.ToString() ?? ".";

                builder.Append($"{value,4}");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private class BufferPointer
    {
        private readonly bool _inverted;
        public int Value { get; private set; }

        public BufferPointer(int bufferLength, bool inverted)
        {
            _inverted = inverted;
            Value = inverted ? bufferLength - 1 : 0;
        }

        public int Next()
            => _inverted ? Value-- : Value++;
    }
}
