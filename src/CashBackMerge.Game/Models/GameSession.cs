using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;

namespace CashBackMerge.Game.Models;

public class GameSession
{
    public GameConfig Config { get; }

    public Board Board { get; private set; }
    public int UsedMoves { get; private set; }
    public GameStatus Status { get; private set; }
    public int Score { get; private set; }

    private Random _random;
    private WeightedRandom _spawnRandom;

    internal GameSession(GameConfig config, int seed)
    {
        Config = config;
        _random = new Random(seed);
        _spawnRandom = new WeightedRandom(Config.SpawnRules.ToList(), _random);

        Status = GameStatus.InProgress;
        Score = config.InitialState.Sum();

        InitializeBoard();
    }

    public void ApplyMove(Direction direction)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");

        if (!Board.TryMove(direction))
            return;

        var spawned = new Tile(_spawnRandom.Next());
        var empty = GetRandomPosition();
        Board[empty.Row, empty.Col] = spawned;

        UsedMoves++;
        Score += spawned.Cashback;

        if (UsedMoves >= Config.MovesLimit || !Board.HasAvailableMove())
            Status = GameStatus.End;
    }

    private void InitializeBoard()
    {
        Board = new Board(Config.MapSize, Config.MaxRewardCashback);

        foreach (var cashback in Config.InitialState)
        {
            var empty = GetRandomPosition();
            Board[empty.Row, empty.Col] = new Tile(cashback);
        }
    }

    private (int Row, int Col) GetRandomPosition()
    {
        var emptyCells = Board.GetEmptyPositions();

        if (emptyCells.Count == 0)
            throw new InvalidOperationException("На доске нет свободных позиций");

        return emptyCells[_random.Next(emptyCells.Count)];
    }
}
