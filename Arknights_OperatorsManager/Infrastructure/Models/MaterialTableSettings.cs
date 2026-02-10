namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材テーブル設定（JSON永続化用）
/// </summary>
public class MaterialTableSettings
{
    /// <summary>ソート列ID</summary>
    public string? SortColumn { get; set; }

    /// <summary>降順フラグ</summary>
    public bool SortDescending { get; set; }
}
