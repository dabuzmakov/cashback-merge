using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;
using System.Data;

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
    public Board BoardSnapshot => _board.Clone();

    public GameSession(
        GameConfig config, 
        int seed, 
        Board? initialBoard = null)
    {
        GameConfigValidator.Validate(config);

        _config = config;
        _random = new Random(seed);
        _spawnRandom = new WeightedRandom(_config.SpawnRules, _random);
        _board = InitializeBoard(initialBoard);

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

    private Board InitializeBoard(Board? initialBoard = null)
    {
        if (initialBoard == null)
        {
            var initialTiles = _config.InitialState.Select(x => new Tile(x));
            var board = new Board(_config.MapSize, _config.MaxRewardCashback)
                .WithRandomPlaced(initialTiles, _random);

            return board;
        }
            
        if (initialBoard.Size != _config.MapSize ||
            initialBoard.MaxRewardCashback != _config.MaxRewardCashback)
            throw new ArgumentException(
                "Состояние доски initialBoard не согласовано с игровым конфигом");

        return initialBoard.Clone();
    }
}
