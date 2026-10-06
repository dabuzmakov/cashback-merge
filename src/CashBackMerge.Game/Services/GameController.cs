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

        var isInnerInverted = false;
        var isOuterInnerSwapped = false;

        for (var outer = 0; outer < Config.MapSize; outer++)
        {
            var mergeQueue = new Queue<Tile>(Config.MapSize);

            for (var i = 0; i < Config.MapSize; i++)
            {
                var inner = isInnerInverted ? Config.MapSize - 1 - i : i;

                var row = isOuterInnerSwapped ? inner : outer;
                var col = isOuterInnerSwapped ? outer : inner;

                if (session.Board[row, col] != null)
                    mergeQueue.Enqueue(session.Board[row, col]!.Value);
            }
            
            var pointer = new BufferPointer(Config.MapSize, isInnerInverted);

            while (mergeQueue.Count != 0)
            {
                var currentTile = mergeQueue.Dequeue();

                var index = pointer.Next();
                var row = isOuterInnerSwapped ? index : outer;
                var col = isOuterInnerSwapped ? outer : index;

                if (!mergeQueue.TryPeek(out var tile) || tile.Cashback != currentTile.Cashback)
                {
                    resultBoard[row, col] = new Tile(currentTile.Cashback);
                }
                else
                {
                    mergeQueue.Dequeue();
                    resultBoard[row, col] = new Tile(currentTile.Cashback * 2);
                }
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

        public BufferPointer(int bufferLength, bool inverted)
        {
            _inverted = inverted;
            Value = inverted ? bufferLength - 1 : 0;
        }

        public int Next()
            => _inverted ? Value-- : Value++;
    }
}
