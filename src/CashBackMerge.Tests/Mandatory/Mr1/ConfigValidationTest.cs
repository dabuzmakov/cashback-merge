using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: конфиг кампании.</summary>
public class ConfigValidationTest
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

    [Fact(DisplayName = "Валидный конфиг принимается")]
    public void ValidConfigIsAccepted()
        => GameConfigValidator.Validate(_config);

    [Fact(DisplayName = "Сумма вероятностей spawn-правил не равна 1 — конфиг отклоняется")]
    public void ProbabilitiesNotSummingToOneAreRejected()
    {
        var config = _config with 
        {
            SpawnRules =
            [
                new ProbabilityRule(1, 0.86m),
                new ProbabilityRule(2, 0.10m),
                new ProbabilityRule(4, 0.05m)
            ]
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }

    [Fact(DisplayName = "movesLimit <= 0 — конфиг отклоняется")]
    public void NonPositiveMovesLimitIsRejected()
    {
        var config = _config with 
        { 
            MovesLimit = -2 
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }

    [Fact(DisplayName = "Пустой список spawn-правил — конфиг отклоняется")]
    public void EmptySpawnRulesAreRejected()
    {
        var config = _config with
        {
            SpawnRules = []
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }
}