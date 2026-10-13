using CashBackMerge.Game.Configuration;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Models;
using System.Text;

namespace CashBackMerge.Simulator.Services;

public class MarkdownReportBuilder
{
    private string _basePath = null!;

    public void Build(CampaignStatistics statistics, string outputFileName)
    {
        ValidateMarkdownFormat(outputFileName);

        var path = Path.Combine(_basePath, outputFileName);
        var report = CreateReport(statistics);

        File.WriteAllText(path, report);
    }

    public MarkdownReportBuilder SetBasePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Directory.CreateDirectory(path);

        _basePath = path;
        return this;
    }

    private static string CreateReport(CampaignStatistics statistics)
    {
        var report = new StringBuilder();

        report.AppendLine("# Отчёт симуляции кампании CashbackMerge");
        report.AppendLine();

        AppendCampaignConfig(report, statistics.CampaignConfig);
        AppendGameConfig(report, statistics.GameConfig);
        AppendResults(report, statistics);

        report.AppendLine();
        report.AppendLine($"## Статус: {GetCampaignStatus(statistics)}");

        return report.ToString();
    }

    private static void ValidateMarkdownFormat(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var dotIndex = fileName.LastIndexOf('.');

        if (dotIndex <= 0 || dotIndex == fileName.Length - 1)
            throw new ArgumentException("Некорректное имя файла отчёта");

        var format = fileName[(dotIndex + 1)..];

        if (format != "md")
            throw new ArgumentException("Поддерживается только Markdown формат отчёта");
    }

    private static void AppendCampaignConfig(StringBuilder report, CampaignConfig campaign)
    {
        report.AppendLine("## Параметры кампании");
        report.AppendLine();

        report.AppendLine($"- Количество игроков: {campaign.PlayersCount}");
        report.AppendLine($"- Стратегия выбора хода: {campaign.StrategyName}");
        report.AppendLine($"- Ожидаемая доля максимальных наград: {campaign.MaxRewardAllowedRate}");

        report.AppendLine();
    }

    private static void AppendGameConfig(StringBuilder report, GameConfig game)
    {
        report.AppendLine("## Параметры игры");
        report.AppendLine();

        report.AppendLine($"- Размер карты: {game.MapSize}");
        report.AppendLine($"- Максимальная награда: {game.MaxRewardCashback}");
        report.AppendLine($"- Лимит ходов: {game.MovesLimit}");
        report.AppendLine($"- Начальное состояние поля: {string.Join(",", game.InitialState)}");
        report.AppendLine("- Правила появления:");

        foreach (var rule in game.SpawnRules.OrderBy(x => x.Value))
            report.AppendLine($"    {rule.Value}: {rule.Probability}");

        report.AppendLine();
    }

    private static void AppendResults(StringBuilder report, CampaignStatistics statistics)
    {
        report.AppendLine("## Результаты кампании");
        report.AppendLine();

        report.AppendLine($"- Средний счёт: {statistics.AverageScore}");
        report.AppendLine($"- Максимальный счёт: {statistics.MaxScore}");
        report.AppendLine($"- Средний размер награды: {statistics.AverageReward}");
        report.AppendLine("- Распределение наград:");

        var playersCount = statistics.CampaignConfig.PlayersCount;

        foreach (var (tile, players) in statistics.RewardDistribution.OrderBy(x => x.Key))
        {
            var rate = players / (double)playersCount * 100;
            report.AppendLine($"    {tile}: {players} ({rate:F2}%)");
        }
    }

    private static string GetCampaignStatus(CampaignStatistics statistics)
    {
        var campaign = statistics.CampaignConfig;
        var maxRewardPlayers = statistics.RewardDistribution
            .GetValueOrDefault(statistics.GameConfig.MaxRewardCashback);

        var maxRewardRate = maxRewardPlayers / (decimal)campaign.PlayersCount;

        return maxRewardRate <= campaign.MaxRewardAllowedRate
            ? "ОК"
            : "Failed";
    }
}
