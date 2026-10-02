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
}
