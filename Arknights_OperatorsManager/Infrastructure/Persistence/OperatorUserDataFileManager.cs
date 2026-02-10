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
/// オペレーターユーザーデータのFileManager
/// </summary>
public class OperatorUserDataFileManager : FileManagerBase<OperatorUserData>
{
    private const string FileName = "operator_user_data.json";
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ArknightsOperatorsManager",
        "Data");

    private readonly EventAggregator? _eventAggregator;

    public OperatorUserDataFileManager(
        ILogger<OperatorUserDataFileManager> logger,
        EventAggregator? eventAggregator = null)
        : base(new JsonFileSerializer<OperatorUserData>(), logger, eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    protected override string ConfigureFilePath()
    {
        return Path.Combine(DataDirectory, FileName);
    }

    protected override List<BackupTrigger> ConfigureBackupTriggers()
    {
        var triggers = new List<BackupTrigger>();

        // OnModifiedトリガー（変更後30秒でバックアップ）
        if (_eventAggregator != null)
        {
            triggers.Add(new OnModifiedBackupTrigger(
                _eventAggregator,
                Path.Combine(DataDirectory, FileName),
                debounceTime: TimeSpan.FromSeconds(30)));
        }

        // Intervalトリガー（30分ごと）
        triggers.Add(new IntervalBackupTrigger(
            interval: TimeSpan.FromMinutes(30)));

        return triggers;
    }

    protected override BackupOptions ConfigureBackupOptions()
    {
        return new BackupOptions
        {
            BackupDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ArknightsOperatorsManager",
                "Backups",
                "operator_user_data"),
            MaxBackupCount = 10,
            BackupFilePattern = "{filename}_{timestamp}.bak"
        };
    }

    protected override GitHubFileOptions? ConfigureGitHubOptions()
    {
        // ユーザーデータはGitHub同期しない
        return null;
    }

    protected override bool ConfigureAutoSaveEnabled()
    {
        // 自動保存を有効化
        return true;
    }
}
