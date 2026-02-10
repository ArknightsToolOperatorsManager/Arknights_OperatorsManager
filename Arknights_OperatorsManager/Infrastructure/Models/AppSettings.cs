namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// アプリケーション設定（JSON永続化用）
/// </summary>
public class AppSettings
{
    /// <summary>設定バージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>言語（ja-JP, en-US, zh-CN）</summary>
    public string Language { get; set; } = "ja-JP";

    /// <summary>テーマ（Light, Dark, System）</summary>
    public string Theme { get; set; } = "System";

    /// <summary>サーバー設定</summary>
    public string Server { get; set; } = "global";

    /// <summary>バックアップ設定</summary>
    public BackupSettings Backup { get; set; } = new();

    /// <summary>自動更新設定</summary>
    public AutoUpdateSettings AutoUpdate { get; set; } = new();

    /// <summary>UI設定</summary>
    public UiSettings UI { get; set; } = new();
}
