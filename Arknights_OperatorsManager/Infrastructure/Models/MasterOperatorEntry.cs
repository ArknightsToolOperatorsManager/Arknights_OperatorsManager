using System.Text.Json.Serialization;
using Arknights_OperatorsManager.Infrastructure.Serialization;

namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// オペレーターマスターエントリ（JSON永続化用）
/// </summary>
public class MasterOperatorEntry
{
    /// <summary>オペレーターコード（例: "LM04"）</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>オペレーター名（多言語対応）</summary>
    public LocalizedName Name { get; set; } = new();

    /// <summary>レアリティ（例: "☆6"）</summary>
    public string Rarity { get; set; } = string.Empty;

    /// <summary>レアリティの数値（☆の数をカウント）</summary>
    public int RarityValue => Rarity.Count(c => c == '☆');

    /// <summary>性別（0=男性, 1=女性）</summary>
    public int Sex { get; set; }

    /// <summary>職業</summary>
    public int Class { get; set; }

    /// <summary>職分</summary>
    public int SubClass { get; set; }

    /// <summary>種族</summary>
    public int Race { get; set; }

    /// <summary>陣営</summary>
    public int Faction { get; set; }

    /// <summary>出身地</summary>
    public int Place { get; set; }

    /// <summary>追加日（サーバー別）</summary>
    public ServerDate AddDate { get; set; } = new();

    /// <summary>パラドックスシミュレータ追加日（段階 → サーバー別日付）</summary>
    public Dictionary<string, ServerDate>? Paradox { get; set; }

    /// <summary>モジュール追加日（モジュール種別 → サーバー別日付）</summary>
    public Dictionary<string, ServerDate>? ModuleAddDates { get; set; }

    /// <summary>異格コードリスト（例: ["R112", "R113"]）</summary>
    [JsonConverter(typeof(AlternatesJsonConverter))]
    public List<string>? Alternates { get; set; }

    /// <summary>黒wikiページ番号</summary>
    public int Page { get; set; }

    /// <summary>昇進コスト [0]=E1, [1]=E2（素材ID → 数量）</summary>
    public List<Dictionary<string, int>> Elite { get; set; } = new();

    /// <summary>スキルLvコスト [0]=1→2, ..., [5]=6→7（素材ID → 数量）</summary>
    public List<Dictionary<string, int>> Skill { get; set; } = new();

    /// <summary>特化コスト {"1":[M1,M2,M3], ...}（スキル番号 → [各レベルのコスト]）</summary>
    public Dictionary<string, List<Dictionary<string, int>>> SkillMastery { get; set; } = new();

    /// <summary>モジュールコスト {"X":[Lv1,Lv2,Lv3], ...}（モジュール種別 → [各レベルのコスト]）</summary>
    public Dictionary<string, List<Dictionary<string, int>>> Modules { get; set; } = new();
}

/// <summary>
/// 多言語名称
/// </summary>
public class LocalizedName
{
    /// <summary>日本語名</summary>
    public string Ja { get; set; } = string.Empty;

    /// <summary>英語名</summary>
    public string En { get; set; } = string.Empty;

    /// <summary>中国語名</summary>
    public string Ch { get; set; } = string.Empty;
}

/// <summary>
/// サーバー別日付
/// </summary>
public class ServerDate
{
    /// <summary>中国サーバーの日付</summary>
    public string China { get; set; } = string.Empty;

    /// <summary>グローバルサーバーの日付</summary>
    public string Global { get; set; } = string.Empty;
}
