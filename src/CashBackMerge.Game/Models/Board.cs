using CashBackMerge.Game.Configuration;
using System.Text;

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

    public Tile? this[int row, int column]
    {
        get => _board[row, column];
        set => _board[row, column] = value;
    }

    public bool TryMove(Direction direction)
    {
        var changed = false;
        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var outer = 0; outer < Size; outer++)
        {
            var line = ReadLine(direction, outer);
            var merged = MergeLine(line);

            if (line.SequenceEqual(merged))
                continue;

            changed = true;
            WriteLine(direction, outer, merged);
        }

        return changed;
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

    public List<(int Row, int Col)> GetEmptyPositions()
    {
        var emptyCells = new List<(int Row, int Col)>();

        for (var row = 0; row < Size; row++)
            for (var col = 0; col < Size; col++)
                if (this[row, col] is null)
                    emptyCells.Add((row, col));

        return emptyCells;
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

    private List<Tile?> MergeLine(List<Tile?> line)
    {
        var merged = new List<Tile?>();

        var tiles = line
            .Where(tile => tile.HasValue)
            .Select(tile => tile!.Value)
            .ToList();

        for (var i = 0; i < tiles.Count; i++)
        {
            var mergedTile = 
                i + 1 < tiles.Count &&
                tiles[i].Cashback < MaxRewardCashback &&
                tiles[i].Cashback == tiles[i + 1].Cashback
                ? new Tile(tiles[i++].Cashback * 2)
                : tiles[i];

            merged.Add(mergedTile);
        }

        while (merged.Count < Size)
            merged.Add(null);

        return merged;
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

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                if (column > 0)
                    builder.Append(',');

                builder.Append(_board[row, column]?.Cashback.ToString() ?? ".");
            }

            if (row < Size - 1)
                builder.AppendLine();
        }

        return builder.ToString();
    }

    public static Board FromString(string stringBoard)
    {
        var config = new GameConfig();
        var board = new Board(config.MapSize, config.MaxRewardCashback);

        var rows = stringBoard.Split("\r\n");

        for (var row = 0; row < board.Size; row++)
        {
            var cells = rows[row].Split(',');

            for (var col = 0; col < board.Size; col++)
                if (cells[col] != ".")
                    board[row, col] = new Tile(int.Parse(cells[col]));
        }

        return board;
    }
}
