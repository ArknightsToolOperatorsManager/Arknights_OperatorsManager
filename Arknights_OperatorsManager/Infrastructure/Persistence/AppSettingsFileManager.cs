using Arknights_OperatorsManager.Infrastructure.Models;
using llyrtkframework.FileManagement.Backup;
using llyrtkframework.FileManagement.Core;
using llyrtkframework.FileManagement.GitHub;
using llyrtkframework.FileManagement.Serializers;
using llyrtkframework.FileManagement.Triggers;
using Microsoft.Extensions.Logging;
using EventAggregator = llyrtkframework.Events.IEventAggregator;

namespace Arknights_OperatorsManager.Infrastructure.Persistence;

/// <summary>
/// アプリ設定のFileManager
/// </summary>
public class AppSettingsFileManager : FileManagerBase<AppSettings>
{
    private const string FileName = "settings.json";
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ArknightsOperatorsManager",
        "Data");

    public AppSettingsFileManager(
        ILogger<AppSettingsFileManager> logger,
        EventAggregator? eventAggregator = null)
        : base(new JsonFileSerializer<AppSettings>(), logger, eventAggregator)
    {
    }

    protected override string ConfigureFilePath()
    {
        return Path.Combine(DataDirectory, FileName);
    }

    protected override List<BackupTrigger> ConfigureBackupTriggers()
    {
        // 設定ファイルはバックアップトリガーなし
        return new List<BackupTrigger>();
    }

    protected override BackupOptions ConfigureBackupOptions()
    {
        // バックアップトリガーはないが、念のためオプションは設定
        return new BackupOptions
        {
            BackupDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ArknightsOperatorsManager",
                "Backups",
                "settings"),
            MaxBackupCount = 3,
            BackupFilePattern = "{filename}_{timestamp}.bak"
        };
    }

    protected override GitHubFileOptions? ConfigureGitHubOptions()
    {
        // 設定ファイルはGitHub同期しない
        return null;
    }

    protected override bool ConfigureAutoSaveEnabled()
    {
        // 自動保存を有効化
        return true;
    }
}
