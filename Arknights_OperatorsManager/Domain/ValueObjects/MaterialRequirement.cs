using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// 必要素材（素材IDと必要数量のマッピング）
/// </summary>
public sealed class MaterialRequirement : ValueObject
{
    /// <summary>
    /// 素材IDと必要数量の辞書
    /// </summary>
    public IReadOnlyDictionary<string, int> Materials { get; }

    public MaterialRequirement(IReadOnlyDictionary<string, int> materials)
    {
        Materials = materials ?? throw new ArgumentNullException(nameof(materials));
    }

    /// <summary>
    /// 空の必要素材を生成
    /// </summary>
    public static MaterialRequirement Empty => new(new Dictionary<string, int>());

    /// <summary>
    /// 所持素材から不足分を計算
    /// </summary>
    /// <param name="inventory">所持素材（素材ID → 所持数量）</param>
    /// <returns>不足素材（素材ID → 不足数量）</returns>
    public MaterialRequirement CalculateShortage(IReadOnlyDictionary<string, int> inventory)
    {
        ArgumentNullException.ThrowIfNull(inventory);

        var shortage = new Dictionary<string, int>();

        foreach (var (materialId, requiredAmount) in Materials)
        {
            var ownedAmount = inventory.TryGetValue(materialId, out var amount) ? amount : 0;
            var shortageAmount = requiredAmount - ownedAmount;

            if (shortageAmount > 0)
            {
                shortage[materialId] = shortageAmount;
            }
        }

        return new MaterialRequirement(shortage);
    }

    /// <summary>
    /// 別の MaterialRequirement と合算する
    /// </summary>
    public MaterialRequirement Add(MaterialRequirement other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var combined = new Dictionary<string, int>(Materials);

        foreach (var (materialId, amount) in other.Materials)
        {
            if (combined.ContainsKey(materialId))
            {
                combined[materialId] += amount;
            }
            else
            {
                combined[materialId] = amount;
            }
        }

        return new MaterialRequirement(combined);
    }

    /// <summary>
    /// 特定の素材の必要数を取得
    /// </summary>
    public int GetRequiredAmount(string materialId)
    {
        return Materials.TryGetValue(materialId, out var amount) ? amount : 0;
    }

    /// <summary>
    /// 必要素材があるかどうか
    /// </summary>
    public bool IsEmpty => Materials.Count == 0 || Materials.Values.All(v => v == 0);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        foreach (var kvp in Materials.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}
