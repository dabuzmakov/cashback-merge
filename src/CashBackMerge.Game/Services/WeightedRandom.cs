using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Extensions;

namespace CashBackMerge.Game.Services;

public class WeightedRandom
{
    private readonly Random _random;
    private readonly List<(int Value, Int128 Weight)> _normalizedRules;

    public WeightedRandom(List<ProbabilityRule> rules, Random random)
    {
        _random = random;
        _normalizedRules = rules.ToWeightedValues();
    }
}
