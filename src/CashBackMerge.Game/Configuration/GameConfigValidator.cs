namespace CashBackMerge.Game.Configuration;

public static class GameConfigValidator
{
    public static void Validate(GameConfig config)
    {
        if (config.MapSize < 2)
            throw new ArgumentException(
                $"Неверная конфигурация MapSize: минимальный размер поля: 2x2");

        if (config.MovesLimit < 1)
            throw new ArgumentException(
                "Неверная конфигурация MovesLimit: минимальное количество ходов: 1");

        if (!IsPowerOfTwo(config.MaxRewardCashback))
            throw new ArgumentException(
                "Неверная конфигурация MaxRewardCashback: значение максимальной плитки должно " +
                "быть степенью двойки");

        if (config.InitialState.Count == 0)
            throw new ArgumentException(
                "Неверная конфигурация InitialState: поле не может быть пустым");

        if (config.InitialState.Count > config.MapSize * config.MapSize)
            throw new ArgumentException(
                "Неверная конфигурация InitialState: начальное состояние поля не может содержать " +
                $"более MapSize^2 плиток - {config.MapSize * config.MapSize}");

        if (config.InitialState.Any(slab => !IsPowerOfTwo(slab)))
            throw new ArgumentException(
                "Неверная конфигурация InitialState: значение каждой плитки на поле должно " +
                "быть степенью двойки");

        if (config.InitialState.Any(slab => slab > config.MaxRewardCashback))
            throw new ArgumentException(
                $"Неверная конфигурация InitialState: значение плитки не может превосходить " +
                $"MaxRewardCashback: {config.MaxRewardCashback}");

        if (config.SpawnRules.Count == 0)
            throw new ArgumentException(
                "Неверная конфигурация SpawnRules: правила появления плиток не могут быть пустыми");

        if (config.SpawnRules.Any(rule => !IsPowerOfTwo(rule.Cashback)))
            throw new ArgumentException(
                "Неверная конфигурация SpawnRules: значение каждой появившейся плитки на поле " +
                "должно быть степенью двойки");

        if (config.SpawnRules.Any(rule => rule.Cashback > config.MaxRewardCashback))
            throw new ArgumentException(
                $"Неверная конфигурация SpawnRules: каждая новая плитка по значению не может " +
                $"превосходить MaxRewardCashback: {config.MaxRewardCashback}");

        if (config.SpawnRules.GroupBy(rule => rule.Cashback).Any(group => group.Count() > 1))
            throw new ArgumentException(
                "Неверная конфигурация SpawnRules: значения Cashback не должны повторяться");

        if (config.SpawnRules.Sum(rule => rule.Probability) != 1.0m)
            throw new ArgumentException(
                "Неверная конфигурация SpawnRules: сумма вероятностей появления плиток должна быть равна 1" +
                $" Текущая сумма {config.SpawnRules.Sum(rule => rule.Probability)}");
    }

    private static bool IsPowerOfTwo(int n)
        => n > 0 && (n & (n - 1)) == 0;
}
