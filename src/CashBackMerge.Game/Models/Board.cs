namespace CashBackMerge.Game.Models;

public class Board
{
    private readonly Tile?[,] _board;
    public int MaxRewardCashback { get; }
    public int Size { get; }

    public Board(int boardSize, int maxRewardCashback)
    {
        _board = new Tile?[boardSize, boardSize];
        Size = boardSize;
        MaxRewardCashback = maxRewardCashback;
    }

    public Board WithRandomPlaced(IEnumerable<Tile> initialTiles, Random random)
    {
        var board = new Board(Size, MaxRewardCashback);

        foreach (var tile in initialTiles)
            board.TryPlaceRandom(tile, random);

        return board;
    }

    public Board Clone()
    {
        var cloned = new Board(Size, MaxRewardCashback);

        for (var row = 0; row < Size; row++)
            for (var col = 0; col < Size; col++)
                cloned[row, col] = _board[row, col];

        return cloned;
    }

    public MoveResult TryMove(Direction direction)
    {
        var changed = false;
        var scoreGained = 0;
        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var outer = 0; outer < Size; outer++)
        {
            var line = ReadLine(direction, outer);
            var mergeResult = MergeLine(line);

            if (line.SequenceEqual(mergeResult.Tiles))
                continue;

            scoreGained += mergeResult.ScoreGained;
            changed = true;
            WriteLine(direction, outer, mergeResult.Tiles);
        }

        return new MoveResult(changed, scoreGained);
    }

    public bool HasAvailableMove()
    {
        for (var row = 0; row < Size; row++)
        for (var col = 0; col < Size; col++)
        {
            if (this[row, col] is not Tile current)
                return true;

            if (current.Cashback == MaxRewardCashback)
                continue;

            if (col + 1 < Size &&
                this[row, col + 1] is Tile right &&
                current.Cashback == right.Cashback)
                return true;

            if (row + 1 < Size &&
                this[row + 1, col] is Tile down &&
                current.Cashback == down.Cashback)
                return true;
        }

        return false;
    }

    public bool TryPlaceRandom(Tile tile, Random random)
    {
        var emptyCells = GetEmptyPositions();

        if (emptyCells.Count == 0)
            return false;

        var position = emptyCells[random.Next(emptyCells.Count)];
        _board[position.Row, position.Col] = tile;
        return true;
    }

    public int GetMaxTileValue()
    {
        var maxValue = 0;

        foreach (var tile in _board)
            if (tile.HasValue) 
                maxValue = Math.Max(maxValue, tile.Value.Cashback);

        return maxValue;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Board other)
            return false;

        if (Size != other.Size || 
            MaxRewardCashback != other.MaxRewardCashback)
            return false;

        for (var row = 0; row < Size; row++)
            for (var col = 0; col < Size; col++)
                if (_board[row, col] != other[row, col])
                    return false;

        return true;
    }

    private Tile? this[int row, int column]
    {
        get => _board[row, column];
        set => _board[row, column] = value;
    }

    private List<Tile?> ReadLine(Direction direction, int fixedIndex)
    {
        var line = new List<Tile?>(Size);

        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var i = 0; i < Size; i++)
        {
            var slidingIndex = inverted ? Size - 1 - i : i;

            var row = swapped ? slidingIndex : fixedIndex;
            var col = swapped ? fixedIndex : slidingIndex;

            line.Add(this[row, col]);
        }

        return line;
    }

    private LineMergeResult MergeLine(List<Tile?> line)
    {
        var scoreGained = 0;
        var merged = new List<Tile?>();

        var tiles = line
            .Where(tile => tile.HasValue)
            .Select(tile => tile!.Value)
            .ToList();

        for (var i = 0; i < tiles.Count; i++)
        {
            if (i + 1 < tiles.Count &&
                tiles[i].Cashback < MaxRewardCashback &&
                tiles[i].Cashback == tiles[i + 1].Cashback)
            {
                var cashback = tiles[i++].Cashback * 2;
                merged.Add(new Tile(cashback));
                scoreGained += cashback;
            }
            else merged.Add(tiles[i]);
        }

        while (merged.Count < Size)
            merged.Add(null);

        return new LineMergeResult(merged, scoreGained);
    }

    private void WriteLine(Direction direction, int fixedIndex, List<Tile?> line)
    {
        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var i = 0; i < Size; i++)
        {
            var slidingIndex = inverted ? Size - 1 - i : i;

            var row = swapped ? slidingIndex : fixedIndex;
            var col = swapped ? fixedIndex : slidingIndex;

            this[row, col] = line[i];
        }
    }

    private List<(int Row, int Col)> GetEmptyPositions()
    {
        var emptyCells = new List<(int Row, int Col)>();

        for (var row = 0; row < Size; row++)
            for (var col = 0; col < Size; col++)
                if (this[row, col] is null)
                    emptyCells.Add((row, col));

        return emptyCells;
    }
}
