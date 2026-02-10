using Arknights_OperatorsManager.Application.State;
using llyrtkframework.StateManagement;
using ReactiveUI;
using System.Reactive.Linq;

namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// オペレーターパネル全体の管理ViewModel
/// </summary>
public class OperatorPanelViewModel : ViewModelBase
{
    private readonly IStateStore _stateStore;
    private readonly IDisposable _selectedOperatorSubscription;
    private readonly IDisposable _stateSubscription;

    public OperatorListViewModel ListViewModel { get; }
    public OperatorDetailViewModel DetailViewModel { get; }

    public OperatorPanelViewModel(
        OperatorListViewModel listViewModel,
        OperatorDetailViewModel detailViewModel,
        IStateStore stateStore)
    {
        ListViewModel = listViewModel ?? throw new ArgumentNullException(nameof(listViewModel));
        DetailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
        _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        Title = "Operators";

        // ListViewModel の SelectedOperator 変更を IStateStore に反映
        _selectedOperatorSubscription = ListViewModel
            .WhenAnyValue(x => x.SelectedOperator)
            .Subscribe(selectedOp =>
            {
                _stateStore.UpdateState<AppGlobalState>(
                    StateKeys.AppGlobalState,
                    state =>
                    {
                        state.SelectedOperatorId = selectedOp?.Id;
                        return state;
                    });

                // DetailViewModelにも直接反映（即座のUI更新のため）
                DetailViewModel.SelectedOperator = selectedOp;
            });

        // IStateStore の変更を監視（他のコンポーネントからの変更に対応）
        _stateSubscription = _stateStore
            .WhenStateChanged<AppGlobalState>(StateKeys.AppGlobalState)
            .Where(state => state.SelectedOperatorId != DetailViewModel.SelectedOperator?.Id)
            .Subscribe(state =>
            {
                if (state.SelectedOperatorId == null)
                {
                    DetailViewModel.SelectedOperator = null;
                }
                else
                {
                    // ListViewModelから該当オペレーターを検索
                    var op = ListViewModel.Operators
                        .FirstOrDefault(o => o.Id == state.SelectedOperatorId);
                    if (op != null)
                    {
                        DetailViewModel.SelectedOperator = op;
                    }
                }
            });
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        // 子ViewModelの初期化
        ListViewModel.OnInitialize();
        DetailViewModel.OnInitialize();
    }

    public override void OnActivated()
    {
        base.OnActivated();
        ListViewModel.OnActivated();
        DetailViewModel.OnActivated();
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();
        ListViewModel.OnDeactivated();
        DetailViewModel.OnDeactivated();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _selectedOperatorSubscription?.Dispose();
            _stateSubscription?.Dispose();
        }
        base.Dispose(disposing);
    }
}
