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
        private ITranslations _translations;

        [ObservableProperty]
        private string? _value;

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

        public StringFilterViewModel(ITranslations translations, StringFilterDescriptor stringFilterDescriptor)
        {
            Translations = translations;

            PropertyName = stringFilterDescriptor.PropertyName;
            Value = stringFilterDescriptor.Value;

            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            SelectedFilterOperator = stringFilterDescriptor.FilterOperator;
        }

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
        public DataGridState DataGridState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public StringFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="dataGridState">Filter State</param>
        public StringFilter(string propertyName, ITranslations translations, DataGridState dataGridState)
        {
            InitializeComponent();

            DataGridState = dataGridState;
            ViewModel = GetFilterViewModel(propertyName, translations, dataGridState);

            DataContext = ViewModel;
            DataContext = this;
        }

        private StringFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, DataGridState filterState)
        {
            StringFilterDescriptor booleanFilterDescriptor = GetFilterDescriptor(propertyName, filterState);

            return new StringFilterViewModel(translations, booleanFilterDescriptor);
        }

        private StringFilterDescriptor GetFilterDescriptor(string propertyName, DataGridState filterState)
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
            DataGridState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
            ViewModel.Value = null;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
