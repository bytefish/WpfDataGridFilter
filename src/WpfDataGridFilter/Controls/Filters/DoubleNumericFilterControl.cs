// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class DoubleNumericFilterControl : BaseFilterControl<DoubleNumericFilterDescriptor>
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

        private static List<FilterOperator> ValidOperatorsForLowerValue =
        [
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.Before,
            FilterOperator.After,
            FilterOperator.BetweenExclusive,
            FilterOperator.BetweenInclusive,
        ];

        private static List<FilterOperator> ValidOperatorsForUpperValue =
        [
            FilterOperator.BetweenInclusive,
            FilterOperator.BetweenExclusive,
        ];

        #region Controls 

        private ComboBox? FilterOperatorsComboBox;

        private TextBox? LowerValueTextBox;

        private TextBox? UpperValueTextBox;

        #endregion Controls

        public List<Translation<FilterOperator>> FilterOperators { get; private set; } = [];

        /// <summary>
        /// Start Date is only visible for these Values.
        /// </summary>
        public bool IsLowerValueEnabled => ValidOperatorsForLowerValue.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsUpperValueEnabled => ValidOperatorsForUpperValue.Contains(GetCurrentFilterOperator());

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            LowerValueTextBox = GetTemplateChild("PART_LowerValueTextBox") as TextBox;
            UpperValueTextBox = GetTemplateChild("PART_UpperValueTextBox") as TextBox;

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

            UpdateDoubleNumericFilterControls();
        }

        private double? GetDoubleValue(string? value)
        {
            if(!double.TryParse(value, out double result))
            {
                return null;
            }

            return result;
        }

        private FilterOperator GetCurrentFilterOperator()
        {
            if(FilterOperatorsComboBox == null)
            {
                return FilterOperator.None;
            }

            if(FilterOperatorsComboBox.SelectedValue == null)
            {
                return FilterOperator.None;
            }

            FilterOperator currentFilterOperator = (FilterOperator) FilterOperatorsComboBox.SelectedValue;

            return currentFilterOperator;            
        }

        private void OnFilterOperatorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDoubleNumericFilterControls();
        }

        private void UpdateDoubleNumericFilterControls()
        {
            if (LowerValueTextBox != null)
            {
                LowerValueTextBox.IsEnabled = IsLowerValueEnabled;
            }

            if (UpperValueTextBox != null)
            {
                UpperValueTextBox.IsEnabled = IsUpperValueEnabled;
            }
        }

        protected override void OnDataGridStateChanged()
        {
            DoubleNumericFilterDescriptor doubleNumericFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = doubleNumericFilterDescriptor.FilterOperator;
            }

            if (LowerValueTextBox != null)
            {
                LowerValueTextBox.Text = doubleNumericFilterDescriptor.LowerValue?.ToString();
            }

            if (UpperValueTextBox != null)
            {
                UpperValueTextBox.Text = doubleNumericFilterDescriptor.UpperValue?.ToString();
            }

            UpdateDoubleNumericFilterControls();
        }

        protected override void OnApplyFilter()
        {
            // Nothing to do...
        }

        protected override void OnResetFilter()
        {
            UpdateDoubleNumericFilterControls();
        }

        protected override DoubleNumericFilterDescriptor GetDefaultFilterDescriptor()
        {
            return new DoubleNumericFilterDescriptor
            {
                FilterOperator = FilterOperator.None,
                PropertyName = PropertyName,
            };
        }

        protected override FilterDescriptor GetFilterDescriptor()
        {
            return new DoubleNumericFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = GetCurrentFilterOperator(),
                LowerValue = GetDoubleValue(LowerValueTextBox?.Text),
                UpperValue = GetDoubleValue(UpperValueTextBox?.Text),
            };
        }

        protected override List<Translation<FilterOperator>> GetAdditionalTranslations()
        {
            return [];
        }
    }
}
