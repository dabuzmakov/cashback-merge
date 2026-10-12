namespace CashBackMerge.Simulator.Strategies;

public static class StrategyMapper
{
    private static readonly Dictionary<string, IGameStrategy> Strategies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["random"] = new RandomStrategy()
        };

    public static bool TryGetValue(string name, out IGameStrategy strategy)
    {
        var exists = Strategies.TryGetValue(name, out var gameStrategy);

        strategy = gameStrategy;
        return exists;
    }
}
