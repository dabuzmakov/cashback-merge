using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Extensions;

namespace CashBackMerge.Game.Services;

public class WeightedRandom
{
    private readonly Random _random;
    private readonly List<(int Value, long Weight)> _normalizedRules;
    private readonly long _totalWeight;

    public WeightedRandom(List<ProbabilityRule> rules, Random random)
    {
        _random = random;
        _normalizedRules = rules.ToWeightedValues();
        _totalWeight = _normalizedRules.Sum(rule => rule.Weight);
    }

    public int Next()
    {
        var weight = _random.NextInt64(_totalWeight);

        long cumulativeWeight = 0;

        foreach (var rule in _normalizedRules)
        {
            cumulativeWeight += rule.Weight;

            if (weight < cumulativeWeight)
                return rule.Value;
        }

        throw new InvalidOperationException(
            "Не удалось выбрать значение: сумма весов не соответствует диапазонам");
    }
}
