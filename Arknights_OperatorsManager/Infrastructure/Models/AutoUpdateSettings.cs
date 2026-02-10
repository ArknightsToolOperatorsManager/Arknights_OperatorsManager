namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 自動更新設定（JSON永続化用）
/// </summary>
public class AutoUpdateSettings
{
    /// <summary>自動更新有効フラグ</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>起動時に更新チェックを行うか</summary>
    public bool CheckOnStartup { get; set; } = true;
}
