using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;
using CashBackMerge.Game.Services;
using CashBackMerge.Game.Sessions;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: движок ходов — что считается ходом и что после него происходит.</summary>
public class MoveEngineTest
{
    private static readonly GameConfig _config = new GameConfig(
        MapSize: 4,
        MovesLimit: 40,
        MaxRewardCashback: 32,
        InitialState: [1, 1],
        SpawnRules:
        [
            new ProbabilityRule(1, 0.85m),
            new ProbabilityRule(2, 0.10m),
            new ProbabilityRule(4, 0.05m)
        ]
    );

    private static readonly Board _defaultBoard = 
        new Board(_config.MapSize, _config.MaxRewardCashback);

    [Fact(DisplayName = "Невалидный ход не меняет поле, не тратит ход и не создаёт плитку")]
    public void InvalidMoveChangesNothing()
    {
        var board = _defaultBoard.WithFixedState(new int[,]
        {
            { 1, 2, 4, 8 },
            { 2, 4, 8, 1 },
            { 4, 8, 1, 2 },
            { 8, 1, 2, 4 }
        });

        var session = new GameSession(_config, seed: 67, board);
        var before = session.BoardSnapshot;

        session.ApplyMove(Direction.Left);

        Assert.Equal(before, session.BoardSnapshot);
        Assert.Equal(0, session.UsedMoves);
    }

    [Fact(DisplayName = "После валидного хода появляется ровно одна новая плитка")]
    public void ValidMoveSpawnsExactlyOneTile()
    {
        var board = _defaultBoard.WithFixedState(new int[,]
        {
            { 1, 1, 2, 4 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });

        var session = new GameSession(_config, seed: 67, board);
        var emptyCount = session.BoardSnapshot
            .GetEmptyPositions().Count();

        session.ApplyMove(Direction.Left);

        var newEmptyCount = session.BoardSnapshot
            .GetEmptyPositions().Count();

        Assert.Equal(1, session.UsedMoves);
        Assert.Equal(emptyCount, newEmptyCount);
    }

    [Fact(DisplayName = "После N валидных ходов игра завершена")]
    public void GameEndsAfterMovesLimit()
    {
        var config = _config with { MovesLimit = 2 };

        var board = _defaultBoard.WithFixedState(new int[,]
        {
            { 1, 1, 2, 0 },
            { 1, 1, 2, 0 },
            { 1, 1, 2, 0 },
            { 1, 1, 2, 0 }
        });

        var session = new GameSession(config, seed: 67, board);

        session.ApplyMove(Direction.Left);
        session.ApplyMove(Direction.Left);

        Assert.Equal(2, session.UsedMoves);
        Assert.Equal(GameStatus.End, session.Status);
    }

    [Fact(DisplayName = "Достижение плитки 32% не заканчивает игру — доигрываем до лимита ходов")]
    public void ReachingMaxTileDoesNotEndGame()
    {
        var config = _config with
        {
            MovesLimit = 2,
            MaxRewardCashback = 32
        };

        var board = _defaultBoard.WithFixedState(new int[,]
        {
            { 16, 16, 2, 4 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });

        var session = new GameSession(config, seed: 67, board);

        session.ApplyMove(Direction.Left);

        Assert.Equal(GameStatus.InProgress, session.Status);

        session.ApplyMove(Direction.Right);

        Assert.Equal(2, session.UsedMoves);
        Assert.Equal(GameStatus.End, session.Status);
    }

    [Fact(DisplayName = "Score увеличивается на значение объединённой плитки")]
    public void ScoreIsCalculatedOnMerges()
    {
        var board = _defaultBoard.WithFixedState(new int[,]
        {
            { 1, 1, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });

        var session = new GameSession(_config, seed: 67, board);
        var initialScore = session.Score;

        session.ApplyMove(Direction.Left);

        Assert.Equal(2, session.Score - initialScore);
    }
}