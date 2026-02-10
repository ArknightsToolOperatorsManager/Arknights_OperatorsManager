using Arknights_OperatorsManager.Domain.Enums;
using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.Entities;

/// <summary>
/// 素材エンティティ（公式IDで識別）
/// </summary>
public class Material : Entity<string>
{
    /// <summary>名前</summary>
    public string Name { get; private set; }

    /// <summary>カテゴリ</summary>
    public MaterialCategory Category { get; private set; }

    /// <summary>レアリティ（T1~T5など）</summary>
    public int Rarity { get; private set; }

    /// <summary>アイコンパス</summary>
    public string? IconPath { get; private set; }

    public Material(
        string id,
        string name,
        MaterialCategory category,
        int rarity,
        string? iconPath = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Material ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Material name cannot be empty", nameof(name));

        if (rarity < 0)
            throw new ArgumentException("Material rarity cannot be negative", nameof(rarity));

        Name = name;
        Category = category;
        Rarity = rarity;
        IconPath = iconPath;
    }

    /// <summary>
    /// 名前を更新
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Material name cannot be empty", nameof(name));

        Name = name;
    }

    /// <summary>
    /// アイコンパスを更新
    /// </summary>
    public void UpdateIconPath(string? iconPath)
    {
        IconPath = iconPath;
    }
}
