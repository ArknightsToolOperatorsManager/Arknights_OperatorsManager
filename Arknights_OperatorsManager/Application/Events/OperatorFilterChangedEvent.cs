using Arknights_OperatorsManager.ViewModels.Operators;
using Prism.Events;

namespace Arknights_OperatorsManager.Application.Events;

/// <summary>
/// オペレーターフィルター条件変更イベント
/// </summary>
public class OperatorFilterChangedEvent : PubSubEvent<FilterCondition>
{
}
