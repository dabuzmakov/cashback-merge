using CashBackMerge.Game.Configuration;
using System.Numerics;

namespace CashBackMerge.Game.Extensions;

public static class ProbabilityExtensions
{
    public static List<(int Value, Int128 Weight)> ToWeightedValues(this List<ProbabilityRule> rules)
    {
        var maxScale = rules.Max(rule =>
            (decimal.GetBits(rule.Probability)[3] >> 16) & 0xFF);

        decimal modifier = 1;

        for (var i = 0; i < maxScale; i++)
            modifier *= 10;

        return rules
            .Select(rule => (rule.Value, (Int128)(rule.Probability * modifier)))
            .ToList();
    }
}