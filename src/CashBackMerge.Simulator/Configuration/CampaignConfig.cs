using CashBackMerge.Simulator.Strategies;

namespace CashBackMerge.Simulator.Configuration;

public class CampaignConfig
{
    public int PlayerCount { get; init; }
    public IGameStrategy Strategy { get; init; }
    public double MaxRewardAllowedRate { get; init; }
}
