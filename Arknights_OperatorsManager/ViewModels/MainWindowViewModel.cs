using Arknights_OperatorsManager.Application.State;
using llyrtkframework.Localization;
using llyrtkframework.StateManagement;
using ReactiveUI;
using System.Reactive.Linq;

namespace Arknights_OperatorsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILocalizationService? _localizationService;
    private readonly IStateStore? _stateStore;
    private string _appTitle = string.Empty;
    private string _greeting = string.Empty;
    private IDisposable? _stateSubscription;
    private IDisposable? _cultureSubscription;

    public MainWindowViewModel()
    {
        // デザイナー用のデフォルトコンストラクタ
        _localizationService = null;
        _stateStore = null;
        AppTitle = "Arknights Operators Manager";
        Greeting = "Welcome!";
    }

    public MainWindowViewModel(
        ILocalizationService localizationService,
        IStateStore stateStore)
    {
        _localizationService = localizationService;
        _stateStore = stateStore;

        // 初期値を設定
        UpdateLocalizedStrings();

        // 言語変更を監視
        _cultureSubscription = _localizationService.CultureChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateLocalizedStrings());

        // グローバル状態変更を監視（将来の拡張用）
        _stateSubscription = _stateStore
            .WhenStateChanged<AppGlobalState>(StateKeys.AppGlobalState)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(state =>
            {
                // 言語変更時のUI更新など
                // 現時点ではILocalizationServiceで処理しているため、
                // 将来的にStateStore経由の言語管理に統合する際に使用
            });
    }

    public string AppTitle
    {
        get => _appTitle;
        set => this.RaiseAndSetIfChanged(ref _appTitle, value);
    }

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }

    private void UpdateLocalizedStrings()
    {
        if (_localizationService == null) return;

        var titleResult = _localizationService.GetString("App.Name");
        AppTitle = titleResult.IsSuccess ? titleResult.Value! : "Arknights Operators Manager";

        var greetingResult = _localizationService.GetString("Common.Loading");
        Greeting = greetingResult.IsSuccess ? greetingResult.Value! : "Loading...";
    }

    public override void OnDeactivated()
    {
        _stateSubscription?.Dispose();
        _cultureSubscription?.Dispose();
        base.OnDeactivated();
    }
}
