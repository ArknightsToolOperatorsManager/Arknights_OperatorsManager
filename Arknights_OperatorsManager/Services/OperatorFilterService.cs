using Arknights_OperatorsManager.Application.Events;
using Arknights_OperatorsManager.Domain.Entities;
using Arknights_OperatorsManager.ViewModels.Operators;
using Prism.Events;

namespace Arknights_OperatorsManager.Services;

/// <summary>
/// オペレーターフィルターサービス
/// </summary>
public class OperatorFilterService
{
    private readonly IEventAggregator _eventAggregator;

    /// <summary>現在適用中のフィルター条件</summary>
    public FilterCondition CurrentCondition { get; private set; } = new();

    /// <summary>ダイアログで設定された条件（前回の条件を保持）</summary>
    public FilterCondition DialogCondition { get; private set; } = new();

    public OperatorFilterService(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
    }

    /// <summary>
    /// フィルターを適用
    /// </summary>
    /// <param name="condition">適用する条件</param>
    public void ApplyFilter(FilterCondition condition)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        CurrentCondition = condition.Clone();
        DialogCondition = condition.Clone();

        _eventAggregator.GetEvent<OperatorFilterChangedEvent>().Publish(CurrentCondition);
    }

    /// <summary>
    /// クイックフィルターを適用（単一条件）
    /// </summary>
    /// <param name="propertyName">プロパティ名（"Rarity", "Race", "Faction" など）</param>
    /// <param name="value">フィルター値</param>
    public void ApplyQuickFilter(string propertyName, object value)
    {
        var condition = new FilterCondition();

        switch (propertyName)
        {
            case "Rarity" when value is int rarity:
                condition.Rarities.Add(rarity);
                break;
            case "Race" when value is string race:
                condition.Races.Add(race);
                break;
            case "Origin" when value is string origin:
                condition.Origins.Add(origin);
                break;
            case "Faction" when value is string faction:
                condition.Factions.Add(faction);
                break;
            case "Class" when value is string className:
                condition.Classes.Add(className);
                break;
            case "Subclass" when value is string subclass:
                condition.Subclasses.Add(subclass);
                break;
            default:
                throw new ArgumentException($"Unknown property name: {propertyName}");
        }

        CurrentCondition = condition;
        // クイックフィルターではDialogConditionは更新しない（保持）

        _eventAggregator.GetEvent<OperatorFilterChangedEvent>().Publish(CurrentCondition);
    }

    /// <summary>
    /// フィルターをリセット
    /// </summary>
    public void ResetFilter()
    {
        CurrentCondition = new FilterCondition();
        DialogCondition = new FilterCondition();

        _eventAggregator.GetEvent<OperatorFilterChangedEvent>().Publish(CurrentCondition);
    }

    /// <summary>
    /// ダイアログ条件を復元（ダイアログを再度開いてOKを押した場合と同じ）
    /// </summary>
    public void RestoreDialogCondition()
    {
        CurrentCondition = DialogCondition.Clone();

        _eventAggregator.GetEvent<OperatorFilterChangedEvent>().Publish(CurrentCondition);
    }

    /// <summary>
    /// 指定されたオペレーターが現在のフィルター条件にマッチするかチェック
    /// </summary>
    /// <param name="operator">チェック対象のオペレーター</param>
    /// <returns>マッチすれば true</returns>
    public bool Matches(Operator @operator)
    {
        if (@operator == null)
            return false;

        var condition = CurrentCondition;

        // レアリティフィルター
        if (condition.Rarities.Count > 0 && !condition.Rarities.Contains(@operator.Rarity.Value))
            return false;

        // 出身地フィルター（未実装: マスターデータ拡張後に対応）
        // if (condition.Origins.Count > 0 && !condition.Origins.Contains(@operator.Origin))
        //     return false;

        // 種族フィルター
        if (condition.Races.Count > 0 && !condition.Races.Contains(@operator.Race))
            return false;

        // 陣営フィルター
        if (condition.Factions.Count > 0 && !condition.Factions.Contains(@operator.Faction))
            return false;

        // クラスフィルター
        if (condition.Classes.Count > 0 && !condition.Classes.Contains(@operator.Class.ToString()))
            return false;

        // 職分フィルター（未実装: マスターデータ拡張後に対応）
        // if (condition.Subclasses.Count > 0 && !condition.Subclasses.Contains(@operator.Subclass))
        //     return false;

        // 性別フィルター
        if (condition.Genders.Count > 0)
        {
            var gender = @operator.Sex switch
            {
                0 => "Male",
                1 => "Female",
                _ => "Unknown"
            };
            if (!condition.Genders.Contains(gender))
                return false;
        }

        // グループフィルター
        if (condition.Groups.Count > 0)
        {
            if (string.IsNullOrEmpty(@operator.Group) || !condition.Groups.Contains(@operator.Group))
                return false;
        }

        // 所持状態フィルター
        if (condition.IsOwned.HasValue && @operator.IsUnowned == condition.IsOwned.Value)
            return false;

        // 実装状態フィルター（未実装: 日付ベースでの判定が必要）
        // if (condition.IsImplemented.HasValue)
        // {
        //     var isImplemented = @operator.AddDate != null;
        //     if (isImplemented != condition.IsImplemented.Value)
        //         return false;
        // }

        // 目標達成フィルター
        if (condition.IsGoalAchieved.HasValue && @operator.IsCompleted != condition.IsGoalAchieved.Value)
            return false;

        // 統合日付フィルター
        if (condition.DateFilter != null)
        {
            var hasMatchingDate = @operator.HasDateInRange(
                condition.DateFilter.Server,
                condition.DateFilter.StartDate,
                condition.DateFilter.EndDate
            );

            if (!hasMatchingDate)
                return false;
        }

        return true;
    }
}
