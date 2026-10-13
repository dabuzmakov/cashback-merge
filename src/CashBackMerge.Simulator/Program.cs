using CashBackMerge.Game.Configuration;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Simulator;

public class Program
{
    private const string _configDirectory = "JsonConfigurations";

    public static void Main(string[] args)
    {
        var launchSettings = MainArgsParser.Parse(args);

        var configPath = Path.Combine(AppContext.BaseDirectory, _configDirectory);
        var configBuilder = new ConfigurationBuilder().SetBasePath(configPath);

        var gameConfig = configBuilder.Get<GameConfig>(launchSettings.GameConfigName);
        var campaignConfig = configBuilder.Get<CampaignConfig>(launchSettings.CampaignConfigName);

        var simulator = new CampaignSimulator(campaignConfig, gameConfig, launchSettings.Seed);
        var reportBuilder = new MarkdownReportBuilder(launchSettings.OutputFileName);

        var statistics = simulator.Run();
        reportBuilder.Build(statistics);
    }
}