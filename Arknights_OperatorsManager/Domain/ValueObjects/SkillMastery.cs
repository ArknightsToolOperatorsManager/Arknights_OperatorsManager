using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// スキル特化レベル（0-3: None/M1/M2/M3）
/// </summary>
public sealed class SkillMastery : SingleValueObject<int>
{
    public SkillMastery(int value) : base(value)
    {
        if (value < 0 || value > 3)
        {
            throw new ArgumentException("SkillMastery must be between 0 and 3", nameof(value));
        }
    }

    /// <summary>
    /// 静的ファクトリメソッド
    /// </summary>
    public static SkillMastery FromInt(int value) => new(value);

    public static SkillMastery None => new(0);
    public static SkillMastery M1 => new(1);
    public static SkillMastery M2 => new(2);
    public static SkillMastery M3 => new(3);
}
