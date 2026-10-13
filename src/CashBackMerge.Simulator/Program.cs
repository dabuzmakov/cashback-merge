using CashBackMerge.Game.Configuration;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Services;

namespace CashBackMerge.Simulator;

public class Program
{
    private const string _configDirectory = "JsonConfigurations";
    private const string _reportDirectory = "MarkdownReports";

    public static void Main(string[] args)
    {
        var launchSettings = MainArgsParser.Parse(args);

        var configPath = Path.Combine(AppContext.BaseDirectory, _configDirectory);
        var configBuilder = new ConfigurationBuilder().SetBasePath(configPath);

        var reportPath = Path.Combine(AppContext.BaseDirectory, _reportDirectory);
        var reportBuilder = new MarkdownReportBuilder().SetBasePath(reportPath);

        var gameConfig = configBuilder.Get<GameConfig>(launchSettings.GameConfigName);
        var campaignConfig = configBuilder.Get<CampaignConfig>(launchSettings.CampaignConfigName);

        var simulator = new CampaignSimulator(campaignConfig, gameConfig, launchSettings.Seed);
        var statistics = simulator.Run();

        reportBuilder.Build(statistics, launchSettings.OutputFileName);
    }
}