using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    /// <summary>
    /// ViewModel.
    /// </summary>
    public partial class DateTimeFilterViewModel : ObservableObject
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly FilterOperatorEnum[] SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
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
        [NotifyPropertyChangedFor(nameof(IsApplyButtonEnabled))]
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
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsApplyButtonEnabled => SelectedFilterOperator != FilterOperatorEnum.None;

        /// <summary>
        /// Property Name this Filter handles.
        /// </summary>
        public readonly string PropertyName;

        public DateTimeFilterViewModel(ITranslations translations, DateTimeFilterDescriptor dateTimeFilterDescriptor)
        {
            Translations = translations;

            PropertyName = dateTimeFilterDescriptor.PropertyName;
            StartDate = dateTimeFilterDescriptor.StartDate;
            EndDate = dateTimeFilterDescriptor.EndDate;

            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            SelectedFilterOperator = dateTimeFilterDescriptor.FilterOperator;
        }

        public FilterDescriptor FilterDescriptor => new DateTimeFilterDescriptor
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
    public partial class DateTimeFilter : UserControl
    {
        /// <summary>
        /// Filter State.
        /// </summary>
        public DataGridState DataGridState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public DateTimeFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="filterState">Filter State</param>
        public DateTimeFilter(string propertyName, ITranslations translations, DataGridState filterState)
        {
            InitializeComponent();

            DataGridState = filterState;
            ViewModel = GetFilterViewModel(propertyName, translations, filterState);

            DataContext = ViewModel;
            DataContext = this;
        }

        private DateTimeFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, DataGridState dataGridState)
        {
            DateTimeFilterDescriptor dateTimeFilterDescriptor = GetFilterDescriptor(propertyName, dataGridState);

            return new DateTimeFilterViewModel(translations, dateTimeFilterDescriptor);
        }

        private DateTimeFilterDescriptor GetFilterDescriptor(string propertyName, DataGridState filterState)
        {
            if (!filterState.TryGetFilter<DateTimeFilterDescriptor>(propertyName, out var dateFilterDescriptor))
            {
                return new DateTimeFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
                };
            }

            return dateFilterDescriptor;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            DataGridState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
            ViewModel.StartDate = null;
            ViewModel.EndDate = null;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
