using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Services;

namespace CashBackMerge.Game.Models;

public class GameSession
{
    private readonly Board _board;
    private readonly Random _random;
    private readonly WeightedRandom _spawnRandom;
    private readonly GameConfig _config;

    public int UsedMoves { get; private set; }
    public int Score { get; private set; }
    public GameStatus Status { get; private set; }

    public int Reward => _board.GetMaxTileValue();
    public string StringBoard => _board.ToString();
    public int EmptyTilesCount => _board.GetEmptyPositions().Count;

    public GameSession(GameConfig config, int seed)
    {
        _config = config;
        _random = new Random(seed);
        _spawnRandom = new WeightedRandom(_config.SpawnRules.ToList(), _random);

        _board = InitializeBoard(
            _config.DeterminedStringState, 
            _config.InitialState, 
            _config.MapSize, 
            _config.MaxRewardCashback);

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
        var empty = GetRandomPosition(_board);
        _board[empty.Row, empty.Col] = spawned;

        if (UsedMoves >= _config.MovesLimit || !_board.HasAvailableMove())
            Status = GameStatus.End;
    }

    private Board InitializeBoard(
        string? stringState, 
        IReadOnlyList<int> initialState, 
        int size, int maxReward)
    {
        if (stringState != null)
            return Board.FromString(stringState, size, maxReward);

        var board = new Board(size, maxReward);

        foreach (var cashback in initialState)
        {
            var empty = GetRandomPosition(board);
            board[empty.Row, empty.Col] = new Tile(cashback);
        }

        return board;
    }

    private (int Row, int Col) GetRandomPosition(Board board)
    {
        var emptyCells = board.GetEmptyPositions();

        if (emptyCells.Count == 0)
            throw new InvalidOperationException("На доске нет свободных позиций");

        return emptyCells[_random.Next(emptyCells.Count)];
    }
}
