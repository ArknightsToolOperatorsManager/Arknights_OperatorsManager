using Arknights_OperatorsManager.Application.Events;
using Arknights_OperatorsManager.Domain.Entities;
using Arknights_OperatorsManager.Infrastructure.Mapping;
using Arknights_OperatorsManager.Infrastructure.Persistence;
using Arknights_OperatorsManager.Services;
using DynamicData;
using DynamicData.Binding;
using llyrtkframework.Notifications;
using Prism.Events;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// オペレーター一覧ViewModel
/// DynamicData + ReactiveUI によるフィルタリング・ソート機能
/// </summary>
public class OperatorListViewModel : ViewModelBase, IDisposable
{
    private readonly OperatorMasterDataFileManager _masterDataFileManager;
    private readonly GameDataMasterFileManager _gameDataFileManager;
    private readonly OperatorFilterService _filterService;
    private readonly IEventAggregator _eventAggregator;
    private readonly INotificationService _notificationService;
    private readonly SourceCache<Operator, string> _operatorCache;
    private readonly IDisposable _filterSubscription;
    private readonly IDisposable _searchSubscription;
    private readonly ReadOnlyObservableCollection<Operator> _operators;

    private string _searchText = string.Empty;
    private int _totalCount;
    private int _filteredCount;
    private Operator? _selectedOperator;

    /// <summary>フィルターダイアログを開くコマンド</summary>
    public ReactiveCommand<Unit, Unit> OpenFilterDialogCommand { get; }

    public OperatorListViewModel(
        OperatorMasterDataFileManager masterDataFileManager,
        GameDataMasterFileManager gameDataFileManager,
        OperatorFilterService filterService,
        IEventAggregator eventAggregator,
        INotificationService notificationService)
    {
        _masterDataFileManager = masterDataFileManager ?? throw new ArgumentNullException(nameof(masterDataFileManager));
        _gameDataFileManager = gameDataFileManager ?? throw new ArgumentNullException(nameof(gameDataFileManager));
        _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        // SourceCacheの初期化（オペレーターIDをキーとする）
        _operatorCache = new SourceCache<Operator, string>(op => op.Id);

        // コマンド初期化
        OpenFilterDialogCommand = ReactiveCommand.Create(OnOpenFilterDialog);

        // フィルター変更イベントの購読
        _filterSubscription = _eventAggregator.GetEvent<OperatorFilterChangedEvent>()
            .Subscribe(_ => RefreshFilter());

        // 検索テキスト変更のデバウンス（300ms）
        _searchSubscription = this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => RefreshFilter());

        // DynamicDataパイプライン構築
        var filterPredicate = this.WhenAnyValue(
                x => x.SearchText,
                x => x._filterService.CurrentCondition)
            .Select(_ => CreateFilterPredicate());

        _operatorCache
            .Connect()
            .Filter(filterPredicate)
            .Sort(SortExpressionComparer<Operator>.Ascending(op => op.Name.Ja))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _operators)
            .Subscribe(changeSet =>
            {
                FilteredCount = _operators.Count;
            });
    }

    /// <summary>検索テキスト</summary>
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    /// <summary>全オペレーター数</summary>
    public int TotalCount
    {
        get => _totalCount;
        set => this.RaiseAndSetIfChanged(ref _totalCount, value);
    }

    /// <summary>フィルター後のオペレーター数</summary>
    public int FilteredCount
    {
        get => _filteredCount;
        set => this.RaiseAndSetIfChanged(ref _filteredCount, value);
    }

    /// <summary>表示中のオペレーターリスト</summary>
    public ReadOnlyObservableCollection<Operator> Operators => _operators;

    /// <summary>選択中のオペレーター</summary>
    public Operator? SelectedOperator
    {
        get => _selectedOperator;
        set => this.RaiseAndSetIfChanged(ref _selectedOperator, value);
    }

    /// <summary>
    /// フィルターダイアログを開くためのコールバック（Viewで設定）
    /// </summary>
    public Action? OpenFilterDialogCallback { get; set; }

    public override void OnInitialize()
    {
        base.OnInitialize();

        // マスターデータの読み込み
        LoadOperators();
    }

    /// <summary>
    /// マスターデータからオペレーターを読み込む
    /// </summary>
    private async void LoadOperators()
    {
        try
        {
            IsBusy = true;

            // ファイルが存在しない場合は空リスト
            if (!_masterDataFileManager.FileExists || !_gameDataFileManager.FileExists)
            {
                await _notificationService.SendAsync(
                    "Master Data Not Found",
                    "No master data found. Please download master data first.",
                    NotificationType.Information);
                _operatorCache.AddOrUpdate(new List<Operator>());
                TotalCount = 0;
                FilteredCount = 0;
                return;
            }

            // マスターデータの読み込み
            var masterDataResult = await _masterDataFileManager.LoadAsync();
            if (masterDataResult.IsFailure)
            {
                await _notificationService.SendAsync(
                    "Load Error",
                    $"Failed to load operator master data: {masterDataResult.ErrorMessage}",
                    NotificationType.Error);
                return;
            }

            // ゲームデータの読み込み
            var gameDataResult = await _gameDataFileManager.LoadAsync();
            if (gameDataResult.IsFailure)
            {
                await _notificationService.SendAsync(
                    "Load Error",
                    $"Failed to load game data: {gameDataResult.ErrorMessage}",
                    NotificationType.Error);
                return;
            }

            var masterData = masterDataResult.Value;
            var gameData = gameDataResult.Value;

            // OperatorMasterDataからOperatorエンティティへの変換
            var operators = new List<Operator>();
            foreach (var entry in masterData.Operators.Values)
            {
                try
                {
                    var operatorEntity = entry.ToOperator(gameData);
                    operators.Add(operatorEntity);
                }
                catch (Exception ex)
                {
                    // 個別のオペレーターで変換エラーが発生しても、他のオペレーターは読み込む
                    System.Diagnostics.Debug.WriteLine($"Failed to convert operator {entry.Code}: {ex.Message}");
                }
            }

            _operatorCache.AddOrUpdate(operators);
            TotalCount = operators.Count;
            FilteredCount = operators.Count;

            await _notificationService.SendAsync(
                "Data Loaded",
                $"Loaded {operators.Count} operators",
                NotificationType.Success);
        }
        catch (Exception ex)
        {
            await _notificationService.SendAsync(
                "Error",
                $"Unexpected error while loading operators: {ex.Message}",
                NotificationType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// フィルター条件を再適用
    /// </summary>
    private void RefreshFilter()
    {
        // DynamicDataのFilterが自動的に再評価されるため、何もしない
        // （WhenAnyValueで監視されているため）
    }

    /// <summary>
    /// フィルターダイアログを開く
    /// </summary>
    private void OnOpenFilterDialog()
    {
        OpenFilterDialogCallback?.Invoke();
    }

    /// <summary>
    /// フィルター述語を作成
    /// </summary>
    private Func<Operator, bool> CreateFilterPredicate()
    {
        return op =>
        {
            // 検索テキストフィルター
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                if (!op.Name.Ja.Contains(searchLower, StringComparison.OrdinalIgnoreCase) &&
                    !op.Name.En.Contains(searchLower, StringComparison.OrdinalIgnoreCase) &&
                    !op.Id.Contains(searchLower, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // フィルターサービスによるフィルタリング
            return _filterService.Matches(op);
        };
    }

    public void Dispose()
    {
        _filterSubscription?.Dispose();
        _searchSubscription?.Dispose();
        _operatorCache?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Dispose();
        }
        base.Dispose(disposing);
    }
}
