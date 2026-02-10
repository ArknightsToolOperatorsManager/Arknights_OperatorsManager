namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材計画エントリ（JSON永続化用）
/// </summary>
public class MaterialPlanEntry
{
    /// <summary>一意ID</summary>
    public Guid Id { get; set; }

    /// <summary>計画名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>有効フラグ</summary>
    public bool IsEnabled { get; set; }

    /// <summary>入手予定素材（MaterialId → 数量）</summary>
    public Dictionary<string, int> PlannedMaterials { get; set; } = new();

    /// <summary>作成日時</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
