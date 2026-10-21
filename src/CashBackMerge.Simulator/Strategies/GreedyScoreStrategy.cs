using CashBackMerge.Game.Models;
using CashBackMerge.Game.Sessions;

namespace CashBackMerge.Simulator.Strategies;

public class GreedyScoreStrategy : IGameStrategy
{
    public Direction GetNextMove(GameSession session)
    {
        var snapshot = session.BoardSnapshot;

        Direction? bestDirection = null;
        var bestScore = -1;

        foreach (var direction in Enum.GetValues<Direction>())
        {
            var board = snapshot.Clone();
            var result = board.TryMove(direction);

            if (!result.Changed)
                continue;

            if (result.ScoreGained > bestScore)
            {
                bestScore = result.ScoreGained;
                bestDirection = direction;
            }
        }

        return bestDirection!.Value;
    }
}