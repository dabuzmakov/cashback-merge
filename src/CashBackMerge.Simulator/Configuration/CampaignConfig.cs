using System.Text.Json;

namespace CashBackMerge.Simulator.Configuration;

public class CampaignConfig
{
    public int PlayersCount { get; init; } = 10000;
    public string Strategy { get; init; } = "random";
    public decimal MaxRewardAllowedRate { get; init; } = 0.45m;

    public static CampaignConfig FromJson(string path)
    {
        using var stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<CampaignConfig>(stream, options)
           ?? throw new InvalidOperationException("Ошибка десереализации конфига симуляции");
    }
}
