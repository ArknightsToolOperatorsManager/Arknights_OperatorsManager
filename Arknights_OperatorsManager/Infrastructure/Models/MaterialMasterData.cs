namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// 素材マスターデータ（JSON永続化用）
/// </summary>
public class MaterialMasterData
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>サーバー種別（global/cn）</summary>
    public string Server { get; set; } = "global";

    /// <summary>素材マスター（素材ID → エントリ）</summary>
    public Dictionary<string, MasterMaterialEntry> Materials { get; set; } = new();
}
