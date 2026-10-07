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
        if (session.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Завершённая сессия не принимает попытки");

        return new MoveResult
        {
            Board = UpdateBoard(session.Board, direction),
            SpawnedTile = new Tile(1)
        };
    }

    public GameSession CreateNewGame(int? seed = null)
    {
        seed ??= _sessionSeedRandom.Next();

        return new GameSession(Config, (int)seed);
    }

    private Board UpdateBoard(Board board, Direction direction)
    {
        var resultBoard = new Board(Config.MapSize);

        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var outer = 0; outer < Config.MapSize; outer++)
        {
            var queue = CreateMergeQueue(board, direction, outer);
            var merged = MergeLine(queue);

            var pointer = new BufferPointer(Config.MapSize, inverted);

            foreach (var tile in merged)
            {
                var slidingIndex = pointer.Next();
                var row = swapped ? slidingIndex : outer;
                var col = swapped ? outer : slidingIndex;

                resultBoard[row, col] = tile;
            }
        }

        return resultBoard;
    }

    private Queue<Tile> CreateMergeQueue(Board board, Direction direction, int fixedIndex)
    {
        var mergeQueue = new Queue<Tile>(Config.MapSize);

        var swapped = direction is Direction.Up or Direction.Down;
        var inverted = direction is Direction.Right or Direction.Down;

        for (var i = 0; i < Config.MapSize; i++)
        {
            var slidingIndex = inverted ? Config.MapSize - 1 - i : i;

            var row = swapped ? slidingIndex : fixedIndex;
            var col = swapped ? fixedIndex : slidingIndex;

            if (board[row, col] != null)
                mergeQueue.Enqueue(board[row, col]!.Value);
        }

        return mergeQueue;
    }

    private List<Tile> MergeLine(Queue<Tile> mergeQueue)
    {
        var merged = new List<Tile>();

        while (mergeQueue.Count != 0)
        {
            var currentTile = mergeQueue.Dequeue();

            if (!mergeQueue.TryPeek(out var tile) || tile.Cashback != currentTile.Cashback)
            {
                merged.Add(new Tile(currentTile.Cashback));
            }
            else
            {
                mergeQueue.Dequeue();
                merged.Add(new Tile(currentTile.Cashback * 2));
            }
        }

        return merged;
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
