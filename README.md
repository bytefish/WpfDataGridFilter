# WpfDataGridFilter #

"I just want to filter some data in a DataGrid, why is all this so complicated?"... said everyone using a WPF DataGrid.

This library simplifies adding server-side filtering, pagination and sorting to a WPF DataGrid.

## What's included

The library allows to add a Custom DataGridHeader to each column of a Grid:

![Filter Column](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-datagridcolumnheader.jpg?raw=true)

By clicking on the Filter Symbol, a Popup appears with the current Filter for the Column:

![Filter Popup](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-operator-list.jpg?raw=true)

## Using it

You start by adding the `WpfDataGridFilter` Styles to your `App.xaml`, like this:

```
<Application x:Class="WpfDataGridFilter.Example.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:WpfDataGridFilter.Example" 
             xmlns:wpfdatagridfilter="clr-namespace:WpfDataGridFilter.Controls;assembly=WpfDataGridFilter"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/WpfDataGridFilter;component/WpfDataGridFilter.xaml" />
                <ResourceDictionary Source="pack://application:,,,/WpfDataGridFilter;component/Theme/Light.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>

```

You could then make a `DataGridColumn` filterable by using the `FilterableColumnHeader` Control as the Header Template for 
a Grid Column. We also need to set the `CanUserSortColumns` property to `False`, so we avoid invoking the built-in client-side 
DataGrid filtering.

We are also using the `DataGridState`, which comes with the `WpfDataGridFilter` library, so we can alter the current state of the 
DataGrid ("Which Filters are applied? What's the current Sort Column? What's the current page? ...").  The example below also 
shows how to use the Pagination Control.

```xml
<Window x:Class="WpfDataGridFilter.Example.MainWindow" 
    x:Name="MainWindowRoot"
    Loaded="Window_Loaded"
    Unloaded="Window_Unloaded">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <!-- The Data Grid -- >
        <DataGrid ItemsSource="{Binding ViewModel.People}" AutoGenerateColumns="False" CanUserSortColumns="False" MinColumnWidth="150">
                <DataGrid.Columns>
                    <DataGridTextColumn Binding="{Binding PersonID}">
                        <DataGridTextColumn.HeaderTemplate>
                            <ItemContainerTemplate>
                                <wpfdatagridfilter:FilterableColumnHeader DataGridState="{Binding ViewModel.DataGridState, ElementName=MainWindowRoot}" HeaderText="PersonID" PropertyName="PersonID" Height="40" MinWidth="150" FilterType="IntNumericFilter"></wpfdatagridfilter:FilterableColumnHeader>
                            </ItemContainerTemplate>
                        </DataGridTextColumn.HeaderTemplate>
                    </DataGridTextColumn>
                </DataGrid.Columns>
        </DataGrid>
        <!-- ... -->
          <Grid Grid.Row="1" Margin="10">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>

            <wpfdatagridfilter:PaginationControl 
                Grid.Column="0"
                HorizontalAlignment="Center"
                SelectedPageSize="{Binding ViewModel.PageSize, Mode=TwoWay}"
                PageSizes="{Binding ViewModel.PageSizes}"
                CurrentPage="{Binding ViewModel.CurrentPage}"
                FirstPage="{Binding ViewModel.FirstPageCommand}"
                PreviousPage="{Binding ViewModel.PreviousPageCommand}"
                NextPage="{Binding ViewModel.NextPageCommand}"
                LastPage="{Binding ViewModel.LastPageCommand}" />

            <TextBlock Width="150" Grid.Column="0"  HorizontalAlignment="Right">
                <Run Text="Page" />
                <Run Text="{Binding ViewModel.CurrentPage, Mode=OneWay}" d:Text="0" />
                <Run Text="/" />
                <Run Text="{Binding ViewModel.LastPage, Mode=OneWay}" d:Text="0" />
                <LineBreak />
                <Run Text="Number of Elements:"></Run>
                <Run Text="{Binding ViewModel.TotalItemCount, Mode=OneWay}" d:Text="1020" />
            </TextBlock>

        </Grid> 
        
    </Grid>

</Window>
```

In the Code-Behind of the Page we create a new ViewModel with a fresh `DataGridState` and call the Page Loaded and Unloaded Events, 
so we could Subscribe and Unsubscribe to the DataGridState Change Events in the ViewModel: 

```csharp
using System.Windows;

namespace WpfDataGridFilter.Example;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        ViewModel = new MainWindowViewModel(new DataGridState([]));
        DataContext = this;

        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnLoaded();
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnUnloaded();
    }
}
```

Now the ViewModel we just need to connect all pieces and load the Data off our Data Source. In this example 
I have used the `WpfDataGridFilter.DynamicLinq`, which provides the functionality to apply a `DataGridState` 
on an `IQueryable<T>`.

```csharp
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
    private DataGridState _dataGridState;

    [ObservableProperty]
    public int _currentPage = 1;

    public int LastPage => ((TotalItemCount - 1) / PageSize) + 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LastPage))]
    private int _totalItemCount;

    [ObservableProperty]
    private List<int> _pageSizes = new() { 10, 25, 50, 100, 250 };

    private int _pageSize = 25;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if(SetProperty(ref _pageSize, value))
            {
                // We could also calculate the page, that contains 
                // the current element, but it's better to just set 
                // it to 1 I think.
                CurrentPage = 1;
                
                // The Last Page has changed, so we can update the 
                // UI. The Last Page is also used to determine the 
                // bounds.
                OnPropertyChanged(nameof(LastPage));

                // Update the Page.
                SetSkipTop();
            }
        }
    }

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
            // If there's no Page Size, we don't need to load anything.
            if(PageSize == 0)
            {
                return;
            }

            // Get the Total Count, so we can update the First and Last Page.
            TotalItemCount = MockData.People
                .AsQueryable()
                .GetTotalItemCount(DataGridState);

            // If our current page is not beyond the last Page, we'll need to rerequest data. At
            // the moment this is going to trigger yet another query for the Count. Obviously that's
            // a big TODO for a better implementation.
            if (CurrentPage > 0 && CurrentPage > LastPage)
            {
                // If the number of items has reduced such that the current page index is no longer valid, move
                // automatically to the final valid page index and trigger a further data load.
                CurrentPage = LastPage;

                SetSkipTop();

                return;
            }

            // Notify all Event Handlers, so we can enable or disable the 
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
```

## Conclusion

And that's it. It's not a perfect implementation yet, but it is a starting point!

This is an Open Source Project, so feel free to contribute.