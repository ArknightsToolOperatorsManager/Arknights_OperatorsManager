using llyrtkframework.Application;
using llyrtkframework.Results;
using Serilog;

namespace Arknights_OperatorsManager.Bootstrap;

/// <summary>
/// Core Init: ユーザーデータとマスターデータの読み込み
/// </summary>
public class DataLoadTask : ICoreInitTask
{
    private readonly string _dataDirectory;

    public DataLoadTask(string? dataDirectory = null)
    {
        _dataDirectory = dataDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ArknightsOperatorsManager",
            "Data");
    }

    /// <summary>
    /// ユーザーデータのファイルパス
    /// </summary>
    public string UserDataPath => Path.Combine(_dataDirectory, "user_data.json");

    /// <summary>
    /// マスターデータのファイルパス
    /// </summary>
    public string MasterDataPath => Path.Combine(_dataDirectory, "master_data.json");

    /// <summary>
    /// 初回起動かどうか
    /// </summary>
    public bool IsFirstRun { get; private set; }

    public Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Directory.CreateDirectory(_dataDirectory);

            // ユーザーデータの存在確認
            IsFirstRun = !File.Exists(UserDataPath);

            if (IsFirstRun)
            {
                Log.Information("First run detected. User data will be created on first save.");
            }
            else
            {
                Log.Information("User data found: {UserDataPath}", UserDataPath);
            }

            // マスターデータの確認
            if (!File.Exists(MasterDataPath))
            {
                Log.Information("Master data not found. Will be downloaded on first update check.");
            }
            else
            {
                Log.Information("Master data found: {MasterDataPath}", MasterDataPath);
            }

            Log.Information("Data directory initialized: {DataDirectory}", _dataDirectory);

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize data directory");
            return Task.FromResult(Result.FromException(ex, "Failed to initialize data directory"));
        }
    }
}
