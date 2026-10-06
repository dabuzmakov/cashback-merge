using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;
using CashBackMerge.Game.Services;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: воспроизводимость по seed.</summary>
public class SeedTest
{
    [Fact(DisplayName = "Одинаковый конфиг и seed дают одинаковое начальное поле")]
    public void SameConfigAndSeedProduceSameInitialBoard()
    {
        var config = new GameConfig();
        var seed = 67;
        var controller = new GameController(config, seed);

        var session1 = controller.CreateNewGame(seed);
        var session2 = controller.CreateNewGame(seed);

        Assert.Equal(
            session1.Board.ToString(),
            session2.Board.ToString());
    }

    [Fact(DisplayName = "Одинаковый конфиг, seed и стратегия дают одинаковый отчёт симуляции",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void SameInputsProduceSameSimulationReport()
    {
        Assert.Fail("Тест не реализован");
    }
}