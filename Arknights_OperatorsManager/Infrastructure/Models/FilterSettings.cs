namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// フィルター設定（JSON永続化用）
/// </summary>
public class FilterSettings
{
    /// <summary>前回のフィルター状態（JSON文字列として保存）</summary>
    public string? LastFilterJson { get; set; }
}
