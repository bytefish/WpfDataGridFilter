# WpfDataGridFilter #

"I just want to filter some data in a `DataGrid`, why is all this so complicated?"... said everyone using a WPF `DataGrid`.

This library makes it possible to add filters for each column of a `DataGrid`, and use them to provide Filtering.

## What's included

When a Filter has been added to the a DataGrid column it looks like this:

![Filter Header Overview](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-datagridcolumnheader.jpg?raw=true)

By clicking on the Filter Symbol, a Popup for the given Filter Type is shown:

![Filter Popup Opened](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-popup.jpg?raw=true)

You can use several Operators for Filtering the Data. It depends on the Filter Type:

![Filter Popup Opened With Filter Operators](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-operator-list.jpg?raw=true)

## Using it:

You just need to add a `FilterableDataGridColumnHeader` as the `DataGridColumn.Header` for the Properties we want to Filter:

```xml
<DataGrid ItemsSource="{Binding ViewModel.People}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Binding="{Binding PersonID}">
            <DataGridTextColumn.Header>
                <wpfdatagridfilter:FilterableDataGridColumnHeader FilterState="{Binding FilterState}" HeaderText="PersonID" PropertyName="PersonID" Height="30" FilterType="IntNumericFilter"></wpfdatagridfilter:FilterableDataGridColumnHeader>
            </DataGridTextColumn.Header>
        </DataGridTextColumn>
        <DataGridTextColumn Binding="{Binding FullName}">
            <DataGridTextColumn.Header>
                <wpfdatagridfilter:FilterableDataGridColumnHeader FilterState="{Binding FilterState}" HeaderText="FullName" PropertyName="FullName" Height="30" FilterType="StringFilter"></wpfdatagridfilter:FilterableDataGridColumnHeader>
            </DataGridTextColumn.Header>
        </DataGridTextColumn>
    </DataGrid.Columns>
</DataGrid>
```

We can then subscribe to changes on a `FilterState` like this:

```csharp
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
```

And here is how to use the `FilterState` in a ViewModel:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfDataGridFilter.DynamicLinq;
using WpfDataGridFilter.Example.Models;
using WpfDataGridFilter.Filters;
using System.Linq.Dynamic.Core;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Example;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Person> _people;

    public MainWindowViewModel()
    {
        People = new ObservableCollection<Person>(MockData.People);
    }

    public void Filter(FilterState filterState)
    {
        List<FilterDescriptor> filters = filterState.Filters.Values.ToList();

        InternalFilter(filters);
    }

    public void InternalFilter(List<FilterDescriptor> filters)
    {
        try
        {
            string dynamicLinqQuery = DynamicLinqConverter.Translate(filters);

            IQueryable<Person> query = MockData.People.AsQueryable();

            if (!string.IsNullOrWhiteSpace(dynamicLinqQuery))
            {
                query = query.Where(dynamicLinqQuery);
            }

            People = new ObservableCollection<Person>(query.ToList());
        }
        catch (Exception e)
        {
            // Some great Pokemon Exception Handling here ...
        }
    }
}
```