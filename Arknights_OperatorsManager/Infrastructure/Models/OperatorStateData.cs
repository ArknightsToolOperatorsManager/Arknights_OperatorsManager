namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// オペレーター育成状態データ（JSON永続化用）
/// </summary>
public class OperatorStateData
{
    /// <summary>昇進段階（0-2）</summary>
    public int Promotion { get; set; }

    /// <summary>レベル</summary>
    public int Level { get; set; }

    /// <summary>スキルレベル（1-7）</summary>
    public int SkillLevel { get; set; }

    /// <summary>スキル1特化（0-3）</summary>
    public int Skill1Mastery { get; set; }

    /// <summary>スキル2特化（0-3）</summary>
    public int Skill2Mastery { get; set; }

    /// <summary>スキル3特化（0-3）</summary>
    public int Skill3Mastery { get; set; }

    /// <summary>モジュールX（0-3）</summary>
    public int ModuleX { get; set; }

    /// <summary>モジュールY（0-3）</summary>
    public int ModuleY { get; set; }

    /// <summary>モジュールD（0-3）</summary>
    public int ModuleD { get; set; }

    /// <summary>モジュールA（0-3）</summary>
    public int ModuleA { get; set; }

    /// <summary>
    /// デフォルトコンストラクタ（JSON逆シリアライズ用）
    /// </summary>
    public OperatorStateData()
    {
        Promotion = 0;
        Level = 1;
        SkillLevel = 1;
        Skill1Mastery = 0;
        Skill2Mastery = 0;
        Skill3Mastery = 0;
        ModuleX = 0;
        ModuleY = 0;
        ModuleD = 0;
        ModuleA = 0;
    }
}
