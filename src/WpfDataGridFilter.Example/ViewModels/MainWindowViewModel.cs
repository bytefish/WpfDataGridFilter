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
