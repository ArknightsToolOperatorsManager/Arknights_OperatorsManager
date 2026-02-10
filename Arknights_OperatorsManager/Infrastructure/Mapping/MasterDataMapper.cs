using Arknights_OperatorsManager.Domain.Entities;
using Arknights_OperatorsManager.Domain.Enums;
using Arknights_OperatorsManager.Domain.ValueObjects;

namespace Arknights_OperatorsManager.Infrastructure.Mapping;

/// <summary>
/// マスターデータのマッピング処理（Infrastructure.Models ⇔ Domain.ValueObjects）
/// </summary>
public static class MasterDataMapper
{
    /// <summary>
    /// Infrastructure.Models.LocalizedName → Domain.ValueObjects.LocalizedName
    /// </summary>
    public static Domain.ValueObjects.LocalizedName ToDomain(this Models.LocalizedName source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new Domain.ValueObjects.LocalizedName(
            source.Ja ?? string.Empty,
            source.En ?? string.Empty,
            source.Ch ?? string.Empty
        );
    }

    /// <summary>
    /// Infrastructure.Models.ServerDate → Domain.ValueObjects.ServerDate
    /// </summary>
    public static Domain.ValueObjects.ServerDate ToDomain(this Models.ServerDate source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new Domain.ValueObjects.ServerDate(
            source.China ?? string.Empty,
            source.Global ?? string.Empty
        );
    }

    /// <summary>
    /// Dictionary<string, Infrastructure.Models.ServerDate> → Dictionary<string, Domain.ValueObjects.ServerDate>
    /// </summary>
    public static Dictionary<string, Domain.ValueObjects.ServerDate>? ToDomain(
        this Dictionary<string, Models.ServerDate>? source)
    {
        if (source == null)
            return null;

        return source.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToDomain()
        );
    }

    /// <summary>
    /// MasterOperatorEntry + UserOperatorEntry → Operator エンティティ
    /// </summary>
    /// <param name="master">マスターデータ</param>
    /// <param name="gameData">ゲームデータマスター（ID→名前変換用）</param>
    /// <param name="user">ユーザーデータ（null=未所持）</param>
    /// <param name="language">言語設定（デフォルト: ja-JP）</param>
    /// <returns>Operatorエンティティ</returns>
    public static Operator ToOperator(
        this Models.MasterOperatorEntry master,
        Models.GameDataMaster gameData,
        Models.UserOperatorEntry? user = null,
        string language = "ja-JP")
    {
        if (master == null)
            throw new ArgumentNullException(nameof(master));
        if (gameData == null)
            throw new ArgumentNullException(nameof(gameData));

        // 職業の変換
        var operatorClass = (OperatorClass)master.Class;

        // Race, Faction の変換（ID → 名前）
        var race = gameData.GetRaceName(master.Race, language);
        var faction = gameData.GetFactionName(master.Faction, language);

        // Tags の生成（TODO: 実際のタグデータがある場合は変換）
        var tags = new List<string>();

        // AvailableModules の生成（TODO: マスターデータから抽出）
        var availableModules = master.Modules.Keys
            .Select(k => Enum.TryParse<ModuleType>(k, out var moduleType) ? moduleType : (ModuleType?)null)
            .Where(m => m.HasValue)
            .Select(m => m!.Value)
            .ToList();

        // ユーザーデータがない場合はデフォルト値
        var potential = user?.Potential ?? 0;
        var group = user?.Group;

        // 現在の状態
        var currentState = user?.CurrentState != null
            ? user.CurrentState.ToOperatorState()
            : OperatorState.Initial;

        // 目標の状態
        var targetState = user?.TargetState != null
            ? user.TargetState.ToOperatorState()
            : OperatorState.Initial;

        return new Operator(
            id: master.Code,
            name: master.Name.ToDomain(),
            rarity: new Rarity(master.RarityValue),
            operatorClass: operatorClass,
            faction: faction,
            race: race,
            tags: tags,
            skillCount: 3, // TODO: マスターデータから取得
            availableModules: availableModules,
            addDate: master.AddDate.ToDomain(),
            sex: master.Sex,
            place: master.Place,
            currentState: currentState,
            targetState: targetState,
            potential: potential,
            trust: user?.Trust ?? 0,
            paradoxCleared: user?.ParadoxCleared ?? false,
            group: group,
            priority: user?.Priority ?? 0,
            memo: user?.Memo,
            paradox: master.Paradox.ToDomain(),
            moduleAddDates: master.ModuleAddDates.ToDomain(),
            alternates: master.Alternates,
            page: master.Page
        );
    }

    /// <summary>
    /// OperatorStateData → OperatorState
    /// </summary>
    public static OperatorState ToOperatorState(this Models.OperatorStateData source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new OperatorState(
            promotion: new Promotion(source.Promotion),
            level: source.Level,
            skillLevel: source.SkillLevel,
            skill1Mastery: new SkillMastery(source.Skill1Mastery),
            skill2Mastery: new SkillMastery(source.Skill2Mastery),
            skill3Mastery: new SkillMastery(source.Skill3Mastery),
            moduleX: new ModuleLevel(source.ModuleX),
            moduleY: new ModuleLevel(source.ModuleY),
            moduleD: new ModuleLevel(source.ModuleD),
            moduleA: new ModuleLevel(source.ModuleA)
        );
    }
}
