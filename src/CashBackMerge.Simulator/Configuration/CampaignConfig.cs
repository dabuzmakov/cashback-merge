using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Simulator.Configuration;

public record CampaignConfig(
    decimal MaxRewardAllowedRate,
    string StrategyName,
    int PlayersCount) : IConfig
{
}
