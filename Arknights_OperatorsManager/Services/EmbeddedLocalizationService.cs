using llyrtkframework.Results;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.Json;

namespace Arknights_OperatorsManager.Services;

/// <summary>
/// 埋め込みリソースから多言語データを読み込むローカライゼーションサービス
/// 言語の追加は Resources/Localization/{culture}.json を追加するだけで対応可能
/// </summary>
public class EmbeddedLocalizationService : llyrtkframework.Localization.ILocalizationService
{
    private readonly ILogger<EmbeddedLocalizationService> _logger;
    private readonly Dictionary<string, Dictionary<string, object>> _resources = new();
    private readonly List<CultureInfo> _availableCultures = new();
    private readonly BehaviorSubject<CultureInfo> _cultureChanged;
    private CultureInfo _currentCulture;

    private static readonly string[] SupportedCultures = { "ja-JP", "en-US", "zh-CN" };
    private const string DefaultCulture = "ja-JP";
    private const string ResourcePrefix = "Arknights_OperatorsManager.Resources.Localization.";

    public EmbeddedLocalizationService(ILogger<EmbeddedLocalizationService> logger)
    {
        _logger = logger;
        _currentCulture = CultureInfo.GetCultureInfo(DefaultCulture);
        _cultureChanged = new BehaviorSubject<CultureInfo>(_currentCulture);

        LoadAllResources();
    }

    public CultureInfo CurrentCulture => _currentCulture;

    public IObservable<CultureInfo> CultureChanged => _cultureChanged;

    public Result SetCulture(CultureInfo culture)
    {
        if (!_availableCultures.Any(c => c.Name == culture.Name))
        {
            _logger.LogWarning("Culture {Culture} is not available, falling back to {Default}", culture.Name, DefaultCulture);
            culture = CultureInfo.GetCultureInfo(DefaultCulture);
        }

        _currentCulture = culture;
        _cultureChanged.OnNext(culture);
        _logger.LogInformation("Culture changed to {Culture}", culture.Name);

        return Result.Success();
    }

    public Result SetCulture(string cultureName)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            return SetCulture(culture);
        }
        catch (CultureNotFoundException ex)
        {
            _logger.LogError(ex, "Invalid culture name: {CultureName}", cultureName);
            return Result.Failure($"Invalid culture name: {cultureName}");
        }
    }

    public Result<string> GetString(string key)
    {
        var value = GetLocalizedValue(key);
        if (value == null)
        {
            _logger.LogWarning("Localization key not found: {Key}", key);
            return Result<string>.Success(key);
        }

        return Result<string>.Success(value);
    }

    public Result<string> GetString(string key, params object[] args)
    {
        var result = GetString(key);
        if (result.IsFailure)
            return result;

        try
        {
            var formatted = string.Format(result.Value!, args);
            return Result<string>.Success(formatted);
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Failed to format localized string for key: {Key}", key);
            return Result<string>.Success(result.Value!);
        }
    }

    public bool ContainsKey(string key)
    {
        return GetLocalizedValue(key) != null;
    }

    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        return _availableCultures.AsReadOnly();
    }

    private void LoadAllResources()
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var cultureName in SupportedCultures)
        {
            var resourceName = $"{ResourcePrefix}{cultureName}.json";

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogWarning("Resource not found: {ResourceName}", resourceName);
                    continue;
                }

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data != null)
                {
                    _resources[cultureName] = data;
                    _availableCultures.Add(CultureInfo.GetCultureInfo(cultureName));
                    _logger.LogDebug("Loaded localization resource: {Culture}", cultureName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load localization resource: {ResourceName}", resourceName);
            }
        }

        _logger.LogInformation("Loaded {Count} localization resources", _availableCultures.Count);
    }

    private string? GetLocalizedValue(string key)
    {
        if (!_resources.TryGetValue(_currentCulture.Name, out var cultureResources))
        {
            if (!_resources.TryGetValue(DefaultCulture, out cultureResources))
                return null;
        }

        var parts = key.Split('.');
        object? current = cultureResources;

        foreach (var part in parts)
        {
            if (current is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Object && jsonElement.TryGetProperty(part, out var prop))
                {
                    current = prop;
                }
                else
                {
                    return null;
                }
            }
            else if (current is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(part, out var value))
                {
                    current = value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        if (current is JsonElement element && element.ValueKind == JsonValueKind.String)
        {
            return element.GetString();
        }

        return current?.ToString();
    }
}
