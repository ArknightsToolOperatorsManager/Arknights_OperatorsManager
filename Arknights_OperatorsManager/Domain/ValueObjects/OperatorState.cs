using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// オペレーター育成状態（現在状態・目標状態で共通使用）
/// 不変オブジェクト
/// </summary>
public sealed class OperatorState : ValueObject
{
    /// <summary>昇進段階</summary>
    public Promotion Promotion { get; }

    /// <summary>レベル</summary>
    public int Level { get; }

    /// <summary>スキルレベル（1-7）</summary>
    public int SkillLevel { get; }

    /// <summary>スキル1特化（0-3）</summary>
    public SkillMastery Skill1Mastery { get; }

    /// <summary>スキル2特化（0-3）</summary>
    public SkillMastery Skill2Mastery { get; }

    /// <summary>スキル3特化（0-3）</summary>
    public SkillMastery Skill3Mastery { get; }

    /// <summary>モジュールX（0-3）</summary>
    public ModuleLevel ModuleX { get; }

    /// <summary>モジュールY（0-3）</summary>
    public ModuleLevel ModuleY { get; }

    /// <summary>モジュールD（0-3）</summary>
    public ModuleLevel ModuleD { get; }

    /// <summary>モジュールA（0-3）</summary>
    public ModuleLevel ModuleA { get; }

    public OperatorState(
        Promotion promotion,
        int level,
        int skillLevel,
        SkillMastery skill1Mastery,
        SkillMastery skill2Mastery,
        SkillMastery skill3Mastery,
        ModuleLevel moduleX,
        ModuleLevel moduleY,
        ModuleLevel moduleD,
        ModuleLevel moduleA)
    {
        Promotion = promotion ?? throw new ArgumentNullException(nameof(promotion));
        Level = level;
        SkillLevel = skillLevel;
        Skill1Mastery = skill1Mastery ?? throw new ArgumentNullException(nameof(skill1Mastery));
        Skill2Mastery = skill2Mastery ?? throw new ArgumentNullException(nameof(skill2Mastery));
        Skill3Mastery = skill3Mastery ?? throw new ArgumentNullException(nameof(skill3Mastery));
        ModuleX = moduleX ?? throw new ArgumentNullException(nameof(moduleX));
        ModuleY = moduleY ?? throw new ArgumentNullException(nameof(moduleY));
        ModuleD = moduleD ?? throw new ArgumentNullException(nameof(moduleD));
        ModuleA = moduleA ?? throw new ArgumentNullException(nameof(moduleA));
    }

    /// <summary>
    /// 初期状態（すべて最小値）を生成
    /// </summary>
    public static OperatorState Initial => new(
        Promotion.Elite0,
        1,
        1,
        SkillMastery.None,
        SkillMastery.None,
        SkillMastery.None,
        ModuleLevel.None,
        ModuleLevel.None,
        ModuleLevel.None,
        ModuleLevel.None
    );

    /// <summary>
    /// 完全育成状態（レアリティ6の最大値）を生成
    /// </summary>
    public static OperatorState MaxFor6Star => new(
        Promotion.Elite2,
        90,
        7,
        SkillMastery.M3,
        SkillMastery.M3,
        SkillMastery.M3,
        ModuleLevel.Stage3,
        ModuleLevel.Stage3,
        ModuleLevel.Stage3,
        ModuleLevel.Stage3
    );

    /// <summary>
    /// 昇進段階を変更した新しい状態を生成
    /// </summary>
    public OperatorState WithPromotion(Promotion promotion) => new(
        promotion,
        Level,
        SkillLevel,
        Skill1Mastery,
        Skill2Mastery,
        Skill3Mastery,
        ModuleX,
        ModuleY,
        ModuleD,
        ModuleA
    );

    /// <summary>
    /// レベルを変更した新しい状態を生成
    /// </summary>
    public OperatorState WithLevel(int level) => new(
        Promotion,
        level,
        SkillLevel,
        Skill1Mastery,
        Skill2Mastery,
        Skill3Mastery,
        ModuleX,
        ModuleY,
        ModuleD,
        ModuleA
    );

    /// <summary>
    /// スキルレベルを変更した新しい状態を生成
    /// </summary>
    public OperatorState WithSkillLevel(int skillLevel) => new(
        Promotion,
        Level,
        skillLevel,
        Skill1Mastery,
        Skill2Mastery,
        Skill3Mastery,
        ModuleX,
        ModuleY,
        ModuleD,
        ModuleA
    );

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Promotion;
        yield return Level;
        yield return SkillLevel;
        yield return Skill1Mastery;
        yield return Skill2Mastery;
        yield return Skill3Mastery;
        yield return ModuleX;
        yield return ModuleY;
        yield return ModuleD;
        yield return ModuleA;
    }
}
