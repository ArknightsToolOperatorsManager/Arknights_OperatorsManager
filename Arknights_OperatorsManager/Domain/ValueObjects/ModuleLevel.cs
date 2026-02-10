using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// モジュールレベル（0-3: None/Stage1/Stage2/Stage3）
/// </summary>
public sealed class ModuleLevel : SingleValueObject<int>
{
    public ModuleLevel(int value) : base(value)
    {
        if (value < 0 || value > 3)
        {
            throw new ArgumentException("ModuleLevel must be between 0 and 3", nameof(value));
        }
    }

    /// <summary>
    /// 静的ファクトリメソッド
    /// </summary>
    public static ModuleLevel FromInt(int value) => new(value);

    public static ModuleLevel None => new(0);
    public static ModuleLevel Stage1 => new(1);
    public static ModuleLevel Stage2 => new(2);
    public static ModuleLevel Stage3 => new(3);
}
