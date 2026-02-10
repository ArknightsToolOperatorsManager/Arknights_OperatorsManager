namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// ユーザーのオペレーターエントリ（JSON永続化用）
/// </summary>
public class UserOperatorEntry
{
    /// <summary>現在の育成状態</summary>
    public OperatorStateData CurrentState { get; set; } = new();

    /// <summary>目標の育成状態</summary>
    public OperatorStateData TargetState { get; set; } = new();

    /// <summary>潜在（0-6、0=未所持）</summary>
    public int Potential { get; set; }

    /// <summary>信頼度（0-200）</summary>
    public int Trust { get; set; }

    /// <summary>逆理演算クリア済み</summary>
    public bool ParadoxCleared { get; set; }

    /// <summary>グループ名</summary>
    public string? Group { get; set; }

    /// <summary>優先度</summary>
    public int Priority { get; set; }

    /// <summary>メモ</summary>
    public string? Memo { get; set; }
}
