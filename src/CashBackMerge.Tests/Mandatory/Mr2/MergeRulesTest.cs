using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: правила слияния плиток.</summary>
public class MergeRulesTest
{
    [Theory(DisplayName = "Сдвиг влево: строка до -> после")]
    [InlineData(
        "1,1,.,.\r\n1,1,1,.\r\n1,1,1,1\r\n2,2,4,4",
        "2,.,.,.\r\n2,1,.,.\r\n2,2,.,.\r\n4,8,.,.")]
    public void RowShiftsLeftAndMerges(string before, string expected)
        => AssertMove(before, expected, Direction.Left);

    [Theory(DisplayName = "Одна плитка не сливается дважды за ход: [1,1,2,.] -> [2,2,.,.]")]
    [InlineData(
        "1,1,2,.\r\n.,.,.,.\r\n.,.,.,.\r\n.,.,.,.",
        "2,2,.,.\r\n.,.,.,.\r\n.,.,.,.\r\n.,.,.,.")]
    public void TileDoesNotMergeTwiceInOneMove(string before, string expected)
        => AssertMove(before, expected, Direction.Left);

    [Theory(DisplayName = "Слияние работает во всех четырёх направлениях")]
    [InlineData(
        "1,.,2,4\r\n1,2,2,.\r\n4,2,.,8\r\n4,.,.,8", 
        "2,4,4,4\r\n8,.,.,16\r\n.,.,.,.\r\n.,.,.,.", Direction.Up)]
    [InlineData(
        "1,.,2,4\r\n1,2,2,4\r\n4,2,.,8\r\n4,.,.,8", 
        ".,.,.,.\r\n.,.,.,.\r\n2,.,.,8\r\n8,4,4,16", Direction.Down)]
    [InlineData(
        "1,1,2,4\r\n2,2,.,4\r\n4,.,4,8\r\n4,4,8,8", 
        "2,2,4,.\r\n4,4,.,.\r\n8,8,.,.\r\n8,16,.,.", Direction.Left)]
    [InlineData(
        "1,1,2,4\r\n2,2,.,4\r\n4,.,4,8\r\n4,4,8,8", 
        ".,2,2,4\r\n.,.,4,4\r\n.,.,8,8\r\n.,.,8,16", Direction.Right)]
    public void MergeWorksInAllDirections(string before, string expected, Direction direction)
        => AssertMove(before, expected, direction);

    private static void AssertMove(string before, string expected, Direction direction)
    {
        var board = Board.FromString(before);

        board.TryMove(direction);

        Assert.Equal(expected, board.ToString());
    }
}