using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Filters.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Filters.Controls
{
    /// <summary>
    /// ViewModel.
    /// </summary>
    public partial class StringFilterViewModel : ObservableObject
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly FilterOperatorEnum[] SupportedFilterOperators =
        [
            FilterOperatorEnum.IsEmpty,
            FilterOperatorEnum.IsNotEmpty,
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.Contains,
            FilterOperatorEnum.NotContains,
            FilterOperatorEnum.StartsWith,
            FilterOperatorEnum.EndsWith,
        ];

        [ObservableProperty]
        private string? _value;

        /// <summary>
        /// Translations used for the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EnumTranslation<FilterOperatorEnum>> _filterOperators = new();

        /// <summary>
        /// Property Name this Filter handles.
        /// </summary>
        public readonly string PropertyName;

        public StringFilterViewModel(ITranslations translations, StringFilterDescriptor stringFilterDescriptor)
        {
            PropertyName = stringFilterDescriptor.PropertyName;
            Value = stringFilterDescriptor.Value;

            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            SelectedFilterOperator = stringFilterDescriptor.FilterOperator;
        }

        [ObservableProperty]
        private FilterOperatorEnum _selectedFilterOperator = FilterOperatorEnum.None;

        public FilterDescriptor FilterDescriptor => new StringFilterDescriptor
        {
            FilterOperator = SelectedFilterOperator,
            PropertyName = PropertyName,
            Value = Value
        };
    }

    /// <summary>
    /// Interaction logic for StringFilter.xaml
    /// </summary>
    public partial class StringFilter : UserControl
    {
        /// <summary>
        /// Filter State.
        /// </summary>
        public FilterState FilterState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public StringFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="filterState">Filter State</param>
        public StringFilter(string propertyName, ITranslations translations, FilterState filterState)
        {
            InitializeComponent();

            FilterState = filterState;
            ViewModel = GetFilterViewModel(propertyName, translations, filterState);

            DataContext = ViewModel;
            DataContext = this;
        }

        private StringFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, FilterState filterState)
        {
            StringFilterDescriptor booleanFilterDescriptor = GetFilterDescriptor(propertyName, filterState);

            return new StringFilterViewModel(translations, booleanFilterDescriptor);
        }

        private StringFilterDescriptor GetFilterDescriptor(string propertyName, FilterState filterState)
        {
            if (!filterState.TryGetFilter<StringFilterDescriptor>(propertyName, out var stringFilterDescriptor))
            {
                return new StringFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None,
                    Value = null
                };
            }

            return stringFilterDescriptor;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            FilterState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
            ViewModel.Value = null;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            FilterState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
