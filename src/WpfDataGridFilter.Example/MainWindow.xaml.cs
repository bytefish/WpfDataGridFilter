using System.Windows;

namespace WpfDataGridFilter.Example;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public DataGridState DataGridState { get; set; }

    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        
        ViewModel = new MainWindowViewModel();

        DataGridState = new DataGridState([]);

        DataGridState.DataGridStateChanged += (s, e) =>
        {
            ViewModel.Filter(e.DataGridState);
        };

        DataContext = this;
    }
}