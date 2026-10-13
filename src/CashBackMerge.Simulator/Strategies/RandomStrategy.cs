using CashBackMerge.Game.Models;

namespace CashBackMerge.Simulator.Strategies;

public class RandomStrategy : IGameStrategy
{
    private readonly Random _random = new Random();

    public Direction GetNextMove(GameSession session)
        => (Direction)_random.Next(0, 4);
}
