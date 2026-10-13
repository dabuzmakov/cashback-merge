using CashBackMerge.Game.Configuration;
using System.Text.Json;

namespace CashBackMerge.Simulator.Configuration;

public class ConfigurationBuilder
{
    private string _basePath;

    public ConfigurationBuilder SetBasePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _basePath = path;
        return this;
    }

    public TConfig Get<TConfig>(string fileName)
        where TConfig : IConfig
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var path = Path.Combine(_basePath, fileName);

        using var stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<TConfig>(stream, options)
           ?? throw new InvalidOperationException("Ошибка десереализации конфига симуляции");
    }
}
