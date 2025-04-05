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
    public partial class DateFilterViewModel : ObservableObject
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly FilterOperatorEnum[] SupportedFilterOperators =
        [
            FilterOperatorEnum.Before,
            FilterOperatorEnum.After,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive,
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
        ];

        private FilterOperatorEnum[] ValidOperatorsForStartDate =
        [
            FilterOperatorEnum.Before,
            FilterOperatorEnum.After,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
        ];
        
        private FilterOperatorEnum[] ValidOperatorsForEndDate =
        [
            FilterOperatorEnum.BetweenInclusive,
            FilterOperatorEnum.BetweenExclusive,
        ];

        [ObservableProperty]
        private ITranslations _translations;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        /// <summary>
        /// Translations used for the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EnumTranslation<FilterOperatorEnum>> _filterOperators = new();

        /// <summary>
        /// Selected Filter.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStartDateEnabled))]
        [NotifyPropertyChangedFor(nameof(IsEndDateEnabled))]
        private FilterOperatorEnum _selectedFilterOperator = FilterOperatorEnum.None;

        /// <summary>
        /// Start Date is only visible for these Values.
        /// </summary>
        public bool IsStartDateEnabled => ValidOperatorsForStartDate.Contains(SelectedFilterOperator);

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsEndDateEnabled => ValidOperatorsForEndDate.Contains(SelectedFilterOperator);

        /// <summary>
        /// Property Name this Filter handles.
        /// </summary>
        public readonly string PropertyName;

        public DateFilterViewModel(ITranslations translations, DateFilterDescriptor booleanFilterDescriptor)
        {
            Translations = translations;

            PropertyName = booleanFilterDescriptor.PropertyName;

            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            SelectedFilterOperator = booleanFilterDescriptor.FilterOperator;
        }

        public FilterDescriptor FilterDescriptor => new DateFilterDescriptor
        {
            FilterOperator = SelectedFilterOperator,
            PropertyName = PropertyName,
            StartDate = StartDate,
            EndDate = EndDate,
        };
    }

    /// <summary>
    /// Interaction logic for DateFilter.xaml
    /// </summary>
    public partial class DateFilter : UserControl
    {
        /// <summary>
        /// Filter State.
        /// </summary>
        public FilterState FilterState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public DateFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="filterState">Filter State</param>
        public DateFilter(string propertyName, ITranslations translations, FilterState filterState)
        {
            InitializeComponent();

            FilterState = filterState;
            ViewModel = GetFilterViewModel(propertyName, translations, filterState);

            DataContext = ViewModel;
            DataContext = this;
        }

        private DateFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, FilterState filterState)
        {
            DateFilterDescriptor booleanFilterDescriptor = GetFilterDescriptor(propertyName, filterState);

            return new DateFilterViewModel(translations, booleanFilterDescriptor);
        }

        private DateFilterDescriptor GetFilterDescriptor(string propertyName, FilterState filterState)
        {
            if (!filterState.TryGetFilter<DateFilterDescriptor>(propertyName, out var dateFilterDescriptor))
            {
                return new DateFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
                };
            }

            return dateFilterDescriptor;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            FilterState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
            ViewModel.StartDate = null;
            ViewModel.EndDate = null;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            FilterState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
