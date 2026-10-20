using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;

namespace CashBackMerge.Game.Models;

public class GameSession
{
    private readonly Board _board;
    private readonly Random _random;
    private readonly WeightedRandom _spawnRandom;
    private readonly GameConfig _config;

    public GameStatus Status { get; private set; }
    public int UsedMoves { get; private set; }
    public int Score { get; private set; }
    public int Reward => _board.GetMaxTileValue();

    public GameSession(GameConfig config, int seed)
    {
        GameConfigValidator.Validate(config);

        _config = config;
        _random = new Random(seed);
        _spawnRandom = new WeightedRandom(_config.SpawnRules, _random);

        var initialTiles = _config.InitialState.Select(x => new Tile(x));
        _board = new Board(_config.MapSize, _config.MaxRewardCashback)
            .WithRandomPlaced(initialTiles, _random);

        Status = GameStatus.InProgress;
        (Score, UsedMoves) = (0, 0);
    }

    public void ApplyMove(Direction direction)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");
        
        var moveResult = _board.TryMove(direction);

        if (!moveResult.Changed) return;

        UsedMoves++;
        Score += moveResult.ScoreGained;

        var spawned = new Tile(_spawnRandom.Next());
        _board.TryPlaceRandom(spawned, _random);

        if (UsedMoves >= _config.MovesLimit || !_board.HasAvailableMove())
            Status = GameStatus.End;
    }
}
