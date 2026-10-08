using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Game.Extensions;

public static class ProbabilityExtensions
{
    private const int _maxProbabilityScale = 18;

    public static List<(int Value, long Weight)> ToWeightedValues(this List<ProbabilityRule> rules)
    {
        var maxScale = rules.Max(rule =>
            (decimal.GetBits(rule.Probability)[3] >> 16) & 0xFF);

        if (maxScale > _maxProbabilityScale)
            throw new ArgumentException($"Точность вероятностей слишком высокая: " +
                $"{maxScale} > {_maxProbabilityScale}");

        var modifier = (long)Math.Pow(10, maxScale);

        return rules
            .Select(rule => (rule.Value, (long)(rule.Probability * modifier)))
            .ToList();
    }
}