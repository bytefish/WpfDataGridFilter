using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class DateTimeFilterControl : FilterControl
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly List<FilterOperatorEnum> SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.Before,
            FilterOperatorEnum.After,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive,
        ];

        private static List<FilterOperatorEnum> ValidOperatorsForStartDate =
        [
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.Before,
            FilterOperatorEnum.After,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive,
        ];

        private static List<FilterOperatorEnum> ValidOperatorsForEndDate =
        [
            FilterOperatorEnum.BetweenInclusive,
            FilterOperatorEnum.BetweenExclusive,
        ];

        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        DatePicker? StartDatePicker;
        DatePicker? EndDatePicker;

        #endregion Controls

        public override string PropertyName { get; set; } = string.Empty;

        public List<EnumTranslation<FilterOperatorEnum>> FilterOperators { get; private set; } = [];

        /// <summary>  
        ///  Translations
        /// </summary>
        public override ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register(
            "Translations", typeof(ITranslations), typeof(DateTimeFilterControl), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeFilterControl dateTimeFilterControl)
            {
                dateTimeFilterControl.Translations = (ITranslations)e.NewValue;
            }
        }

        /// <summary>  
        ///  FilterState of the current DataGrid.
        /// </summary>
        public override DataGridState DataGridState
        {
            get { return (DataGridState)GetValue(DataGridStateProperty); }
            set { SetValue(DataGridStateProperty, value); }
        }

        public static readonly DependencyProperty DataGridStateProperty = DependencyProperty.Register(
            "DataGridState", typeof(DataGridState), typeof(DateTimeFilterControl), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeFilterControl dateTimeFilterControl)
            {
                dateTimeFilterControl.DataGridState = (DataGridState)e.NewValue;

                dateTimeFilterControl.RefreshFilterDescriptor();
            }
        }

        /// <summary>
        /// Start Date is only visible for these Values.
        /// </summary>
        public bool IsStartDateEnabled => ValidOperatorsForStartDate.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsEndDateEnabled => ValidOperatorsForEndDate.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// Builds the FilterDescriptor described by the Control.
        /// </summary>
        public override FilterDescriptor FilterDescriptor => new DateTimeFilterDescriptor
        {
            PropertyName = PropertyName,
            FilterOperator = GetCurrentFilterOperator(),
            StartDate = StartDatePicker?.SelectedDate,
            EndDate = EndDatePicker?.SelectedDate,
        };

        private DateTimeFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<DateTimeFilterDescriptor>(propertyName, out var dateFilterDescriptor))
            {
                return new DateTimeFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
                };
            }

            return dateFilterDescriptor;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyButton = GetTemplateChild("PART_ApplyButton") as Button;
            ResetButton = GetTemplateChild("PART_ResetButton") as Button;
            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            StartDatePicker = GetTemplateChild("PART_StartDatePicker") as DatePicker;
            EndDatePicker = GetTemplateChild("PART_EndDatePicker") as DatePicker;

            FilterOperators = GetEnumTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(EnumTranslation<FilterOperatorEnum>.Translation);
                FilterOperatorsComboBox.SelectedValuePath = nameof(EnumTranslation<FilterOperatorEnum>.Value);
                FilterOperatorsComboBox.ItemsSource = GetEnumTranslations(Translations, SupportedFilterOperators);
            }

            if(ApplyButton != null)
            {
                ApplyButton.Click -= OnApplyButtonClick;
                ApplyButton.Click += OnApplyButtonClick;

                ApplyButton.Content = Translations.ApplyButton;
            }

            if (ResetButton != null)
            {
                ResetButton.Click -= OnResetButtonClick;
                ResetButton.Click += OnResetButtonClick;

                ResetButton.Content = Translations.ResetButton;
            }

            if (DataGridState != null)
            {
                RefreshFilterDescriptor();
            }

            UpdateDatePickerControls();
        }

        private void RefreshFilterDescriptor()
        {
            var dateTimeFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = dateTimeFilterDescriptor.FilterOperator;
            }

            if (StartDatePicker != null)
            {
                StartDatePicker.SelectedDate = dateTimeFilterDescriptor.StartDate;
            }

            if (EndDatePicker != null)
            {
                EndDatePicker.SelectedDate = dateTimeFilterDescriptor.EndDate;
            }

            UpdateDatePickerControls();
        }

        private List<EnumTranslation<FilterOperatorEnum>> GetEnumTranslations(ITranslations translations, List<FilterOperatorEnum> source)
        {
            List<EnumTranslation<FilterOperatorEnum>> enumTranslations = [];

            foreach (var enumValue in source)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == enumValue);

                enumTranslations.Add(translation);
            }

            return enumTranslations;
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.RemoveFilter(PropertyName);

            if(FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperatorEnum.None;
            }

            if (StartDatePicker != null)
            {
                StartDatePicker.SelectedDate = null;
            }

            if (EndDatePicker != null)
            {
                EndDatePicker.SelectedDate = null;
            }

            UpdateDatePickerControls();
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(FilterDescriptor);
        }

        private FilterOperatorEnum GetCurrentFilterOperator()
        {
            if(FilterOperatorsComboBox == null)
            {
                return FilterOperatorEnum.None;
            }

            if(FilterOperatorsComboBox.SelectedValue == null)
            {
                return FilterOperatorEnum.None;
            }

            FilterOperatorEnum currentFilterOperator = (FilterOperatorEnum) FilterOperatorsComboBox.SelectedValue;

            return currentFilterOperator;            
        }

        private void OnFilterOperatorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDatePickerControls();
        }

        private void UpdateDatePickerControls()
        {
            if (StartDatePicker != null)
            {
                StartDatePicker.IsEnabled = IsStartDateEnabled;
            }

            if (EndDatePicker != null)
            {
                EndDatePicker.IsEnabled = IsEndDateEnabled;
            }
        }
    }
}
