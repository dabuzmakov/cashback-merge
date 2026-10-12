using CashBackMerge.Simulator.Models;

namespace CashBackMerge.Simulator.Services;

public static class MainArgsParser
{
    public static LaunchSettings Parse(string[] args)
    {
        string? gameConfig = null;
        string? campaignConfig = null;
        int? seed = null;
        string? output = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (i + 1 >= args.Length || args[i + 1].StartsWith("--"))
                throw new ArgumentException(
                    $"Не указано значение для аргумента {args[i]}");

            switch (args[i])
            {
                case "--game":
                    gameConfig = args[++i];
                    break;

                case "--campaign":
                    campaignConfig = args[++i];
                    break;

                case "--seed":
                    if (!int.TryParse(args[++i], out var parsedSeed))
                        throw new ArgumentException("Некорректное значение seed");

                    seed = parsedSeed;
                    break;

                case "--output":
                    output = args[++i];
                    break;

                default:
                    throw new ArgumentException($"Неизвестный аргумент: {args[i]}");
            }
        }

        if (string.IsNullOrWhiteSpace(gameConfig))
            throw new ArgumentException("Не указан --game");

        if (string.IsNullOrWhiteSpace(campaignConfig))
            throw new ArgumentException("Не указан --campaign");

        return new LaunchSettings(
            gameConfig,
            campaignConfig,
            seed ?? new Random().Next(),
            output ?? "report.md"
        );
    }
}
