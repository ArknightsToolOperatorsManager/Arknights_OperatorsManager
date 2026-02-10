namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// UI設定（JSON永続化用）
/// </summary>
public class UiSettings
{
    /// <summary>オペレーターテーブル設定</summary>
    public OperatorTableSettings OperatorTable { get; set; } = new();

    /// <summary>素材テーブル設定</summary>
    public MaterialTableSettings MaterialTable { get; set; } = new();

    /// <summary>ウィンドウ状態</summary>
    public WindowSettings Window { get; set; } = new();

    /// <summary>Tips表示ON/OFF（デフォルトtrue）</summary>
    public bool ShowTips { get; set; } = true;

    /// <summary>素材計画機能の有効/無効（デフォルトtrue）</summary>
    public bool UseMaterialPlanFeature { get; set; } = true;

    /// <summary>大陸版情報を表示（OFF時はグローバル版実装日を基準にフィルタ）</summary>
    public bool ShowChinaServerData { get; set; } = false;

    /// <summary>フィルター設定</summary>
    public FilterSettings Filter { get; set; } = new();
}
