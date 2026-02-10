using Arknights_OperatorsManager.Domain.Enums;
using Arknights_OperatorsManager.Domain.ValueObjects;
using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.Entities;

/// <summary>
/// オペレーターエンティティ（公式IDで識別）
/// </summary>
public class Operator : Entity<string>
{
    // ===== マスターデータ（変更不可） =====

    /// <summary>名前（多言語対応）</summary>
    public LocalizedName Name { get; private set; }

    /// <summary>レアリティ（1-6）</summary>
    public Rarity Rarity { get; private set; }

    /// <summary>職業</summary>
    public OperatorClass Class { get; private set; }

    /// <summary>陣営</summary>
    public string Faction { get; private set; }

    /// <summary>種族</summary>
    public string Race { get; private set; }

    /// <summary>タグ</summary>
    public List<string> Tags { get; private set; }

    /// <summary>スキル数（1-3）</summary>
    public int SkillCount { get; private set; }

    /// <summary>利用可能モジュール</summary>
    public List<ModuleType> AvailableModules { get; private set; }

    /// <summary>追加日（サーバー別）</summary>
    public ServerDate AddDate { get; private set; }

    /// <summary>性別（0=男性, 1=女性）</summary>
    public int Sex { get; private set; }

    /// <summary>配置位置</summary>
    public int Place { get; private set; }

    /// <summary>パラドックスシミュレータ追加日（段階 → サーバー別日付）</summary>
    public Dictionary<string, ServerDate>? Paradox { get; private set; }

    /// <summary>モジュール追加日（モジュール種別 → サーバー別日付）</summary>
    public Dictionary<string, ServerDate>? ModuleAddDates { get; private set; }

    /// <summary>異格コードリスト</summary>
    public List<string>? Alternates { get; private set; }

    /// <summary>ページ番号</summary>
    public int Page { get; private set; }

    // ===== ユーザーデータ（変更可能） =====

    /// <summary>現在の育成状態</summary>
    public OperatorState CurrentState { get; private set; }

    /// <summary>目標の育成状態</summary>
    public OperatorState TargetState { get; private set; }

    /// <summary>潜在（0-6、0=未所持）</summary>
    public int Potential { get; private set; }

    /// <summary>信頼度（0-200）</summary>
    public int Trust { get; private set; }

    /// <summary>逆理演算クリア済み</summary>
    public bool ParadoxCleared { get; private set; }

    /// <summary>グループ名</summary>
    public string? Group { get; private set; }

    /// <summary>優先度</summary>
    public int Priority { get; private set; }

    /// <summary>メモ</summary>
    public string? Memo { get; private set; }

    public Operator(
        string id,
        LocalizedName name,
        Rarity rarity,
        OperatorClass operatorClass,
        string faction,
        string race,
        List<string> tags,
        int skillCount,
        List<ModuleType> availableModules,
        ServerDate addDate,
        int sex,
        int place,
        OperatorState currentState,
        OperatorState targetState,
        int potential = 0,
        int trust = 0,
        bool paradoxCleared = false,
        string? group = null,
        int priority = 0,
        string? memo = null,
        Dictionary<string, ServerDate>? paradox = null,
        Dictionary<string, ServerDate>? moduleAddDates = null,
        List<string>? alternates = null,
        int page = 0) : base(id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Operator ID cannot be empty", nameof(id));

        if (skillCount < 1 || skillCount > 3)
            throw new ArgumentException("Skill count must be between 1 and 3", nameof(skillCount));

        if (potential < 0 || potential > 6)
            throw new ArgumentException("Potential must be between 0 and 6", nameof(potential));

        if (trust < 0 || trust > 200)
            throw new ArgumentException("Trust must be between 0 and 200", nameof(trust));

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
        Class = operatorClass;
        Faction = faction ?? throw new ArgumentNullException(nameof(faction));
        Race = race ?? throw new ArgumentNullException(nameof(race));
        Tags = tags ?? new List<string>();
        SkillCount = skillCount;
        AvailableModules = availableModules ?? new List<ModuleType>();
        AddDate = addDate ?? throw new ArgumentNullException(nameof(addDate));
        Sex = sex;
        Place = place;
        CurrentState = currentState ?? throw new ArgumentNullException(nameof(currentState));
        TargetState = targetState ?? throw new ArgumentNullException(nameof(targetState));
        Potential = potential;
        Trust = trust;
        ParadoxCleared = paradoxCleared;
        Group = group;
        Priority = priority;
        Memo = memo;
        Paradox = paradox;
        ModuleAddDates = moduleAddDates;
        Alternates = alternates;
        Page = page;
    }

    // ===== ユーザーデータ更新メソッド =====

    /// <summary>現在の育成状態を更新</summary>
    public void UpdateCurrentState(OperatorState state)
    {
        CurrentState = state ?? throw new ArgumentNullException(nameof(state));
    }

    /// <summary>目標の育成状態を更新</summary>
    public void UpdateTargetState(OperatorState state)
    {
        TargetState = state ?? throw new ArgumentNullException(nameof(state));
    }

    /// <summary>潜在を更新</summary>
    public void UpdatePotential(int potential)
    {
        if (potential < 0 || potential > 6)
            throw new ArgumentException("Potential must be between 0 and 6", nameof(potential));

        Potential = potential;
    }

    /// <summary>信頼度を更新</summary>
    public void UpdateTrust(int trust)
    {
        if (trust < 0 || trust > 200)
            throw new ArgumentException("Trust must be between 0 and 200", nameof(trust));

        Trust = trust;
    }

    /// <summary>逆理演算クリア状態を更新</summary>
    public void UpdateParadoxCleared(bool cleared)
    {
        ParadoxCleared = cleared;
    }

    /// <summary>グループを更新</summary>
    public void UpdateGroup(string? group)
    {
        Group = group;
    }

    /// <summary>優先度を更新</summary>
    public void UpdatePriority(int priority)
    {
        Priority = priority;
    }

    /// <summary>メモを更新</summary>
    public void UpdateMemo(string? memo)
    {
        Memo = memo;
    }

    // ===== ビジネスロジック =====

    /// <summary>
    /// 育成が完了しているか（現在状態 == 目標状態）
    /// </summary>
    public bool IsCompleted => CurrentState.Equals(TargetState);

    /// <summary>
    /// 未所持か（潜在が0）
    /// </summary>
    public bool IsUnowned => Potential == 0;

    /// <summary>
    /// モジュールが解放されているか
    /// </summary>
    public bool CanUseModule(Rarity rarity, OperatorState state)
    {
        if (state.Promotion.Value < 2)
            return false;

        return rarity.Value switch
        {
            6 => state.Level >= 60,
            5 => state.Level >= 50,
            4 => state.Level >= 40,
            _ => false
        };
    }

    /// <summary>
    /// 統合日付フィルタ: 指定された日付範囲内に該当する日付があるかチェック
    /// （AddDate、Paradox、ModuleAddDates のいずれかが範囲内であれば true）
    /// </summary>
    /// <param name="server">サーバー名 ("China" or "Global")</param>
    /// <param name="startDate">開始日</param>
    /// <param name="endDate">終了日</param>
    /// <returns>範囲内の日付があれば true</returns>
    public bool HasDateInRange(string server, DateTime startDate, DateTime endDate)
    {
        // AddDateチェック
        if (AddDate.IsInRange(server, startDate, endDate))
            return true;

        // Paradoxチェック
        if (Paradox != null)
        {
            foreach (var date in Paradox.Values)
            {
                if (date.IsInRange(server, startDate, endDate))
                    return true;
            }
        }

        // ModuleAddDatesチェック
        if (ModuleAddDates != null)
        {
            foreach (var date in ModuleAddDates.Values)
            {
                if (date.IsInRange(server, startDate, endDate))
                    return true;
            }
        }

        return false;
    }
}
