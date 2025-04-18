using System.Windows;
using WpfDataGridFilter.Filters;

namespace WpfDataGridFilter.Example;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public DataGridState FilterState { get; set; }

    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        
        ViewModel = new MainWindowViewModel();

        FilterState = new DataGridState();

        FilterState.DataGridStateChanged += (s, e) =>
        {
            ViewModel.Filter(e.DataGridState);
        };

        DataContext = this;
    }
}