using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Game.Services;

public class WeightedRandom
{
    private const int _probabilityScale = 100;
    private readonly List<(int Value, int Weight)> _weightedRules;
    private readonly Random _random;

    public WeightedRandom(IReadOnlyList<ProbabilityRule> rules, Random random)
    {
        _random = random;

        _weightedRules = rules
            .Select(rule => (rule.Value, (int)(rule.Probability * _probabilityScale)))
            .ToList();
    }

    public int Next()
    {
        var weight = _random.Next(_probabilityScale);   
        var cumulativeWeight = 0;

        foreach (var rule in _weightedRules)
        {
            cumulativeWeight += rule.Weight;

            if (weight < cumulativeWeight)
                return rule.Value;
        }

        throw new InvalidOperationException(
            "Не удалось выбрать значение: сумма весов не соответствует диапазонам");
    }
}
