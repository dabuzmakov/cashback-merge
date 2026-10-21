using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;
using CashBackMerge.Game.Services;

using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Models;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Tests.Mandatory.Mr3;

/// <summary>Обязательные тесты: награды, симуляция и отчёт.</summary>
public class RewardAndSimulationTest
{
    private static readonly GameConfig _gameConfig = new GameConfig(
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

    private static readonly CampaignConfig _campaignConfig = new CampaignConfig(
        MaxRewardAllowedRate: 0.15m,
        StrategyName: "random",
        PlayersCount: 10000
    );

    private static readonly CampaignStatistics _statistics = new CampaignStatistics(
        GameConfig: _gameConfig,
        CampaignConfig: _campaignConfig,
        AverageScore: 100m,
        MaxScore: 200,
        AverageReward: 17.6m,
        RewardDistribution: new Dictionary<int, int>
        {
            [8] = 2000,
            [16] = 6000,
            [32] = 2000
        }
    );

    private static readonly string _markdownReport = 
        MarkdownReportBuilder.CreateReport(_statistics);

    [Fact(DisplayName = "Максимальная плитка 16% даёт награду соответствующего tier")]
    public void RewardMatchesHighestTile()
    {
        var maxTileOnBoard = 16;

        var board = new Board(_gameConfig.MapSize, _gameConfig.MaxRewardCashback)
            .WithFixedState(new int[,]
            {
                { 16, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            });

        var session = new GameSession(_gameConfig, seed: 67, board);
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
        var report = MarkdownReportBuilder.CreateReport(_statistics);

        Assert.Contains("- Распределение наград:", report);
        Assert.Contains("    8: 2000 (20,00%)", report);
        Assert.Contains("    16: 6000 (60,00%)", report);
        Assert.Contains("    32: 2000 (20,00%)", report);
    }

    [Fact(DisplayName = "Отчёт содержит долю игроков с максимальной наградой")]
    public void ReportContainsMaxRewardRate()
    {
        var rate = _statistics.MaxRewardRate * 100;
        Assert.Contains($"- Доля игроков с максимальной наградой: {rate:F2}%", _markdownReport);
    }

    [Fact(DisplayName = "Статус OK, если actualMaxRewardRate <= maxRewardAllowedRate, иначе FAILED")]
    public void ReportStatusReflectsBusinessConstraint()
    {
        var status = _statistics.Status;
        Assert.Contains($"## Статус: {status}", _markdownReport);
    }
}
