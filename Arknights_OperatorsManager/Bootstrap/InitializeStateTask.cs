using Arknights_OperatorsManager.Application.State;
using Arknights_OperatorsManager.Infrastructure.Persistence;
using llyrtkframework.Application;
using llyrtkframework.Results;
using llyrtkframework.StateManagement;
using Microsoft.Extensions.Logging;

namespace Arknights_OperatorsManager.Bootstrap;

/// <summary>
/// Core Init: AppSettingsからIStateStoreを初期化
/// </summary>
public class InitializeStateTask : ICoreInitTask
{
    private readonly IStateStore _stateStore;
    private readonly AppSettingsFileManager _settingsManager;
    private readonly ILogger<InitializeStateTask> _logger;

    public InitializeStateTask(
        IStateStore stateStore,
        AppSettingsFileManager settingsManager,
        ILogger<InitializeStateTask> logger)
    {
        _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Initializing state from AppSettings...");

            // AppSettingsから設定を読み込み
            var settingsResult = await _settingsManager.LoadAsync();

            AppGlobalState appState;
            if (settingsResult.IsSuccess && settingsResult.Value != null)
            {
                var settings = settingsResult.Value;
                appState = new AppGlobalState
                {
                    CurrentTheme = settings.Theme,
                    CurrentLanguage = settings.Language,
                    ShowTips = settings.UI.ShowTips,
                    UseMaterialPlanFeature = settings.UI.UseMaterialPlanFeature,
                    ShowChinaServerData = settings.UI.ShowChinaServerData
                };
                _logger.LogInformation(
                    "State initialized from AppSettings: Theme={Theme}, Language={Language}",
                    settings.Theme, settings.Language);
            }
            else
            {
                // 設定ファイルが存在しない場合はデフォルト値
                appState = new AppGlobalState();
                _logger.LogInformation("AppSettings not found, using default state");
            }

            // IStateStoreに初期状態を設定
            var setResult = _stateStore.SetState(StateKeys.AppGlobalState, appState);
            if (setResult.IsFailure)
            {
                _logger.LogError("Failed to set initial state: {Error}", setResult.ErrorMessage);
                return setResult;
            }

            _logger.LogInformation("State initialization completed");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize state");
            return Result.FromException(ex, "Failed to initialize state from AppSettings");
        }
    }
}
