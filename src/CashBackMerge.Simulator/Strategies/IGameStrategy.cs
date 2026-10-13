using CashBackMerge.Game.Models;

namespace CashBackMerge.Simulator.Strategies;

public interface IGameStrategy
{
    Direction GetNextMove(GameSession session);
}
