namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// オペレーターマスターデータ（JSON永続化用）
/// </summary>
public class OperatorMasterData
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>オペレーターマスター（オペレーターコード → エントリ）</summary>
    public Dictionary<string, MasterOperatorEntry> Operators { get; set; } = new();
}
