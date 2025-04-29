// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class DateTimeFilterControl : BaseFilterControl<DateTimeFilterDescriptor>
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly List<FilterOperator> SupportedFilterOperators =
        [
            FilterOperator.None,
            FilterOperator.IsNull,
            FilterOperator.IsNotNull,
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.Before,
            FilterOperator.After,
            FilterOperator.BetweenExclusive,
            FilterOperator.BetweenInclusive,
        ];

        private static List<FilterOperator> ValidOperatorsForStartDate =
        [
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.Before,
            FilterOperator.After,
            FilterOperator.BetweenExclusive,
            FilterOperator.BetweenInclusive,
        ];

        private static List<FilterOperator> ValidOperatorsForEndDate =
        [
            FilterOperator.BetweenInclusive,
            FilterOperator.BetweenExclusive,
        ];

        #region Controls 

        private ComboBox? FilterOperatorsComboBox;

        private DatePicker? StartDatePicker;

        private DatePicker? EndDatePicker;

        #endregion Controls

        public List<Translation<FilterOperator>> FilterOperators { get; private set; } = [];

        /// <summary>
        /// Start Date is only visible for these Values.
        /// </summary>
        public bool IsStartDateEnabled => ValidOperatorsForStartDate.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsEndDateEnabled => ValidOperatorsForEndDate.Contains(GetCurrentFilterOperator());

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            StartDatePicker = GetTemplateChild("PART_StartDatePicker") as DatePicker;
            EndDatePicker = GetTemplateChild("PART_EndDatePicker") as DatePicker;

            FilterOperators = GetFilterOperatorTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(Translation<FilterOperator>.Text);
                FilterOperatorsComboBox.SelectedValuePath = nameof(Translation<FilterOperator>.Value);
                FilterOperatorsComboBox.ItemsSource = GetFilterOperatorTranslations(Translations, SupportedFilterOperators);
            }

            if (DataGridState != null)
            {
                OnDataGridStateChanged();
            }

            UpdateDatePickerControls();
        }

        private FilterOperator GetCurrentFilterOperator()
        {
            if (FilterOperatorsComboBox == null)
            {
                return FilterOperator.None;
            }

            if (FilterOperatorsComboBox.SelectedValue == null)
            {
                return FilterOperator.None;
            }

            FilterOperator currentFilterOperator = (FilterOperator)FilterOperatorsComboBox.SelectedValue;

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

        protected override void OnDataGridStateChanged()
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

        protected override void OnApplyFilter()
        {
            // TODO
        }

        protected override void OnResetFilter()
        {
            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperator.None;
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

        protected override DateTimeFilterDescriptor GetDefaultFilterDescriptor()
        {
            return new DateTimeFilterDescriptor
            {
                FilterOperator = FilterOperator.None,
                PropertyName = PropertyName,
                StartDate = null,
                EndDate = null,
            };
        }

        protected override FilterDescriptor GetFilterDescriptor()
        {
            return new DateTimeFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = GetCurrentFilterOperator(),
                StartDate = StartDatePicker?.SelectedDate,
                EndDate = EndDatePicker?.SelectedDate,
            };
        }
    }
}
