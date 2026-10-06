using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;

namespace CashBackMerge.Game.Services;

public class GameController
{
    public GameConfig Config { get; }
    private readonly Random _sessionSeedRandom;

    public GameController(GameConfig config, int seed)
    {
        _sessionSeedRandom = new Random(seed);
        Config = config;
    }

    public MoveResult ApplyMove(GameSession session, Direction direction)
    {
        var result = new MoveResult();

        if (session.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");



        return result;
    }

    public GameSession CreateNewGame(int? seed = null)
    {
        seed ??= _sessionSeedRandom.Next();

        return new GameSession(Config, (int)seed);
    }
}
