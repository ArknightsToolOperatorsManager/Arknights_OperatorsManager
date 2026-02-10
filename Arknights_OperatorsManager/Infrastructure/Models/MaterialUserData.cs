namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材ユーザーデータ（JSON永続化用）
/// </summary>
public class MaterialUserData
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>所持素材（MaterialId → 数量）</summary>
    public Dictionary<string, int> Materials { get; set; } = new();
}
