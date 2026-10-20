using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;

using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Tests.Mandatory.Mr3;

/// <summary>Обязательные тесты: награды, симуляция и отчёт.</summary>
public class RewardAndSimulationTest
{
    private readonly GameConfig _gameConfig = new GameConfig(
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

    private readonly CampaignConfig _campaignConfig = new CampaignConfig(
        PlayersCount: 10000,
        MaxRewardAllowedRate: 0.15m,
        StrategyName: "circle"
    );

    [Fact(DisplayName = "Максимальная плитка 16% даёт награду соответствующего tier")]
    public void RewardMatchesHighestTile()
    {
        var maxTileOnBoard = 16;

        var config = _gameConfig with
        {
            DeterminedStringState =
            $"{maxTileOnBoard},.,.,.\r\n" +
            ".,.,.,.\r\n" +
            ".,.,.,.\r\n" +
            ".,.,.,."
        };

        var controller = new GameFactory(config, seed: 67);
        var session = controller.CreateNewGame();

        Assert.Equal(maxTileOnBoard, session.Reward);
    }

    [Fact(DisplayName = "Weighted random учитывает вероятности из конфига")]
    public void SpawnRespectsConfiguredProbabilities()
    {
        var rules = _gameConfig.SpawnRules.ToList();
        var random = new WeightedRandom(rules, new Random(Seed: 67));
        var counts = rules.ToDictionary(rule => rule.Value, _ => 0);

        var iterations = 100000;

        for (var i = 0; i < iterations; i++)
            counts[random.Next()]++;

        foreach (var rule in rules)
        {
            var actualProbability = (decimal)counts[rule.Value] / iterations;

            Assert.InRange(
                Math.Abs(actualProbability - rule.Probability),
                0m,
                0.01m);
        }
    }

    [Fact(DisplayName = "Отчёт содержит распределение максимальных плиток и наград")]
    public void ReportContainsDistributions()
    {
        var report = MakeFailedReport();

        Assert.Contains("- Распределение наград:", report);
        Assert.Contains("    8: 324 (3,24%)", report);
        Assert.Contains("    16: 8039 (80,39%)", report);
        Assert.Contains("    32: 1637 (16,37%)", report);
    }

    [Fact(DisplayName = "Отчёт содержит долю игроков с максимальной наградой")]
    public void ReportContainsMaxRewardRate()
    {
        var report = MakeFailedReport();

        Assert.Contains("- Доля игроков с максимальной наградой: 16,37%", report);
    }

    [Fact(DisplayName = "Статус OK, если actualMaxRewardRate <= maxRewardAllowedRate, иначе FAILED")]
    public void ReportStatusReflectsBusinessConstraint()
    {
        var failedReport = MakeFailedReport();
        var successedReport = MakeSuccessedReport();

        Assert.Contains("## Статус: Failed", failedReport);
        Assert.Contains("## Статус: ОК", successedReport);
    }

    private string MakeFailedReport()
    {
        var simulator = new CampaignSimulator(_campaignConfig, _gameConfig, seed: 67);
        var statistics = simulator.Run();

        return MarkdownReportBuilder.CreateReport(statistics);
    }

    private string MakeSuccessedReport()
    {
        var campaignConfig = _campaignConfig with
        {
            MaxRewardAllowedRate = 1m
        };

        var simulator = new CampaignSimulator(campaignConfig, _gameConfig, seed: 67);
        var statistics = simulator.Run();

        return MarkdownReportBuilder.CreateReport(statistics);
    }
}
