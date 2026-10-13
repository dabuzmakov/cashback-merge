using CashBackMerge.Game.Models;

namespace CashBackMerge.Simulator.Strategies;

public class CircleStrategy : IGameStrategy
{
    private int _pointer;

    public Direction GetNextMove(GameSession session)
    {
        if (++_pointer > 3) _pointer = 0;
        return (Direction)_pointer;
    }
}
