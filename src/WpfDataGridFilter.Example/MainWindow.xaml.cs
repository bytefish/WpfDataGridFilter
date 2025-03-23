using System.Windows;
using WpfDataGridFilter.Filters;

namespace WpfDataGridFilter.Example;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public FilterState FilterState { get; set; }

    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        
        ViewModel = new MainWindowViewModel();

        FilterState = new FilterState();

        FilterState.FilterStateChanged += (s, e) =>
        {
            ViewModel.Filter(e.FilterState);
        };

        DataContext = this;
    }
}