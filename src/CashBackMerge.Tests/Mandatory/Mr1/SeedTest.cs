using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Models;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: воспроизводимость по seed.</summary>
public class SeedTest
{
    private readonly GameConfig _config = new GameConfig(
        MapSize: 4,
        MovesLimit: 40,
        MaxRewardCashback: 32,
        InitialState: [1, 1],
        SpawnRules:
        [
            new ProbabilityRule(1, 0.85m),
            new ProbabilityRule(2, 0.10m),
            new ProbabilityRule(4, 0.05m)
        ]
    );

    [Fact(DisplayName = "Одинаковый конфиг и seed дают одинаковое начальное поле")]
    public void SameConfigAndSeedProduceSameInitialBoard()
    {
        var seed = 67;
        var controller = new GameController(_config, seed);

        var session1 = controller.CreateNewGame(seed);
        var session2 = controller.CreateNewGame(seed);

        Assert.Equal(session1.StringBoard, session2.StringBoard);
    }

    [Fact(DisplayName = "Одинаковый конфиг, seed и стратегия дают одинаковый отчёт симуляции")]
    public void SameInputsProduceSameSimulationReport()
    {
        var seed = 67;

        var campaignConfig = new CampaignConfig(
            PlayersCount: 10000,
            MaxRewardAllowedRate: 0.15m,
            StrategyName: "circle");

        var simulator = new CampaignSimulator(campaignConfig, _config, seed);

        var statistics1 = simulator.Run();
        var statistics2 = simulator.Run();

        var report1 = MarkdownReportBuilder.CreateReport(statistics1);
        var report2 = MarkdownReportBuilder.CreateReport(statistics2);

        Assert.Equal(report1, report2);
    }
}