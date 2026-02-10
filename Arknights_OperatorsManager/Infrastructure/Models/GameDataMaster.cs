namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// ゲームデータマスター（ID→多言語名称変換用）
/// </summary>
public class GameDataMaster
{
    /// <summary>データバージョン</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>最終更新日時</summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>性別マスター（ID → 名称）</summary>
    public Dictionary<string, LocalizedName> Sex { get; set; } = new();

    /// <summary>職業マスター（ID → 名称）</summary>
    public Dictionary<string, LocalizedName> Class { get; set; } = new();

    /// <summary>職分マスター（クラス名 → ID → 名称）</summary>
    public Dictionary<string, Dictionary<string, LocalizedName>> SubClass { get; set; } = new();

    /// <summary>種族マスター（ID → 名称）</summary>
    public Dictionary<string, LocalizedName> Race { get; set; } = new();

    /// <summary>陣営マスター（ID → 名称）</summary>
    public Dictionary<string, LocalizedName> Faction { get; set; } = new();

    /// <summary>出身地マスター（ID → 名称）</summary>
    public Dictionary<string, LocalizedName> Place { get; set; } = new();

    /// <summary>
    /// 性別の名前を取得
    /// </summary>
    public string GetSexName(int id, string language = "ja-JP")
    {
        if (Sex.TryGetValue(id.ToString(), out var name))
            return GetLocalizedString(name, language);
        return "Unknown";
    }

    /// <summary>
    /// 職業の名前を取得
    /// </summary>
    public string GetClassName(int id, string language = "ja-JP")
    {
        if (Class.TryGetValue(id.ToString(), out var name))
            return GetLocalizedString(name, language);
        return "Unknown";
    }

    /// <summary>
    /// 職分の名前を取得
    /// </summary>
    public string GetSubClassName(string className, int id, string language = "ja-JP")
    {
        if (SubClass.TryGetValue(className, out var subClassDict))
        {
            if (subClassDict.TryGetValue(id.ToString(), out var name))
                return GetLocalizedString(name, language);
        }
        return "Unknown";
    }

    /// <summary>
    /// 種族の名前を取得
    /// </summary>
    public string GetRaceName(int id, string language = "ja-JP")
    {
        if (Race.TryGetValue(id.ToString(), out var name))
            return GetLocalizedString(name, language);
        return "Unknown";
    }

    /// <summary>
    /// 陣営の名前を取得
    /// </summary>
    public string GetFactionName(int id, string language = "ja-JP")
    {
        if (Faction.TryGetValue(id.ToString(), out var name))
            return GetLocalizedString(name, language);
        return "Unknown";
    }

    /// <summary>
    /// 出身地の名前を取得
    /// </summary>
    public string GetPlaceName(int id, string language = "ja-JP")
    {
        if (Place.TryGetValue(id.ToString(), out var name))
            return GetLocalizedString(name, language);
        return "Unknown";
    }

    /// <summary>
    /// LocalizedNameから指定言語の文字列を取得
    /// </summary>
    private string GetLocalizedString(LocalizedName name, string language)
    {
        return language switch
        {
            "ja-JP" or "ja" => name.Ja,
            "en-US" or "en" => name.En,
            "zh-CN" or "zh" or "ch" => name.Ch,
            _ => name.En
        };
    }
}
