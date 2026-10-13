namespace CashBackMerge.Simulator.Strategies;

public static class StrategyMapper
{
    private static readonly Dictionary<string, Func<IGameStrategy>> Strategies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["random"] = () => new RandomStrategy(),
            ["circle"] = () => new CircleStrategy()
        };

    public static bool TryGetValue(string name, out IGameStrategy strategy)
    {
        strategy = Strategies.TryGetValue(name, out var gameStrategy) 
            ? gameStrategy()
            : null!;

        return strategy != null;
    }

    public static IGameStrategy Get(string name)
        => Strategies[name]();
}
