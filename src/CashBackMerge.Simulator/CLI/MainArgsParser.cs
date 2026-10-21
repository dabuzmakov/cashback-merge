namespace CashBackMerge.Simulator.CLI;

public static class MainArgsParser
{
    public static LaunchSettings Parse(string[] args)
    {
        string? gameConfigName = null;
        string? campaignConfigName = null;
        int? seed = null;
        string? outputFileName = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (i + 1 >= args.Length || args[i + 1].StartsWith("--"))
                throw new ArgumentException(
                    $"Не указано значение для аргумента {args[i]}");

            switch (args[i])
            {
                case "--game":
                    gameConfigName = args[++i];
                    break;

                case "--campaign":
                    campaignConfigName = args[++i];
                    break;

                case "--seed":
                    if (!int.TryParse(args[++i], out var parsedSeed))
                        throw new ArgumentException("Некорректное значение seed");

                    seed = parsedSeed;
                    break;

                case "--output":
                    outputFileName = args[++i];
                    break;

                default:
                    throw new ArgumentException($"Неизвестный аргумент: {args[i]}");
            }
        }

        if (string.IsNullOrWhiteSpace(gameConfigName))
            throw new ArgumentException("Не указан --game");

        if (string.IsNullOrWhiteSpace(campaignConfigName))
            throw new ArgumentException("Не указан --campaign");

        return new LaunchSettings(
            gameConfigName,
            campaignConfigName,
            seed ?? new Random().Next(),
            outputFileName ?? "report.md"
        );
    }
}
