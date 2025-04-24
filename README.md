# WpfDataGridFilter

[WpfDataGridFilter]: https://github.com/bytefish/WpfDataGridFilter

"I just want to filter some data in a DataGrid, why is all this so complicated?"... said everyone using a WPF DataGrid. 

So I have written [WpfDataGridFilter], which is a small library to simplify server-side filtering, pagination and sorting 
in a WPF DataGrid. It works by using a custom `DataGridColumnHeader` template and comes with its own Pagination control and 
filtering infrastructure.

All code can be found in a Git repository at:

* [https://github.com/bytefish/WpfDataGridFilter](https://github.com/bytefish/WpfDataGridFilter)

## The Problem ##

The WPF `DataGrid` control is a powerful component, but it lacks very basic features, such as filtering and pagination for 
data. And while it's somewhat easy to add client-side sorting and filtering using a `CollectionViewSource`, doing server-side 
processing required all kinds of hacks.

I have a week off, so I have written a small library to make it easier.

## What we are going to build ##

The idea is to provide a custom `DataGridColumnHeader` control, that enables us to filter the content in a `DataGrid` column:

<div style="display:flex; align-items:center; justify-content:center;margin-bottom:15px;">
    <a href="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-datagridcolumnheader.jpg">
        <img src="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-datagridcolumnheader.jpg" alt="WPF Filter and Pagination Control">
    </a>
</div>

The Filter Symbol turns red for filtered columns.

If you click on the Filter Symbol in the DataGrid Header, a Popup with a Filter Control is shown:

<div style="display:flex; align-items:center; justify-content:center;margin-bottom:15px;">
    <a href="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-opened.jpg">
        <img src="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-opened.jpg" alt="WPF Filter Control Popup">
    </a>
</div>

The Filter Controls support several Filter Operators, based on their Type. For a `StringFilter`, it looks like this:

<div style="display:flex; align-items:center; justify-content:center;margin-bottom:15px;">
    <a href="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-operator-list.jpg">
        <img src="https://raw.githubusercontent.com/bytefish/WpfDataGridFilter/refs/heads/main/doc/filter-operator-list.jpg" alt="WPF Filter Operators">
    </a>
</div>

## Using It ##

You start by adding the [WpfDataGridFilter] to your project using the NuGet package:

```
dotnet package add WpfDataGridFilter
```

And you should also add the Dynamic LINQ Plugin to simplify working with data:

```
dotnet package add WpfDataGridFilter.DynamicLinq
```

Built-in the following filter types for a column are supported:

* `BooleanFilter`
* `DateTimeFilter`
* `IntNumericFilter`
* `DoubleNumericFilter`
* `StringFilter`

The core concept to understand is the notion of a `DataGridState`, which holds the entire state for your Data Grid. This 
includes pagination information, filters and the sort column. We can register to `DataGridState` changes and query the data 
on changes.

So imagine, we are building a small application for filtering people. To have some anonymized data, we'll load it 
from a CSV file, distributed with the repository:

```csharp
using System.IO;

namespace WpfDataGridFilter.Example.Models;

public class Person
{
    public required int PersonID { get; set; }

    public required string FullName { get; set; }

    public required string PreferredName { get; set; }
    
    public required string SearchName { get; set; }
    
    public required string IsPermittedToLogon { get; set; }

    public required string? LogonName { get; set; }

    public required bool IsExternalLogonProvider { get; set; }

    public required bool IsSystemUser { get; set; }

    public required bool IsEmployee { get; set; }

    public required bool IsSalesperson { get; set; }

    public required string? PhoneNumber { get; set; }

    public required string? FaxNumber { get; set; }

    public required string? EmailAddress { get; set; }

    public required DateTime ValidFrom { get; set; }

    public required DateTime ValidTo { get; set; }
}

public static class MockData
{
    /// <summary>
    /// Mock Data File Path.
    /// </summary>
    public static readonly string CsvFilename = Path.Combine(AppContext.BaseDirectory, "Assets", "people.csv");

    /// <summary>
    /// Mock Data.
    /// </summary>
    public static List<Person> People = CsvReader.GetFromFile(CsvFilename);
}

public static class CsvReader
{
    public static List<Person> GetFromFile(string path)
    {
        return File.ReadLines(path)
            .Skip(1) // Skip Header
            .Select(x => x.Split(',')) // Split into Components
            .Select(x => Convert(x)) // Convert to the C# class
            .ToList();
    }

    public static Person Convert(string[] values)
    {
        return new Person
        {
            PersonID = int.Parse(values[0]),
            FullName = values[1],
            PreferredName = values[2],
            SearchName = values[3],
            IsPermittedToLogon = values[4],
            LogonName = values[5],
            IsExternalLogonProvider = int.Parse(values[6]) == 1 ? true : false,
            IsSystemUser = int.Parse(values[7]) == 1 ? true : false,
            IsEmployee = int.Parse(values[8]) == 1 ? true : false,
            IsSalesperson = int.Parse(values[9]) == 1 ? true : false,
            PhoneNumber = values[10],
            FaxNumber = values[11],
            EmailAddress = values[12],
            ValidFrom = DateTime.Parse(values[13]),
            ValidTo = DateTime.Parse(values[14]),
        };
    }
}
```

Then in the `App.xaml` you are adding the Resources for the Control Styles:

```xml
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

To add a Filter Header for the `FullName` for the `FullName` Property, we just need to add a `FilterableColumnHeader` like this:

```xml
<Window x:Name="MainWindowRoot">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <DataGrid ItemsSource="{Binding ViewModel.People}" AutoGenerateColumns="False" CanUserSortColumns="False" MinColumnWidth="150">
            <DataGrid.Columns>
                <DataGridTextColumn Binding="{Binding PersonID}">
                    <DataGridTextColumn.HeaderTemplate>
                        <ItemContainerTemplate>
                            <wpfdatagridfilter:FilterableColumnHeader 
                                DataGridState="{Binding ViewModel.DataGridState, ElementName=MainWindowRoot}" 
                                HeaderText="PersonID" 
                                PropertyName="PersonID" 
                                Height="40" 
                                MinWidth="150" 
                                FilterType="IntNumericFilter" />
                        </ItemContainerTemplate>
                    </DataGridTextColumn.HeaderTemplate>
                </DataGridTextColumn>
            </DataGrid.Columns>
        </DataGrid>        
    </Grid>
</Window>
```

In the Code-Behind we are creating a View Model and pass a new `DataGridState` to it:

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

And in the View Model we can register to the `DataGridStateChanged` event and refresh the 
data accordingly. The full example also shows how to add pagination, this code-snippet shows 
the most basic use-case:

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

    // ...
    
    public void OnLoaded()
    {
        DataGridState.DataGridStateChanged += DataGridState_DataGridStateChanged;
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
    }

    public async Task Refresh()
    {
        await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
        {
            // Pagination, ...
            
            List<Person> filteredResult = MockData.People
                    .AsQueryable()
                    .ApplyDataGridState(DataGridState)
                    .ToList();

            People = new ObservableCollection<Person>(filteredResult);
        });
    }
}
```

And that's it!

## Adding Pagination

Something that almost always comes up is some sort of Pagination. You could use the `PaginationControl` coming with the library.

Here is the XAML, that shows how to use the PaginationControl in a MVVM application:

```xaml
<Window x:Class="WpfDataGridFilter.Example.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfDataGridFilter.Example" 
        xmlns:wpfdatagridfilter="clr-namespace:WpfDataGridFilter.Controls;assembly=WpfDataGridFilter" 
        xmlns:models="clr-namespace:WpfDataGridFilter.Example.Models"
        mc:Ignorable="d"
        Loaded="Window_Loaded"
        Unloaded="Window_Unloaded"
        Title="MainWindow" Height="450" Width="800"
        x:Name="MainWindowRoot">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
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

And in the Code-Behind you can see, how we could use the `DataGridState` and the `PaginationControl` for requesting paginated data:

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

## Adding your own FilterControls ##

If the existing Filter Controls don't suit your needs, you can add your own Controls. All you have to do is 
implementing the abstract base class `FilterControl` and register it in the `FilterControlProvider` (or add 
your own `IFilterControlProvider` implementation).

## Conclusion ##

And that's it. 

I think it's a nice library for adding filtering, pagination and sorting to a `DataGrid`, without having to 
dive into `DataGrid` internals and rather just use it by adding a Header Template.

The implementation? Yes, it is far from perfect... due to my lack of experience writing custom WPF 
controls. But I hope this library could be a starting point for collaboration and improving it.