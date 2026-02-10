namespace Arknights_OperatorsManager.ViewModels.Operators;

/// <summary>
/// 日付フィルターの範囲
/// </summary>
public class DateRange
{
    /// <summary>開始日</summary>
    public DateTime StartDate { get; set; }

    /// <summary>終了日</summary>
    public DateTime EndDate { get; set; }

    /// <summary>サーバー名 ("China" or "Global")</summary>
    public string Server { get; set; } = "Global";

    public DateRange()
    {
        StartDate = DateTime.MinValue;
        EndDate = DateTime.MaxValue;
    }

    public DateRange(DateTime startDate, DateTime endDate, string server)
    {
        StartDate = startDate;
        EndDate = endDate;
        Server = server ?? "Global";
    }

    /// <summary>
    /// ディープコピーを作成
    /// </summary>
    public DateRange Clone() => new(StartDate, EndDate, Server);
}
