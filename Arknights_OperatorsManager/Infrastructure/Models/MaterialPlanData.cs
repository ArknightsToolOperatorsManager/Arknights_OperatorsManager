namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材計画データ（JSON永続化用）
/// </summary>
public class MaterialPlanData
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>素材計画データ</summary>
    public List<MaterialPlanEntry> Plans { get; set; } = new();
}
