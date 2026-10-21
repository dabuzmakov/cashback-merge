using CashBackMerge.Game.Models;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: правила слияния плиток.</summary>
public class MergeRulesTest
{
    [Theory(DisplayName = "Сдвиг влево: строка до -> после")]
    [MemberData(nameof(RowShiftsLeftData))]
    public void RowShiftsLeftAndMerges(int[,] before, int[,] expected)
        => AssertMove(before, expected, Direction.Left);

    public static TheoryData<int[,], int[,]> RowShiftsLeftData => new()
    {
        {
            new int[,]
            {
                { 1, 1, 0, 0 },
                { 1, 1, 1, 0 },
                { 1, 1, 1, 1 },
                { 2, 2, 4, 4 }
            },
            new int[,]
            {
                { 2, 0, 0, 0 },
                { 2, 1, 0, 0 },
                { 2, 2, 0, 0 },
                { 4, 8, 0, 0 }
            }
        }
    };

    [Theory(DisplayName = "Одна плитка не сливается дважды за ход")]
    [MemberData(nameof(TileDoesNotMergeTwiceData))]
    public void TileDoesNotMergeTwiceInOneMove(int[,] before, int[,] expected)
        => AssertMove(before, expected, Direction.Left);

    public static TheoryData<int[,], int[,]> TileDoesNotMergeTwiceData => new()
    {
        {
            new int[,]
            {
                { 1, 1, 2, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            new int[,]
            {
                { 2, 2, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            }
        }
    };

    [Theory(DisplayName = "Слияние работает во всех четырёх направлениях")]
    [MemberData(nameof(MergeWorksInAllDirectionsData))]
    public void MergeWorksInAllDirections(
        int[,] before, int[,] expected, Direction direction)
        => AssertMove(before, expected, direction);

    public static TheoryData<int[,], int[,], Direction> MergeWorksInAllDirectionsData => new()
    {
        {
            new int[,]
            {
                { 1, 0, 2, 4 },
                { 1, 2, 2, 0 },
                { 4, 2, 0, 8 },
                { 4, 0, 0, 8 }
            },
            new int[,]
            {
                { 2, 4, 4, 4 },
                { 8, 0, 0, 16 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            Direction.Up
        },
        {
            new int[,]
            {
                { 1, 0, 2, 4 },
                { 1, 2, 2, 4 },
                { 4, 2, 0, 8 },
                { 4, 0, 0, 8 }
            },
            new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 2, 0, 0, 8 },
                { 8, 4, 4, 16 }
            },
            Direction.Down
        },
        {
            new int[,]
            {
                { 1, 1, 2, 4 },
                { 2, 2, 0, 4 },
                { 4, 0, 4, 8 },
                { 4, 4, 8, 8 }
            },
            new int[,]
            {
                { 2, 2, 4, 0 },
                { 4, 4, 0, 0 },
                { 8, 8, 0, 0 },
                { 8, 16, 0, 0 }
            },
            Direction.Left
        },
        {
            new int[,]
            {
                { 1, 1, 2, 4 },
                { 2, 2, 0, 4 },
                { 4, 0, 4, 8 },
                { 4, 4, 8, 8 }
            },
            new int[,]
            {
                { 0, 2, 2, 4 },
                { 0, 0, 4, 4 },
                { 0, 0, 8, 8 },
                { 0, 0, 8, 16 }
            },
            Direction.Right
        }
    };

    private static void AssertMove(int[,] before, int[,] expected, Direction direction)
    {
        var defaultBoard = new Board(boardSize: 4, maxRewardCashback: 32);

        var currentBoard = defaultBoard.WithFixedState(before);
        var expectedBoard = defaultBoard.WithFixedState(expected);

        currentBoard.TryMove(direction);

        Assert.Equal(currentBoard, expectedBoard);
    }
}