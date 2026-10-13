using CashBackMerge.Game.Configuration;
using CashBackMerge.Simulator.Configuration;

namespace CashBackMerge.Simulator.Models;

public record CampaignStatistics(
    GameConfig GameConfig,
    CampaignConfig CampaignConfig,
    decimal AverageScore,
    int MaxScore,
    decimal AverageReward,
    Dictionary<int, int> RewardDistribution)
{
}
