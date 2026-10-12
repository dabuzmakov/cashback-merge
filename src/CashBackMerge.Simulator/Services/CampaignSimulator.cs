using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Models;

namespace CashBackMerge.Simulator.Services;

public class CampaignSimulator
{
    private readonly CampaignConfig _campaignConfig;
    private readonly GameController _controller;

    public CampaignSimulator(
        CampaignConfig campaignConfig, 
        GameConfig gameConfig,
        int seed)
    {
        _campaignConfig = campaignConfig;
        _controller = new GameController(gameConfig, seed);
    }

    public CampaignStatistics Run()
    {
        var totalScore = 0;
        var maxScore = 0;
        var totalReward = 0;

    Dictionary< int, (int Players, int Rate) > RewardDistribution
    }
}
