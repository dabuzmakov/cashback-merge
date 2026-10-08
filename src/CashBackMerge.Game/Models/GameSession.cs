using CashBackMerge.Game.Configuration;
using System.Drawing;

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
        Config = config;
        SpawnRandom = new Random(seed);
        Status = GameStatus.InProgress;
        Score = config.InitialState.Sum();

        Board = InitializeBoard();
    }

    public void ApplyMove(Direction direction)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");

        if (!Board.TryMove(direction))
            return;

        UsedMoves++;
        //Score += spawned.CashBack;

        if (UsedMoves == Config.MovesLimit || !Board.HasAvailableMove())
            Status = GameStatus.End;
    }

    private Board InitializeBoard()
    {
        var board = new Board(Config.MapSize, Config.MaxRewardCashback);

        foreach (var cashback in Config.InitialState)
        {
            var index = SpawnRandom.Next(board.Size * board.Size);

            while (board[index / board.Size, index % board.Size] != null)
                index = SpawnRandom.Next(board.Size * board.Size);

            board[index / board.Size, index % board.Size] = new Tile(cashback);
        }

        return board;
    }
}
