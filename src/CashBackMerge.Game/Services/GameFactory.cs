using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;

namespace CashBackMerge.Game.Services;

public class GameFactory
{
    private readonly GameConfig _config;
    private readonly Random _sessionSeedRandom;

    public GameFactory(GameConfig config, int seed)
    {
        _sessionSeedRandom = new Random(seed);
        _config = config;
    }

    public GameSession CreateNewGame(int? seed = null)
    {
        seed ??= _sessionSeedRandom.Next();

        return new GameSession(_config, (int)seed);
    }
}
