namespace Arknights_OperatorsManager.Application.State;

/// <summary>
/// アプリケーション全体で共有するグローバル状態
/// </summary>
public class AppGlobalState
{
    /// <summary>
    /// 現在適用中のテーマ（Light/Dark/System）
    /// </summary>
    public string CurrentTheme { get; set; } = "System";

    /// <summary>
    /// 現在の言語（ja-JP/en-US/zh-CN）
    /// </summary>
    public string CurrentLanguage { get; set; } = "ja-JP";

    /// <summary>
    /// Tips表示のON/OFF
    /// </summary>
    public bool ShowTips { get; set; } = true;

    /// <summary>
    /// 素材計画機能の有効/無効
    /// </summary>
    public bool UseMaterialPlanFeature { get; set; } = true;

    /// <summary>
    /// 大陸版情報を表示（OFF時はグローバル版実装日を基準にフィルタ）
    /// </summary>
    public bool ShowChinaServerData { get; set; } = false;

    /// <summary>
    /// 選択中のオペレーターID（nullは未選択）
    /// </summary>
    public string? SelectedOperatorId { get; set; }

    /// <summary>
    /// 素材計算中フラグ
    /// </summary>
    public bool IsCalculatingMaterials { get; set; }
}
