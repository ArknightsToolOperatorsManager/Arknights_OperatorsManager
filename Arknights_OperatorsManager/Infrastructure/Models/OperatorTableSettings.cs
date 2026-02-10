namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// オペレーターテーブル設定（JSON永続化用）
/// </summary>
public class OperatorTableSettings
{
    /// <summary>表示する列ID</summary>
    public List<string> VisibleColumns { get; set; } = new();

    /// <summary>ソート列ID（nullでソートなし）</summary>
    public string? SortColumn { get; set; }

    /// <summary>降順フラグ（デフォルトfalse）</summary>
    public bool SortDescending { get; set; }
}
