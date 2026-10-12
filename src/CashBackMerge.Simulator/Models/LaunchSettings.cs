namespace CashBackMerge.Simulator.Models;

public record LaunchSettings(
    string GameConfigName, 
    string CampaignConfigName, 
    int Seed, 
    string OutputFileName) 
{ 

}
