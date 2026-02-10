using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.Entities;

/// <summary>
/// 素材計画エンティティ（将来入手予定の素材シミュレーション用）
/// GUIDで識別
/// </summary>
public class MaterialPlan : GuidEntity
{
    /// <summary>計画名</summary>
    public string Name { get; private set; }

    /// <summary>有効フラグ</summary>
    public bool IsEnabled { get; private set; }

    /// <summary>入手予定素材（MaterialId → 数量）</summary>
    public Dictionary<string, int> PlannedMaterials { get; private set; }

    /// <summary>作成日時</summary>
    public DateTime CreatedAt { get; private set; }

    public MaterialPlan(
        Guid id,
        string name,
        bool isEnabled,
        Dictionary<string, int> plannedMaterials,
        DateTime createdAt) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plan name cannot be empty", nameof(name));

        Name = name;
        IsEnabled = isEnabled;
        PlannedMaterials = plannedMaterials ?? new Dictionary<string, int>();
        CreatedAt = createdAt;
    }

    /// <summary>
    /// 新規作成用のファクトリメソッド
    /// </summary>
    public static MaterialPlan Create(string name, bool isEnabled = true)
    {
        return new MaterialPlan(
            Guid.NewGuid(),
            name,
            isEnabled,
            new Dictionary<string, int>(),
            DateTime.UtcNow
        );
    }

    /// <summary>
    /// 計画を有効化
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
    }

    /// <summary>
    /// 計画を無効化
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
    }

    /// <summary>
    /// 計画名を更新
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plan name cannot be empty", nameof(name));

        Name = name;
    }

    /// <summary>
    /// 入手予定素材を追加/更新
    /// </summary>
    public void SetMaterial(string materialId, int amount)
    {
        if (string.IsNullOrWhiteSpace(materialId))
            throw new ArgumentException("Material ID cannot be empty", nameof(materialId));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (amount == 0)
        {
            PlannedMaterials.Remove(materialId);
        }
        else
        {
            PlannedMaterials[materialId] = amount;
        }
    }

    /// <summary>
    /// 特定素材を削除
    /// </summary>
    public void RemoveMaterial(string materialId)
    {
        PlannedMaterials.Remove(materialId);
    }

    /// <summary>
    /// すべての素材をクリア
    /// </summary>
    public void ClearMaterials()
    {
        PlannedMaterials.Clear();
    }

    /// <summary>
    /// 特定素材の数量を取得
    /// </summary>
    public int GetMaterialAmount(string materialId)
    {
        return PlannedMaterials.TryGetValue(materialId, out var amount) ? amount : 0;
    }
}
