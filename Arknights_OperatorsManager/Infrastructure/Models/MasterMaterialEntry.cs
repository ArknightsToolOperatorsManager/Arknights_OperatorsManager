namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材マスターエントリ（JSON永続化用）
/// </summary>
public class MasterMaterialEntry
{
    /// <summary>素材ID</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>レアリティ（2-6）</summary>
    public int Rarity { get; set; }

    /// <summary>合成レシピ（null=合成不可）</summary>
    public List<RecipeItem>? Recipe { get; set; }
}
