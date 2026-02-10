using Arknights_OperatorsManager.Domain.Entities;
using Arknights_OperatorsManager.Services;
using ReactiveUI;
using System.Reactive;

namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// オペレーター詳細表示ViewModel
/// </summary>
public class OperatorDetailViewModel : ViewModelBase
{
    private readonly OperatorFilterService _filterService;
    private Operator? _selectedOperator;
    private bool _isDetailVisible;

    public OperatorDetailViewModel(OperatorFilterService filterService)
    {
        _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));

        // コマンド初期化
        ApplyQuickFilterCommand = ReactiveCommand.Create<string>(OnApplyQuickFilter);
        CloseDetailCommand = ReactiveCommand.Create(OnCloseDetail);

        Title = "Operator Detail";
    }

    /// <summary>選択中のオペレーター</summary>
    public Operator? SelectedOperator
    {
        get => _selectedOperator;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOperator, value);
            IsDetailVisible = value != null;
        }
    }

    /// <summary>詳細エリアの表示状態</summary>
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => this.RaiseAndSetIfChanged(ref _isDetailVisible, value);
    }

    /// <summary>クイックフィルターコマンド</summary>
    public ReactiveCommand<string, Unit> ApplyQuickFilterCommand { get; }

    /// <summary>詳細エリアを閉じるコマンド</summary>
    public ReactiveCommand<Unit, Unit> CloseDetailCommand { get; }

    /// <summary>
    /// クイックフィルターを適用
    /// </summary>
    /// <param name="filterKey">フィルターキー（"Rarity", "Race", "Faction" など）</param>
    private void OnApplyQuickFilter(string filterKey)
    {
        if (SelectedOperator == null)
            return;

        switch (filterKey)
        {
            case "Rarity":
                _filterService.ApplyQuickFilter("Rarity", SelectedOperator.Rarity.Value);
                break;
            case "Race":
                _filterService.ApplyQuickFilter("Race", SelectedOperator.Race);
                break;
            case "Faction":
                _filterService.ApplyQuickFilter("Faction", SelectedOperator.Faction);
                break;
            case "Class":
                _filterService.ApplyQuickFilter("Class", SelectedOperator.Class.ToString());
                break;
            default:
                // 未知のフィルターキー
                break;
        }
    }

    private void OnCloseDetail()
    {
        SelectedOperator = null;
        IsDetailVisible = false;
    }
}
