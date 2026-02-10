using Avalonia.Styling;
using Microsoft.Extensions.Logging;

namespace Arknights_OperatorsManager.Services;

/// <summary>
/// テーマ管理サービスの実装
/// </summary>
public class ThemeService : IThemeService
{
    private readonly Avalonia.Application _application;
    private readonly ILogger<ThemeService> _logger;

    public string CurrentTheme { get; private set; } = "System";

    public ThemeService(Avalonia.Application application, ILogger<ThemeService> logger)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ApplyTheme(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
            themeName = "System";

        CurrentTheme = themeName;

        try
        {
            var theme = themeName switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                "System" => ThemeVariant.Default,
                _ => ThemeVariant.Default
            };

            _application.RequestedThemeVariant = theme;
            _logger.LogInformation("Theme changed to {Theme}", themeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply theme {Theme}", themeName);
        }
    }
}
