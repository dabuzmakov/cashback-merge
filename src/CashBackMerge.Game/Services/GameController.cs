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

        //left-right direction with flag invertedCol
        var invertedCol = false;
        for (var row = 0; row < Config.MapSize; row++)
        {
            var mergeQueue = new Queue<Tile>(Config.MapSize);

            for (var i = 0; i < Config.MapSize; i++)
            {
                var col = invertedCol ? Config.MapSize - 1 - i : i;

                if (session.Board[row, col] != null)
                    mergeQueue.Enqueue(session.Board[row, col]!.Value);
            }

            var pointer = 0;
            while (mergeQueue.Count != 0)
            {
                var currentTile = mergeQueue.Dequeue();

                if (!mergeQueue.TryPeek(out var tile) || tile.Cashback != currentTile.Cashback)
                    resultBoard[row, pointer++] = new Tile(currentTile.Cashback);
                else
                {
                    mergeQueue.Dequeue();
                    resultBoard[row, pointer++] = new Tile(currentTile.Cashback * 2);
                }
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

    private class BufferPointer
    {
        private readonly bool _inverted;
        public int Value { get; private set; }

        public BufferPointer(int initial, bool inverted)
        {
            _inverted = inverted;
            Value = initial;
        }

        public int Next()
        {
            Value += _inverted ? -1 : 1;
            return Value;
        }
    }
}
