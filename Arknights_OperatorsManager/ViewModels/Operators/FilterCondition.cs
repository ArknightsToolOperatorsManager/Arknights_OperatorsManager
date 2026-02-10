namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// オペレーターフィルターの条件クラス
/// </summary>
public class FilterCondition
{
    /// <summary>レアリティ（1-6）</summary>
    public HashSet<int> Rarities { get; set; } = new();

    /// <summary>出身地</summary>
    public HashSet<string> Origins { get; set; } = new();

    /// <summary>種族</summary>
    public HashSet<string> Races { get; set; } = new();

    /// <summary>陣営</summary>
    public HashSet<string> Factions { get; set; } = new();

    /// <summary>クラス</summary>
    public HashSet<string> Classes { get; set; } = new();

    /// <summary>職分</summary>
    public HashSet<string> Subclasses { get; set; } = new();

    /// <summary>性別</summary>
    public HashSet<string> Genders { get; set; } = new();

    /// <summary>グループ</summary>
    public HashSet<string> Groups { get; set; } = new();

    /// <summary>所持状態（true=所持、false=未所持、null=全て）</summary>
    public bool? IsOwned { get; set; }

    /// <summary>実装状態（true=実装済み、false=未実装、null=全て）</summary>
    public bool? IsImplemented { get; set; }

    /// <summary>目標達成状態（true=達成、false=未達成、null=全て）</summary>
    public bool? IsGoalAchieved { get; set; }

    /// <summary>日付フィルター（null=フィルターなし）</summary>
    public DateRange? DateFilter { get; set; }

    /// <summary>
    /// すべての条件が空（フィルターなし）かチェック
    /// </summary>
    public bool IsEmpty =>
        Rarities.Count == 0 &&
        Origins.Count == 0 &&
        Races.Count == 0 &&
        Factions.Count == 0 &&
        Classes.Count == 0 &&
        Subclasses.Count == 0 &&
        Genders.Count == 0 &&
        Groups.Count == 0 &&
        IsOwned == null &&
        IsImplemented == null &&
        IsGoalAchieved == null &&
        DateFilter == null;

    /// <summary>
    /// ディープコピーを作成
    /// </summary>
    public FilterCondition Clone()
    {
        return new FilterCondition
        {
            Rarities = new HashSet<int>(Rarities),
            Origins = new HashSet<string>(Origins),
            Races = new HashSet<string>(Races),
            Factions = new HashSet<string>(Factions),
            Classes = new HashSet<string>(Classes),
            Subclasses = new HashSet<string>(Subclasses),
            Genders = new HashSet<string>(Genders),
            Groups = new HashSet<string>(Groups),
            IsOwned = IsOwned,
            IsImplemented = IsImplemented,
            IsGoalAchieved = IsGoalAchieved,
            DateFilter = DateFilter?.Clone()
        };
    }

    /// <summary>
    /// すべての条件をクリア
    /// </summary>
    public void Reset()
    {
        Rarities.Clear();
        Origins.Clear();
        Races.Clear();
        Factions.Clear();
        Classes.Clear();
        Subclasses.Clear();
        Genders.Clear();
        Groups.Clear();
        IsOwned = null;
        IsImplemented = null;
        IsGoalAchieved = null;
        DateFilter = null;
    }
}
