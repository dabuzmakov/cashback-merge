using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;
using CashBackMerge.Game.Services;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: движок ходов — что считается ходом и что после него происходит.</summary>
public class MoveEngineTest
{
    [Fact(DisplayName = "Невалидный ход не меняет поле, не тратит ход и не создаёт плитку")]
    public void InvalidMoveChangesNothing()
    {
        var config = new GameConfig
        {
            DeterminedStringState =
               "1,2,4,8\r\n" +
               "2,4,8,1\r\n" +
               "4,8,1,2\r\n" +
               "8,1,2,4"
        };

        var seed = Random.Shared.Next();
        var controller = new GameController(config, seed);
        var session = controller.CreateNewGame();

        var before = session.Board.ToString();

        session.ApplyMove(Direction.Left);

        Assert.Equal(before, session.Board.ToString());
        Assert.Equal(0, session.UsedMoves);
    }

    [Fact(DisplayName = "После валидного хода появляется ровно одна новая плитка")]
    public void ValidMoveSpawnsExactlyOneTile()
    {
        var config = new GameConfig
        {
            DeterminedStringState =
                "1,1,2,4\r\n" +
                ".,.,.,.\r\n" +
                ".,.,.,.\r\n" +
                ".,.,.,."
        };

        var seed = Random.Shared.Next();
        var controller = new GameController(config, seed);
        var session = controller.CreateNewGame();

        var emptyCount = session.Board.GetEmptyPositions().Count;
        session.ApplyMove(Direction.Left);
        var newEmptyCount = session.Board.GetEmptyPositions().Count;

        Assert.Equal(emptyCount, newEmptyCount);
    }

    [Fact(DisplayName = "После N валидных ходов игра завершена")]
    public void GameEndsAfterMovesLimit()
    {
        var config = new GameConfig
        {
            MovesLimit = 2,
            DeterminedStringState =
                "1,1,2,.\r\n" +
                "1,1,2,.\r\n" +
                "1,1,2,.\r\n" +
                "1,1,2,."
        };

        var seed = Random.Shared.Next();
        var controller = new GameController(config, seed);
        var session = controller.CreateNewGame();

        session.ApplyMove(Direction.Left);
        session.ApplyMove(Direction.Left);

        Assert.Equal(2, session.UsedMoves);
        Assert.Equal(GameStatus.End, session.Status);
    }

    [Fact(DisplayName = "Достижение плитки 32% не заканчивает игру — доигрываем до лимита ходов")]
    public void ReachingMaxTileDoesNotEndGame()
    {
        var config = new GameConfig
        {
            MovesLimit = 2,
            MaxRewardCashback = 32,
            DeterminedStringState =
                "16,16,2,4\r\n" +
                ".,.,.,.\r\n" +
                ".,.,.,.\r\n" +
                ".,.,.,."
        };

        var seed = Random.Shared.Next();
        var controller = new GameController(config, seed);
        var session = controller.CreateNewGame();

        session.ApplyMove(Direction.Left);

        Assert.Equal(GameStatus.InProgress, session.Status);

        session.ApplyMove(Direction.Right);

        Assert.Equal(2, session.UsedMoves);
        Assert.Equal(GameStatus.End, session.Status);
    }

    [Fact(DisplayName = "Score увеличивается с каждым ходом на значение новой плитки")]
    public void ScoreIsCalculatedOnMerges()
    {
        var config = new GameConfig
        {
            DeterminedStringState =
            "1,1,.,.\r\n" +
            ".,.,.,.\r\n" +
            ".,.,.,.\r\n" +
            ".,.,.,."
        };

        var seed = Random.Shared.Next();
        var controller = new GameController(config, seed);
        var session = controller.CreateNewGame();

        var initialScore = session.Score;

        session.ApplyMove(Direction.Left);

        var difference = session.Score - initialScore;

        Assert.Contains(difference, config.SpawnRules.Select(rule => rule.Value));
        Assert.Equal(1, session.UsedMoves);
    }
}