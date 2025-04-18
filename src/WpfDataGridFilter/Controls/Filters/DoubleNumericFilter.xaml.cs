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
    public partial class DoubleNumericFilterViewModel : ObservableObject
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly FilterOperatorEnum[] SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.IsGreaterThan,
            FilterOperatorEnum.IsGreaterThanOrEqualTo,
            FilterOperatorEnum.IsLessThan,
            FilterOperatorEnum.IsLessThanOrEqualTo,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive
        ];

        [ObservableProperty]
        private ITranslations _translations;

        [ObservableProperty]
        private double? _lowerValue;

        [ObservableProperty]
        private double? _upperValue;

        /// <summary>
        /// Translations used for the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EnumTranslation<FilterOperatorEnum>> _filterOperators = new();

        /// <summary>
        /// Currently Selected Filter Operator.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsApplyButtonEnabled))]
        private FilterOperatorEnum _selectedFilterOperator = FilterOperatorEnum.None;

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsApplyButtonEnabled => SelectedFilterOperator != FilterOperatorEnum.None;

        /// <summary>
        /// Property Name this Filter handles.
        /// </summary>
        public readonly string PropertyName;

        public DoubleNumericFilterViewModel(ITranslations translations, DoubleNumericFilterDescriptor doubleNumericFilterDescriptor)
        {
            Translations = translations;

            PropertyName = doubleNumericFilterDescriptor.PropertyName;
            LowerValue = doubleNumericFilterDescriptor.LowerValue;
            UpperValue = doubleNumericFilterDescriptor.UpperValue;

            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            SelectedFilterOperator = doubleNumericFilterDescriptor.FilterOperator;
        }

        public FilterDescriptor FilterDescriptor => new DoubleNumericFilterDescriptor
        {
            FilterOperator = SelectedFilterOperator,
            PropertyName = PropertyName,
            LowerValue = LowerValue,
            UpperValue = UpperValue,
        };
    }

    /// <summary>
    /// Interaction logic for DoubleNumericFilter.xaml
    /// </summary>
    public partial class DoubleNumericFilter : UserControl
    {
        /// <summary>
        /// Filter State.
        /// </summary>
        public DataGridState FilterState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public DoubleNumericFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="filterState">Filter State</param>
        public DoubleNumericFilter(string propertyName, ITranslations translations, DataGridState filterState)
        {
            InitializeComponent();

            FilterState = filterState;
            ViewModel = GetFilterViewModel(propertyName, translations, filterState);

            DataContext = ViewModel;
            DataContext = this;
        }

        private DoubleNumericFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, DataGridState filterState)
        {
            DoubleNumericFilterDescriptor intNumericFilterDescriptor = GetFilterDescriptor(propertyName, filterState);

            return new DoubleNumericFilterViewModel(translations, intNumericFilterDescriptor);
        }

        private DoubleNumericFilterDescriptor GetFilterDescriptor(string propertyName, DataGridState filterState)
        {
            if (!filterState.TryGetFilter<DoubleNumericFilterDescriptor>(propertyName, out var dateFilterDescriptor))
            {
                return new DoubleNumericFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None,
                    LowerValue = null,
                    UpperValue = null,
                };
            }

            return dateFilterDescriptor;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            FilterState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
            ViewModel.LowerValue = null;
            ViewModel.UpperValue = null;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            FilterState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
