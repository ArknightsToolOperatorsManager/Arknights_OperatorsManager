using Arknights_OperatorsManager.Application.State;
using Arknights_OperatorsManager.Infrastructure.Models;
using Arknights_OperatorsManager.Infrastructure.Persistence;
using llyrtkframework.StateManagement;
using Microsoft.Extensions.Logging;

namespace Arknights_OperatorsManager.Services;

/// <summary>
/// IStateStoreの状態をAppSettingsに同期するサービス
/// </summary>
public class StateToSettingsSyncService
{
    private readonly IStateStore _stateStore;
    private readonly AppSettingsFileManager _settingsManager;
    private readonly ILogger<StateToSettingsSyncService> _logger;

    public StateToSettingsSyncService(
        IStateStore stateStore,
        AppSettingsFileManager settingsManager,
        ILogger<StateToSettingsSyncService> logger)
    {
        _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 現在の状態をAppSettingsに保存
    /// </summary>
    public async Task SaveStateToSettingsAsync()
    {
        try
        {
            var stateResult = _stateStore.GetState<AppGlobalState>(StateKeys.AppGlobalState);
            if (stateResult.IsFailure)
            {
                _logger.LogWarning("Failed to get state for saving: {Error}", stateResult.ErrorMessage);
                return;
            }

            var state = stateResult.Value!;

            // 現在の設定を取得（なければデフォルト値）
            var settingsResult = await _settingsManager.LoadAsync();
            var settings = settingsResult.IsSuccess && settingsResult.Value != null
                ? settingsResult.Value
                : new AppSettings();

            // 状態を設定に反映
            settings.Theme = state.CurrentTheme;
            settings.Language = state.CurrentLanguage;
            settings.UI.ShowTips = state.ShowTips;
            settings.UI.UseMaterialPlanFeature = state.UseMaterialPlanFeature;
            settings.UI.ShowChinaServerData = state.ShowChinaServerData;

            // キャッシュを更新して保存
            _settingsManager.UpdateCachedData(settings);
            await _settingsManager.AutoSaveAsync();

            _logger.LogInformation("State saved to AppSettings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save state to AppSettings");
        }
    }
}
