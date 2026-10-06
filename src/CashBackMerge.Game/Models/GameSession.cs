using CashBackMerge.Game.Configuration;

namespace CashBackMerge.Game.Models;

public class GameSession
{
    public Random SpawnRandom { get; }
    public GameConfig Config { get; }
    public Board Board { get; private set; }
    public int UsedMoves { get; private set; }
    public GameStatus Status { get; private set; }
    public int Score { get; private set; }

    internal GameSession(GameConfig config, int seed)
    {
        Board = new Board(config.MapSize);
        SpawnRandom = new Random(seed);
        Config = config;

        Status = GameStatus.InProgress;
        Score = config.InitialState.Sum();

        InitializeBoard();
    }

    public void RegisterMove(MoveResult result)
    {
        Board = result.Board;
        UsedMoves++;
        Score += result.SpawnedTile.Cashback;
    }

    private void InitializeBoard()
    {
        foreach (var cashback in Config.InitialState)
        {
            var index = SpawnRandom.Next(Board.Size * Board.Size);

            while (Board[index / Board.Size, index % Board.Size] != null)
                index = SpawnRandom.Next(Board.Size * Board.Size);

            Board[index / Board.Size, index % Board.Size] = new Tile(cashback);
        }
    }
}
