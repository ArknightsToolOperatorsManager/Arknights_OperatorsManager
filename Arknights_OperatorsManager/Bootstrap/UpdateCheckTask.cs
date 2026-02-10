using llyrtkframework.Application;
using llyrtkframework.Results;
using Serilog;

namespace Arknights_OperatorsManager.Bootstrap;

/// <summary>
/// UI Setup: マスターデータの更新チェック
/// 起動時および24時間ごとに更新を確認
/// </summary>
public class UpdateCheckTask : IUiSetupTask
{
    private readonly string _dataDirectory;
    private readonly TimeSpan _updateCheckInterval = TimeSpan.FromHours(24);

    public UpdateCheckTask(string? dataDirectory = null)
    {
        _dataDirectory = dataDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ArknightsOperatorsManager",
            "Data");
    }

    /// <summary>
    /// 更新が利用可能かどうか
    /// </summary>
    public bool UpdateAvailable { get; private set; }

    /// <summary>
    /// 最後の更新チェック日時
    /// </summary>
    public DateTime? LastCheckTime { get; private set; }

    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var lastCheckPath = Path.Combine(_dataDirectory, ".last_update_check");

            // 最後のチェック時刻を確認
            if (File.Exists(lastCheckPath))
            {
                if (DateTime.TryParse(await File.ReadAllTextAsync(lastCheckPath, cancellationToken), out var lastCheck))
                {
                    LastCheckTime = lastCheck;

                    if (DateTime.Now - lastCheck < _updateCheckInterval)
                    {
                        Log.Debug("Skipping update check (last check: {LastCheck})", lastCheck);
                        return Result.Success();
                    }
                }
            }

            Log.Information("Checking for master data updates...");

            // TODO: 実際のGitHub API呼び出しを実装
            // 現時点ではオフラインモードとして動作
            UpdateAvailable = false;

            // チェック時刻を記録
            LastCheckTime = DateTime.Now;
            await File.WriteAllTextAsync(lastCheckPath, LastCheckTime.Value.ToString("O"), cancellationToken);

            Log.Information("Update check completed. Update available: {UpdateAvailable}", UpdateAvailable);

            return Result.Success();
        }
        catch (Exception ex)
        {
            // 更新チェックの失敗は致命的ではない
            Log.Warning(ex, "Failed to check for updates (offline mode)");
            return Result.Success();
        }
    }
}
