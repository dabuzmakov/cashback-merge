using CashBackMerge.Game.Models;
using CashBackMerge.Game.Sessions;

namespace CashBackMerge.Simulator.Strategies;

public class RandomStrategy : IGameStrategy
{
    private readonly Random _random;

    public RandomStrategy(int seed)
    {
        _random = new Random(seed);
    }

    public Direction GetNextMove(GameSession session)
        => (Direction)_random.Next(0, 4);
}
