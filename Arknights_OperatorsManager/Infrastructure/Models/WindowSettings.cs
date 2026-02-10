namespace Arknights_OperatorsManager.Infrastructure.Models;

/// <summary>
/// ウィンドウ設定（JSON永続化用）
/// </summary>
public class WindowSettings
{
    /// <summary>ウィンドウX座標</summary>
    public int X { get; set; }

    /// <summary>ウィンドウY座標</summary>
    public int Y { get; set; }

    /// <summary>ウィンドウ幅</summary>
    public int Width { get; set; } = 1280;

    /// <summary>ウィンドウ高さ</summary>
    public int Height { get; set; } = 720;

    /// <summary>最大化フラグ</summary>
    public bool IsMaximized { get; set; }
}
