using CashBackMerge.Simulator.Strategies;

namespace CashBackMerge.Simulator.Configuration;

public static class CampaignConfigValidator
{
    public static void Validate(CampaignConfig config)
    {
        if (config.MaxRewardAllowedRate < 0 || config.MaxRewardAllowedRate > 1)
            throw new ArgumentException(
                $"Неверная конфигурация MaxRewardAllowedRate: требуется значение от 0 до 1");

        if (config.PlayersCount <= 0)
            throw new ArgumentException(
                $"Неверная конфигурация PlayersCount: количество игроков должно быть положительным целым числом");

        if (!StrategyFactory.Contains(config.StrategyName))
            throw new ArgumentException(
                $"Неверная конфигурация Strategy: стратегии {config.StrategyName} не существует");
    }
}
