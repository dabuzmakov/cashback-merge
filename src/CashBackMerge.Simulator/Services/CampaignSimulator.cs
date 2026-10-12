using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;
using CashBackMerge.Simulator.Configuration;

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
}
