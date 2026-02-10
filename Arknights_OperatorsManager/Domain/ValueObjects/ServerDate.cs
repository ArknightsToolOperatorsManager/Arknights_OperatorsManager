using System.Globalization;
using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// サーバー別の日付情報
/// </summary>
public sealed class ServerDate : ValueObject
{
    /// <summary>中国サーバーの日付（"yyyy/MM/dd" 形式の文字列）</summary>
    public string China { get; }

    /// <summary>グローバルサーバーの日付（"yyyy/MM/dd" 形式の文字列）</summary>
    public string Global { get; }

    public ServerDate(string china, string global)
    {
        China = china ?? string.Empty;
        Global = global ?? string.Empty;
    }

    /// <summary>
    /// 指定されたサーバーの日付を DateTime に変換
    /// </summary>
    /// <param name="server">サーバー名 ("China" or "Global")</param>
    /// <returns>日付（パースできない場合は null）</returns>
    public DateTime? GetDate(string server) => server switch
    {
        "China" when TryParseDate(China, out var date) => date,
        "Global" when TryParseDate(Global, out var date) => date,
        _ => null
    };

    /// <summary>
    /// 指定されたサーバーの日付が範囲内かチェック
    /// </summary>
    /// <param name="server">サーバー名 ("China" or "Global")</param>
    /// <param name="startDate">開始日</param>
    /// <param name="endDate">終了日</param>
    /// <returns>範囲内なら true</returns>
    public bool IsInRange(string server, DateTime startDate, DateTime endDate)
    {
        var date = GetDate(server);
        return date.HasValue && date.Value >= startDate && date.Value <= endDate;
    }

    private static bool TryParseDate(string dateString, out DateTime date)
    {
        // "yyyy/MM/dd" 形式をパース
        return DateTime.TryParseExact(
            dateString,
            "yyyy/MM/dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return China;
        yield return Global;
    }

    public override string ToString() => Global; // デフォルトはグローバルサーバーの日付
}
