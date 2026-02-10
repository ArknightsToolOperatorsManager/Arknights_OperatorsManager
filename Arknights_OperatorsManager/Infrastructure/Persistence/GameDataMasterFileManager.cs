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
/// ゲームデータマスターのFileManager（GitHub同期対応）
/// </summary>
public class GameDataMasterFileManager : FileManagerBase<GameDataMaster>
{
    private const string FileName = "game_data_master.json";
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ArknightsOperatorsManager",
        "Data");

    private readonly EventAggregator? _eventAggregator;

    public GameDataMasterFileManager(
        ILogger<GameDataMasterFileManager> logger,
        EventAggregator? eventAggregator = null)
        : base(new JsonFileSerializer<GameDataMaster>(), logger, eventAggregator)
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

        // OnStartupトリガー（起動時にバックアップ）
        triggers.Add(new OnStartupBackupTrigger(delay: TimeSpan.FromSeconds(5)));

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
                "game_data_master"),
            MaxBackupCount = 5,
            BackupFilePattern = "{filename}_{timestamp}.bak"
        };
    }

    protected override GitHubFileOptions? ConfigureGitHubOptions()
    {
        // GitHub同期を有効化
        // TODO: 実際のリポジトリ情報に置き換える必要があります
        return new GitHubFileOptions
        {
            Owner = "YOUR_GITHUB_USERNAME",
            Repository = "arknights-operators-manager-data",
            Branch = "main",
            FilePath = "data/game_data_master.json",
            Token = null, // パブリックリポジトリの場合はnull
            PollingInterval = TimeSpan.FromMinutes(30),
            CacheDuration = TimeSpan.FromDays(1)
        };
    }

    protected override bool ConfigureAutoSaveEnabled()
    {
        // マスターデータは自動保存を有効化（キャッシュのため）
        return true;
    }
}
