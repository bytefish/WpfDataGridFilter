using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WpfDataGridFilter.DynamicLinq;
using WpfDataGridFilter.Example.Models;

namespace WpfDataGridFilter.Example;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Person> _people;

    [ObservableProperty]
    public int _currentPage = 1;

    [ObservableProperty]
    private int _lastPage;

    [ObservableProperty]
    private int _totalItemCount;

    [ObservableProperty]
    private List<int> _pageSizes = new() { 10, 25, 50, 100, 250 };

    [ObservableProperty]
    private int _pageSize = 25;

    [ObservableProperty]
    private DataGridState _dataGridState;

    public IRelayCommand FirstPageCommand { get; }

    public IRelayCommand PreviousPageCommand { get; }

    public IRelayCommand NextPageCommand { get; }

    public IRelayCommand LastPageCommand { get; }
    
    public void OnLoaded()
    {
        DataGridState.DataGridStateChanged += DataGridState_DataGridStateChanged;

        SetSkipTop();
    }

    public void OnUnloaded()
    {
        DataGridState.DataGridStateChanged -= DataGridState_DataGridStateChanged;
    }

    private async void DataGridState_DataGridStateChanged(object? sender, DataGridStateChangedEventArgs e)
    {
        await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
        {
            await Refresh();
        });
    }

    public MainWindowViewModel(DataGridState dataGridState)
    {
        DataGridState = dataGridState;

        People = new ObservableCollection<Person>([]);

        FirstPageCommand = new RelayCommand(() =>
        {
            CurrentPage = 1;
            SetSkipTop();
        },
        () => CurrentPage != 1);

        PreviousPageCommand = new RelayCommand(() =>
        {
            CurrentPage = CurrentPage - 1;
            SetSkipTop();
        },
        () => CurrentPage > 1);

        NextPageCommand = new RelayCommand(() =>
        {
            CurrentPage = CurrentPage + 1;
            SetSkipTop();
        },
        () => CurrentPage < LastPage);

        LastPageCommand = new RelayCommand(() =>
        {
            CurrentPage = LastPage;
            SetSkipTop();
        },
        () => CurrentPage != LastPage);
    }

    public void SetSkipTop()
    {
        DataGridState.SetSkipTop((CurrentPage - 1) * PageSize, PageSize);
    }

    public async Task Refresh()
    {
        await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
        {
            // Get the Total Count
            TotalItemCount = MockData.People
                .AsQueryable()
                .GetTotalCount(DataGridState);

            // Adjust the Page number and Page count with the Query results:
            LastPage = (TotalItemCount + PageSize - 1) / PageSize;

            // Notify all Event Handlers:
            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();

            List<Person> filteredResult = MockData.People
                    .AsQueryable()
                    .ApplyDataGridState(DataGridState)
                    .ToList();

            People = new ObservableCollection<Person>(filteredResult);
        });
    }
}
