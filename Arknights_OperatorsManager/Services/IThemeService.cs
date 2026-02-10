namespace Arknights_OperatorsManager.Services;

/// <summary>
/// テーマ管理サービスのインターフェース
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// テーマを適用します
    /// </summary>
    /// <param name="themeName">テーマ名（Light/Dark/System）</param>
    void ApplyTheme(string themeName);

    /// <summary>
    /// 現在のテーマ
    /// </summary>
    string CurrentTheme { get; }
}
