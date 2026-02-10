using Avalonia;
using llyrtkframework.Application;
using llyrtkframework.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Arknights_OperatorsManager;

sealed class Program
{
    private const string AppName = "ArknightsOperatorsManager";

    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        AppName);

    private static ApplicationInstanceManager? _instanceManager;
    private static CrashRecoveryManager? _crashRecoveryManager;

    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            // Phase 1: ログ初期化（フレームワークの設定を使用）
            Log.Logger = LoggerConfigurationExtensions
                .CreateDefaultConfiguration(AppName)
                .CreateLogger();

            Log.Information("=== {AppName} Starting ===", AppName);

            // ロガーファクトリを作成
            using var loggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(Log.Logger);

            // Phase 2: シングルインスタンスチェック（フレームワーク使用）
            _instanceManager = new ApplicationInstanceManager(
                loggerFactory.CreateLogger<ApplicationInstanceManager>());

            var instanceResult = _instanceManager.TryAcquireInstance();
            if (instanceResult.IsFailure || !instanceResult.Value)
            {
                Log.Warning("Another instance is already running");
                return 1;
            }

            // Phase 3: クラッシュ検出（フレームワーク使用）
            Directory.CreateDirectory(DataDirectory);
            _crashRecoveryManager = new CrashRecoveryManager(
                loggerFactory.CreateLogger<CrashRecoveryManager>(),
                DataDirectory);

            var crashResult = _crashRecoveryManager.CheckPreviousCrash();
            if (crashResult.IsSuccess && crashResult.Value)
            {
                Log.Warning("Previous crash detected - recovery may be needed");
            }

            // クラッシュフラグを設定
            _crashRecoveryManager.SetCrashFlag();

            Log.Information("Pre-boot completed. Starting Avalonia...");

            // Avalonia アプリケーション起動
            var exitCode = BuildAvaloniaApp()
                .AfterSetup(builder =>
                {
                    if (builder.Instance is App app)
                    {
                        app.SetCrashRecoveryManager(_crashRecoveryManager);
                        app.SetInstanceManager(_instanceManager);
                    }
                })
                .StartWithClassicDesktopLifetime(args);

            // 正常終了：クラッシュフラグをクリア
            _crashRecoveryManager.ClearCrashFlag();

            Log.Information("=== Application exiting with code {ExitCode} ===", exitCode);

            return exitCode;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
        finally
        {
            _instanceManager?.Dispose();
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
