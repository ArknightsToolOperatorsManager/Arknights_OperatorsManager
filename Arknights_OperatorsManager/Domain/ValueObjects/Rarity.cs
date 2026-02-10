using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// レアリティ（1-6）
/// </summary>
public sealed class Rarity : SingleValueObject<int>
{
    public Rarity(int value) : base(value)
    {
        if (value < 1 || value > 6)
        {
            throw new ArgumentException("Rarity must be between 1 and 6", nameof(value));
        }
    }

    /// <summary>
    /// レアリティから最大昇進段階を取得
    /// </summary>
    public int MaxPromotion => Value switch
    {
        1 or 2 => 0, // ★1~2: 昇進0のみ
        3 => 1,      // ★3: 昇進1まで
        4 or 5 or 6 => 2, // ★4~6: 昇進2まで
        _ => throw new InvalidOperationException($"Invalid rarity value: {Value}")
    };

    /// <summary>
    /// 昇進段階ごとの最大レベルを取得
    /// </summary>
    /// <param name="promotion">昇進段階</param>
    /// <returns>最大レベル</returns>
    public int GetMaxLevel(Promotion promotion)
    {
        return (Value, promotion.Value) switch
        {
            (6, 0) => 50,
            (6, 1) => 80,
            (6, 2) => 90,
            (5, 0) => 50,
            (5, 1) => 70,
            (5, 2) => 80,
            (4, 0) => 45,
            (4, 1) => 60,
            (4, 2) => 70,
            (3, 0) => 40,
            (3, 1) => 55,
            (2, 0) => 30,
            (1, 0) => 30,
            _ => throw new ArgumentException($"Invalid combination: Rarity={Value}, Promotion={promotion.Value}")
        };
    }

    /// <summary>
    /// 静的ファクトリメソッド
    /// </summary>
    public static Rarity FromInt(int value) => new(value);
}
