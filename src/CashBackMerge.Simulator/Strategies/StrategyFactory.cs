namespace CashBackMerge.Simulator.Strategies;

public static class StrategyFactory
{
    private static readonly IReadOnlyDictionary<string, Func<int, IGameStrategy>> _registry =
         new Dictionary<string, Func<int, IGameStrategy>>(StringComparer.OrdinalIgnoreCase)
         {
             ["random"] = seed => new RandomStrategy(seed),
             ["circle"] = _ => new CircleStrategy(),
             ["greedy-score"] = _ => new GreedyScoreStrategy(),
         };

    public static bool Contains(string name)
        => _registry.ContainsKey(name);

    public static IGameStrategy Create(string name, int seed)
        => _registry.TryGetValue(name, out var factory)
            ? factory(seed)
            : throw new ArgumentException($"Неизвестная стратегия: {name}");
}
