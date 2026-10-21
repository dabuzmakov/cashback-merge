namespace CashBackMerge.Simulator.CLI;

public record LaunchSettings(
    string GameConfigName, 
    string CampaignConfigName, 
    int Seed, 
    string OutputFileName) 
{ 

}
