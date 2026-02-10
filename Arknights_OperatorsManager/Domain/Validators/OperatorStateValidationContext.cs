using Arknights_OperatorsManager.Domain.Enums;
using Arknights_OperatorsManager.Domain.ValueObjects;

namespace Arknights_OperatorsManager.Domain.Validators;

/// <summary>
/// オペレーター状態検証用のコンテキスト
/// </summary>
public class OperatorStateValidationContext
{
    /// <summary>検証対象の状態</summary>
    public OperatorState State { get; }

    /// <summary>オペレーターのレアリティ</summary>
    public Rarity Rarity { get; }

    /// <summary>オペレーターのスキル数</summary>
    public int SkillCount { get; }

    /// <summary>利用可能なモジュール種別</summary>
    public List<ModuleType> AvailableModules { get; }

    public OperatorStateValidationContext(
        OperatorState state,
        Rarity rarity,
        int skillCount,
        List<ModuleType> availableModules)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
        SkillCount = skillCount;
        AvailableModules = availableModules ?? new List<ModuleType>();
    }
}
