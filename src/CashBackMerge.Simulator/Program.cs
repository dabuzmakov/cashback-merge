using CashBackMerge.Game.Configuration;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Simulator;

public class Program
{
    public static void Main(string[] args)
    {
        var launchSettings = MainArgsParser.Parse(args);

        var gameConfig = GameConfig.FromJson(launchSettings.GameConfigName);
        var campaignConfig = CampaignConfig.FromJson(launchSettings.CampaignConfigName);

    }
}