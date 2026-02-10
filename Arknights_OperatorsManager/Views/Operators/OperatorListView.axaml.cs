using Avalonia;
using Avalonia.Controls;
using Arknights_OperatorsManager.ViewModels.Operators;
using Prism.DryIoc;
using Prism.Ioc;

namespace Arknights_OperatorsManager.Views.Operators;

public partial class OperatorListView : UserControl
{
    public OperatorListView()
    {
        InitializeComponent();

        // DataContextChanged イベントで ViewModel のコールバックを設定
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is OperatorListViewModel viewModel)
        {
            // FilterDialog を開くコールバックを設定
            viewModel.OpenFilterDialogCallback = async () =>
            {
                // Prism Application から ContainerProvider を取得
                if (Avalonia.Application.Current is not PrismApplication prismApp)
                    return;

                var containerProvider = prismApp.Container;

                var dialog = new FilterDialog();
                var filterViewModel = containerProvider.Resolve<FilterDialogViewModel>();

                dialog.DataContext = filterViewModel;

                // ダイアログを閉じるコールバックを設定
                filterViewModel.CloseDialog = (result) =>
                {
                    dialog.Close(result);
                };

                filterViewModel.OnInitialize();

                // ダイアログを表示（親ウィンドウを取得）
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window window)
                {
                    await dialog.ShowDialog(window);
                }
            };
        }
    }
}
