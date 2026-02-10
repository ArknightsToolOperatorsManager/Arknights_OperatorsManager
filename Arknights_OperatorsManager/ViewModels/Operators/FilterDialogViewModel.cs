using Arknights_OperatorsManager.Services;
using ReactiveUI;
using System.Reactive;

namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// フィルターダイアログViewModel
/// </summary>
public class FilterDialogViewModel : ViewModelBase
{
    private readonly OperatorFilterService _filterService;

    // レアリティ選択状態（1-6）
    private bool _rarity1;
    private bool _rarity2;
    private bool _rarity3;
    private bool _rarity4;
    private bool _rarity5;
    private bool _rarity6;

    // 性別選択状態
    private bool _male;
    private bool _female;

    // 所持状態選択
    private bool? _isOwned;

    // 目標達成状態選択
    private bool? _isGoalAchieved;

    // 日付フィルター
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string _selectedServer = "Global";
    private bool _enableDateFilter;

    public FilterDialogViewModel(OperatorFilterService filterService)
    {
        _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));

        // コマンド初期化
        OkCommand = ReactiveCommand.Create(OnOk);
        CancelCommand = ReactiveCommand.Create(OnCancel);
        ResetCommand = ReactiveCommand.Create(OnReset);

        Title = "Filter";
    }

    #region プロパティ

    public bool Rarity1
    {
        get => _rarity1;
        set => this.RaiseAndSetIfChanged(ref _rarity1, value);
    }

    public bool Rarity2
    {
        get => _rarity2;
        set => this.RaiseAndSetIfChanged(ref _rarity2, value);
    }

    public bool Rarity3
    {
        get => _rarity3;
        set => this.RaiseAndSetIfChanged(ref _rarity3, value);
    }

    public bool Rarity4
    {
        get => _rarity4;
        set => this.RaiseAndSetIfChanged(ref _rarity4, value);
    }

    public bool Rarity5
    {
        get => _rarity5;
        set => this.RaiseAndSetIfChanged(ref _rarity5, value);
    }

    public bool Rarity6
    {
        get => _rarity6;
        set => this.RaiseAndSetIfChanged(ref _rarity6, value);
    }

    public bool Male
    {
        get => _male;
        set => this.RaiseAndSetIfChanged(ref _male, value);
    }

    public bool Female
    {
        get => _female;
        set => this.RaiseAndSetIfChanged(ref _female, value);
    }

    public bool? IsOwned
    {
        get => _isOwned;
        set => this.RaiseAndSetIfChanged(ref _isOwned, value);
    }

    public bool? IsGoalAchieved
    {
        get => _isGoalAchieved;
        set => this.RaiseAndSetIfChanged(ref _isGoalAchieved, value);
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }

    public string SelectedServer
    {
        get => _selectedServer;
        set => this.RaiseAndSetIfChanged(ref _selectedServer, value);
    }

    public bool EnableDateFilter
    {
        get => _enableDateFilter;
        set => this.RaiseAndSetIfChanged(ref _enableDateFilter, value);
    }

    #endregion

    #region コマンド

    public ReactiveCommand<Unit, Unit> OkCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    #endregion

    public override void OnInitialize()
    {
        base.OnInitialize();

        // 前回のフィルター条件を復元
        LoadFromFilterService();
    }

    private void LoadFromFilterService()
    {
        var condition = _filterService.DialogCondition;

        // レアリティ
        Rarity1 = condition.Rarities.Contains(1);
        Rarity2 = condition.Rarities.Contains(2);
        Rarity3 = condition.Rarities.Contains(3);
        Rarity4 = condition.Rarities.Contains(4);
        Rarity5 = condition.Rarities.Contains(5);
        Rarity6 = condition.Rarities.Contains(6);

        // 性別
        Male = condition.Genders.Contains("Male");
        Female = condition.Genders.Contains("Female");

        // 所持状態
        IsOwned = condition.IsOwned;

        // 目標達成状態
        IsGoalAchieved = condition.IsGoalAchieved;

        // 日付フィルター
        if (condition.DateFilter != null)
        {
            EnableDateFilter = true;
            StartDate = condition.DateFilter.StartDate;
            EndDate = condition.DateFilter.EndDate;
            SelectedServer = condition.DateFilter.Server;
        }
        else
        {
            EnableDateFilter = false;
            StartDate = null;
            EndDate = null;
            SelectedServer = "Global";
        }
    }

    private void OnOk()
    {
        var condition = new FilterCondition();

        // レアリティ
        if (Rarity1) condition.Rarities.Add(1);
        if (Rarity2) condition.Rarities.Add(2);
        if (Rarity3) condition.Rarities.Add(3);
        if (Rarity4) condition.Rarities.Add(4);
        if (Rarity5) condition.Rarities.Add(5);
        if (Rarity6) condition.Rarities.Add(6);

        // 性別
        if (Male) condition.Genders.Add("Male");
        if (Female) condition.Genders.Add("Female");

        // 所持状態
        condition.IsOwned = IsOwned;

        // 目標達成状態
        condition.IsGoalAchieved = IsGoalAchieved;

        // 日付フィルター
        if (EnableDateFilter && StartDate.HasValue && EndDate.HasValue)
        {
            condition.DateFilter = new DateRange(StartDate.Value, EndDate.Value, SelectedServer);
        }

        _filterService.ApplyFilter(condition);

        // ダイアログを閉じる（Viewで処理）
        CloseDialog?.Invoke(true);
    }

    private void OnCancel()
    {
        // ダイアログを閉じる（変更を破棄）
        CloseDialog?.Invoke(false);
    }

    private void OnReset()
    {
        Rarity1 = false;
        Rarity2 = false;
        Rarity3 = false;
        Rarity4 = false;
        Rarity5 = false;
        Rarity6 = false;
        Male = false;
        Female = false;
        IsOwned = null;
        IsGoalAchieved = null;
        EnableDateFilter = false;
        StartDate = null;
        EndDate = null;
        SelectedServer = "Global";
    }

    /// <summary>
    /// ダイアログを閉じるためのコールバック（Viewで設定）
    /// </summary>
    public Action<bool>? CloseDialog { get; set; }
}
