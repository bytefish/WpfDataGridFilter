using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfDataGridFilter.DynamicLinq;
using WpfDataGridFilter.Example.Models;

namespace WpfDataGridFilter.Example;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Person> _people;

    public MainWindowViewModel()
    {
        People = new ObservableCollection<Person>(MockData.People);
    }

    public void Filter(DataGridState dataGridState)
    {
        // Get the Count with Filters
        List<Person> filteredResult = MockData.People
                .AsQueryable()
                .ApplyDataGridState(dataGridState)
                .ToList();

        People = new ObservableCollection<Person>(filteredResult);
    }
}
