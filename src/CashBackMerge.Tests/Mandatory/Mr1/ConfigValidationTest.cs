using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: конфиг кампании.</summary>
public class ConfigValidationTest
{
    [Fact(DisplayName = "Валидный конфиг принимается")]
    public void ValidConfigIsAccepted()
    {
        var config = new GameConfig();
        GameConfigValidator.Validate(config);
    }

    [Fact(DisplayName = "Сумма вероятностей spawn-правил не равна 1 — конфиг отклоняется")]
    public void ProbabilitiesNotSummingToOneAreRejected()
    {
        var config = new GameConfig()
        { 
            SpawnRules = 
            [
                new SpawnRule(1, 0.85m),
                new SpawnRule(2, 0.0999999999999999m),
                new SpawnRule(4, 0.05m),
            ] 
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }

    [Fact(DisplayName = "movesLimit <= 0 — конфиг отклоняется")]
    public void NonPositiveMovesLimitIsRejected()
    {
        var config = new GameConfig() 
        { 
            MovesLimit = -2 
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }

    [Fact(DisplayName = "Пустой список spawn-правил — конфиг отклоняется")]
    public void EmptySpawnRulesAreRejected()
    {
        var config = new GameConfig()
        {
            SpawnRules = []
        };

        Assert.Throws<ArgumentException>(
            () => GameConfigValidator.Validate(config));
    }
}