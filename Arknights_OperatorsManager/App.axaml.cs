using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Prism.DryIoc;
using Prism.Ioc;
using Arknights_OperatorsManager.ViewModels;
using Arknights_OperatorsManager.Views;
using Arknights_OperatorsManager.Services;
using Arknights_OperatorsManager.Bootstrap;
using Arknights_OperatorsManager.ViewModels.Operators;
using Arknights_OperatorsManager.Views.Operators;
using Arknights_OperatorsManager.Infrastructure.Persistence;
using Prism.Events;
using llyrtkframework.Application;
using llyrtkframework.Localization;
using llyrtkframework.Notifications;
using llyrtkframework.StateManagement;
using Arknights_OperatorsManager.Application.State;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Serilog;

namespace Arknights_OperatorsManager;

public partial class App : PrismApplication
{
    private CrashRecoveryManager? _crashRecoveryManager;
    private ApplicationInstanceManager? _instanceManager;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override async void OnInitialized()
    {
        // Avalonia の DataAnnotation バリデーションを無効化
        // CommunityToolkit.Mvvm と FluentValidation を使用するため
        DisableAvaloniaDataAnnotationValidation();

        // ApplicationBootstrapper でタスクを実行
        var logger = Container.Resolve<ILogger<ApplicationBootstrapper>>();
        var bootstrapper = new ApplicationBootstrapper(logger);

        // Core Init: データ読み込み
        bootstrapper.AddCoreInitTask(Container.Resolve<DataLoadTask>());

        // Core Init: 状態初期化（DataLoadTaskの後）
        bootstrapper.AddCoreInitTask(Container.Resolve<InitializeStateTask>());

        // UI Setup: 更新チェック
        bootstrapper.AddUiSetupTask(Container.Resolve<UpdateCheckTask>());

        // ブートストラップ実行
        var bootstrapResult = await bootstrapper.BootstrapAsync();
        if (bootstrapResult.IsFailure)
        {
            Log.Fatal("Bootstrap failed: {Error}", bootstrapResult.ErrorMessage);
            // ブートストラップ失敗時はアプリケーションを終了
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown(1);
            }
            return;
        }

        Log.Information("Bootstrap completed successfully");

        // テーマの初期適用
        InitializeThemeFromState();

        // アプリケーション終了時のイベントを登録
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop2)
        {
            desktop2.ShutdownRequested += OnShutdownRequested;
        }

        base.OnInitialized();
    }

    private void InitializeThemeFromState()
    {
        try
        {
            var stateStore = Container.Resolve<IStateStore>();
            var themeService = Container.Resolve<IThemeService>();

            var stateResult = stateStore.GetState<AppGlobalState>(StateKeys.AppGlobalState);
            if (stateResult.IsSuccess)
            {
                themeService.ApplyTheme(stateResult.Value!.CurrentTheme);
            }

            // 以降の変更を自動適用
            stateStore
                .WhenStateChanged<AppGlobalState>(StateKeys.AppGlobalState)
                .Subscribe(state => themeService.ApplyTheme(state.CurrentTheme));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to initialize theme from state");
        }
    }

    protected override AvaloniaObject CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // ロガーファクトリの登録
        containerRegistry.RegisterSingleton<ILoggerFactory>(() =>
            new SerilogLoggerFactory(Log.Logger));

        // 汎用ロガーの登録
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        // llyrtkframework サービスの登録
        containerRegistry.RegisterSingleton<ILocalizationService, EmbeddedLocalizationService>();
        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

        // IStateStore をSingleton登録
        containerRegistry.RegisterSingleton<IStateStore>(provider =>
        {
            var stateLogger = provider.Resolve<ILogger<StateStore>>();
            return new StateStore(stateLogger);
        });

        // IThemeService をSingleton登録
        containerRegistry.RegisterSingleton<IThemeService>(provider =>
        {
            var themeLogger = provider.Resolve<ILogger<ThemeService>>();
            return new ThemeService(Current!, themeLogger);
        });

        // StateToSettingsSyncService をSingleton登録
        containerRegistry.RegisterSingleton<StateToSettingsSyncService>();

        // Bootstrap タスクの登録
        containerRegistry.RegisterSingleton<DataLoadTask>();
        containerRegistry.RegisterSingleton<UpdateCheckTask>();
        containerRegistry.RegisterSingleton<InitializeStateTask>();

        // Prism EventAggregator の登録
        containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();

        // Infrastructure: FileManagers の登録
        containerRegistry.RegisterSingleton<OperatorMasterDataFileManager>();
        containerRegistry.RegisterSingleton<OperatorUserDataFileManager>();
        containerRegistry.RegisterSingleton<MaterialMasterDataFileManager>();
        containerRegistry.RegisterSingleton<MaterialUserDataFileManager>();
        containerRegistry.RegisterSingleton<GameDataMasterFileManager>();
        containerRegistry.RegisterSingleton<AppSettingsFileManager>();
        containerRegistry.RegisterSingleton<MaterialPlanDataFileManager>();

        // Application Services の登録
        containerRegistry.RegisterSingleton<OperatorFilterService>();

        // Views の登録
        containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

        // Operators Views の登録
        containerRegistry.RegisterForNavigation<OperatorPanelView, OperatorPanelViewModel>();

        // Operators ViewModels の登録（子ViewModel）
        containerRegistry.Register<OperatorListViewModel>();
        containerRegistry.Register<OperatorDetailViewModel>();
        containerRegistry.Register<FilterDialogViewModel>();

        // TODO: 追加の View/ViewModel ペアを登録
        // containerRegistry.RegisterForNavigation<MaterialListView, MaterialListViewModel>();
    }

    private async void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        try
        {
            // 状態をAppSettingsに保存
            var syncService = Container.Resolve<StateToSettingsSyncService>();
            await syncService.SaveStateToSettingsAsync();
            Log.Information("State saved on shutdown");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to save state on shutdown");
        }

        // 正常終了をマーク（Program.csで処理するため、ここでは追加処理なし）
        // クラッシュフラグのクリアとインスタンス解放はProgram.csで行う
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    /// <summary>
    /// CrashRecoveryManager を設定（Program.cs から呼び出し）
    /// </summary>
    public void SetCrashRecoveryManager(CrashRecoveryManager manager)
    {
        _crashRecoveryManager = manager;
    }

    /// <summary>
    /// ApplicationInstanceManager を設定（Program.cs から呼び出し）
    /// </summary>
    public void SetInstanceManager(ApplicationInstanceManager manager)
    {
        _instanceManager = manager;
    }
}
