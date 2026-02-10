namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// オペレーターユーザーデータ（JSON永続化用）
/// </summary>
public class OperatorUserData
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>オペレーターデータ（オペレーターID → エントリ）</summary>
    public Dictionary<string, UserOperatorEntry> Operators { get; set; } = new();
}
