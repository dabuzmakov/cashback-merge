using CashBackMerge.Simulator.Models;

namespace CashBackMerge.Simulator.Services;

public class MarkdownReportBuilder
{
    private readonly string _outputFileName;

    public MarkdownReportBuilder(string outputFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFileName);

        ValidateMarkdownFormat(outputFileName);
        _outputFileName = outputFileName;
    }

    public void Build(CampaignStatistics statistics)
    {

    }

    private static void ValidateMarkdownFormat(string name)
    {
        var dotIndex = name.LastIndexOf('.');

        if (dotIndex <= 0 || dotIndex == name.Length - 1)
            throw new ArgumentException("Некорректное имя файла отчёта");

        var format = name[(dotIndex + 1)..];

        if (format != "md")
            throw new ArgumentException(
                "Поддерживается только Markdown формат отчёта");
    }
}
