using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Game.Models;

public class GameSession
{
    public Random SpawnRandom { get; }
    public GameConfig Config { get; }

    public Board Board { get; init; }
    public int UsedMoves { get; private set; }
    public GameStatus Status { get; private set; }
    public int Score { get; private set; }

    internal GameSession(GameConfig config, int seed)
    {
        Board = InitializeBoard();
        SpawnRandom = new Random(seed);
        Config = config;

        Status = GameStatus.InProgress;
        Score = config.InitialState.Sum();
    }

    public void ApplyMove(Direction direction)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");

        Board.TryMove(direction);
        UsedMoves++;
        //Score += spawned.CashBack;

        if (UsedMoves == Config.MovesLimit)
            Status = GameStatus.End;
    }

    private Board InitializeBoard()
    {
        var board = new Board(Config.MapSize, Config.MaxRewardCashback);

        foreach (var cashback in Config.InitialState)
        {
            var index = SpawnRandom.Next(Board.Size * Board.Size);

            while (Board[index / Board.Size, index % Board.Size] != null)
                index = SpawnRandom.Next(Board.Size * Board.Size);

            Board[index / Board.Size, index % Board.Size] = new Tile(cashback);
        }

        return board;
    }
}
