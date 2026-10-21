namespace CashBackMerge.Simulator.Strategies;

public static class StrategyFactory
{
    private static readonly HashSet<string> Strategies = new HashSet<string>
    {
        "random",
        "circle"
    };

    public static bool Contains(string name)
        => Strategies.Contains(name);

    public static IGameStrategy Create(string name, int seed)
        => name.ToLowerInvariant() switch
    {
        "random" => new RandomStrategy(seed),
        "circle" => new CircleStrategy(),
        _ => throw new ArgumentException($"Неизвестная стратегия: {name}")
    };
}
