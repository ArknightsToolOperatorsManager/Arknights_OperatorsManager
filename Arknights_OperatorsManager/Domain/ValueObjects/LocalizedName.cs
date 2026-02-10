using llyrtkframework.Domain;

namespace Arknights_OperatorsManager.Domain.ValueObjects;

/// <summary>
/// 多言語対応の名前
/// </summary>
public sealed class LocalizedName : ValueObject
{
    /// <summary>日本語名</summary>
    public string Ja { get; }

    /// <summary>英語名</summary>
    public string En { get; }

    /// <summary>中国語名</summary>
    public string Ch { get; }

    public LocalizedName(string ja, string en, string ch)
    {
        Ja = ja ?? throw new ArgumentNullException(nameof(ja));
        En = en ?? throw new ArgumentNullException(nameof(en));
        Ch = ch ?? throw new ArgumentNullException(nameof(ch));
    }

    /// <summary>
    /// 指定された言語の名前を取得
    /// </summary>
    /// <param name="language">言語コード (ja-JP, en-US, zh-CN)</param>
    /// <returns>指定言語の名前（該当なしの場合は英語名）</returns>
    public string GetName(string language) => language switch
    {
        "ja-JP" or "ja" => Ja,
        "en-US" or "en" => En,
        "zh-CN" or "zh" or "ch" => Ch,
        _ => En
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Ja;
        yield return En;
        yield return Ch;
    }

    public override string ToString() => Ja; // デフォルトは日本語名
}
