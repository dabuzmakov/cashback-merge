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
        var resultBoard = new Board(Config.MapSize);

        if (session.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");

        //left direction
        for (var row = 0; row < Config.MapSize; row++)
        {
            var pointer = 0;

            for (var col = 0; col < Config.MapSize; col++)
            {
                var cell = session.Board[row, col];

                if (session.Board[row, col] == null)
                    continue;

                if (col == Config.MapSize - 1 || 
                    session.Board[row, col + 1] != session.Board[row, col])
                {
                    
                }
            }
        }

        //right direction
        for (var row = 0; row < Config.MapSize; row++)
        {
            for (var col = Config.MapSize - 1; col >= 0; col--)
            {

            }
        }

        //top direction
        for (var col = 0; col < Config.MapSize; col++)
        {
            for (var row = 0; row < Config.MapSize; row++)
            {

            }
        }

        //bottom direction
        for (var col = 0; col < Config.MapSize; col++)
        {
            for (var row = Config.MapSize - 1; row >= 0; row--)
            {

            }
        }

        return new MoveResult 
        { 
            Board = resultBoard, 
            SpawnedTile = new Tile(1) 
        };
    }

    public GameSession CreateNewGame(int? seed = null)
    {
        seed ??= _sessionSeedRandom.Next();

        return new GameSession(Config, (int)seed);
    }
}
