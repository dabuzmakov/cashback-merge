using System.Text.Json;

namespace CashBackMerge.Game.Configuration;

public class GameConfig
{
    public int MapSize { get; init; } = 4;

    public int MovesLimit { get; init; } = 40;

    public int MaxRewardCashback { get; init; } = 32;

    public IReadOnlyList<int> InitialState { get; init; } = [1, 1];

    public IReadOnlyList<SpawnRule> SpawnRules { get; init; } = 
    [
        new SpawnRule { Cashback = 1, Probability = 0.85m },
        new SpawnRule { Cashback = 2, Probability = 0.10m },
        new SpawnRule { Cashback = 4, Probability = 0.05m }
    ];

    public static GameConfig FromJson(string path)
    {
        using var stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<GameConfig>(stream, options)
           ?? throw new InvalidOperationException("Ошибка десереализации игрового конфига");
    }
}