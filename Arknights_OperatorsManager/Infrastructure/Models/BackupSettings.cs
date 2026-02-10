namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// バックアップ設定（JSON永続化用）
/// </summary>
public class BackupSettings
{
    /// <summary>バックアップ有効フラグ</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>保持する最大バックアップ数</summary>
    public int MaxBackups { get; set; } = 10;

    /// <summary>バックアップ間隔（分）</summary>
    public int IntervalMinutes { get; set; } = 30;
}
