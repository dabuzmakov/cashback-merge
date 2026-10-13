namespace CashBackMerge.Game.Configuration;

public record GameConfig(
    int MapSize,
    int MovesLimit,
    int MaxRewardCashback,
    IReadOnlyList<int> InitialState,
    IReadOnlyList<ProbabilityRule> SpawnRules,
    string? DeterminedStringState = null) : IConfig
{
}