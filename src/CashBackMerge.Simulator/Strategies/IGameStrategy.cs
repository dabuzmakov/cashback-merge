using CashBackMerge.Game.Models;
using CashBackMerge.Game.Sessions;

namespace CashBackMerge.Simulator.Strategies;

public interface IGameStrategy
{
    Direction GetNextMove(GameSession session);
}
