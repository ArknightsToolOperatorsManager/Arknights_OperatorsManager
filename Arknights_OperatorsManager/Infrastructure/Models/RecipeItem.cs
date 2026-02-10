namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 合成レシピ要素（JSON永続化用）
/// </summary>
public class RecipeItem
{
    /// <summary>素材ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>必要数量</summary>
    public int Count { get; set; }
}
