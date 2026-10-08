using System.Text.Json;

namespace CashBackMerge.Game.Configuration;

public class GameConfig
{
    public int MapSize { get; init; } = 4;

    public int MovesLimit { get; init; } = 40;

    public int MaxRewardCashback { get; init; } = 32;

    public IReadOnlyList<int> InitialState { get; init; } = [1, 1];

    //MR2 mandatory tests
    public string? DeterminedStringState { get; init; } = null;

    public IReadOnlyList<ProbabilityRule> SpawnRules { get; init; } = 
    [
        new ProbabilityRule { Value = 1, Probability = 0.85m },
        new ProbabilityRule { Value = 2, Probability = 0.1m },
        new ProbabilityRule { Value = 4, Probability = 0.05m }
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