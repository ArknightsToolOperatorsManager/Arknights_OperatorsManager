using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// 昇進段階（0-2: Elite0/Elite1/Elite2）
/// </summary>
public sealed class Promotion : SingleValueObject<int>
{
    public Promotion(int value) : base(value)
    {
        if (value < 0 || value > 2)
        {
            throw new ArgumentException("Promotion must be between 0 and 2", nameof(value));
        }
    }

    /// <summary>
    /// スキル特化が可能か（昇進2のみ）
    /// </summary>
    public bool CanMastery => Value == 2;

    /// <summary>
    /// モジュールが使用可能か（昇進2のみ）
    /// </summary>
    public bool CanModule => Value == 2;

    /// <summary>
    /// スキルレベルの上限を取得
    /// </summary>
    public int MaxSkillLevel => Value switch
    {
        0 => 4, // 昇進0: スキルレベル最大4
        1 or 2 => 7, // 昇進1/2: スキルレベル最大7
        _ => throw new InvalidOperationException($"Invalid promotion value: {Value}")
    };

    /// <summary>
    /// 静的ファクトリメソッド
    /// </summary>
    public static Promotion FromInt(int value) => new(value);

    public static Promotion Elite0 => new(0);
    public static Promotion Elite1 => new(1);
    public static Promotion Elite2 => new(2);
}
