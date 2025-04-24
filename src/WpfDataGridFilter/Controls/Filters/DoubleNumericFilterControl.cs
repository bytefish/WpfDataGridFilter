// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class DoubleNumericFilterControl : FilterControl
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

        private static List<FilterOperatorEnum> ValidOperatorsForLowerValue =
        [
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.Before,
            FilterOperatorEnum.After,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive,
        ];

        private static List<FilterOperatorEnum> ValidOperatorsForUpperValue =
        [
            FilterOperatorEnum.BetweenInclusive,
            FilterOperatorEnum.BetweenExclusive,
        ];

        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        TextBox? LowerValueTextBox;
        TextBox? UpperValueTextBox;

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
            "Translations", typeof(ITranslations), typeof(DoubleNumericFilterControl), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleNumericFilterControl control)
            {
                control.Translations = (ITranslations)e.NewValue;
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
            "DataGridState", typeof(DataGridState), typeof(DoubleNumericFilterControl), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleNumericFilterControl control)
            {
                control.DataGridState = (DataGridState)e.NewValue;

                control.RefreshFilterDescriptor();
            }
        }

        /// <summary>
        /// Start Date is only visible for these Values.
        /// </summary>
        public bool IsLowerValueEnabled => ValidOperatorsForLowerValue.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// End Date is only visible for these operators.
        /// </summary>
        public bool IsUpperValueEnabled => ValidOperatorsForUpperValue.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// Creates a FilterDescriptor this Control describes.
        /// </summary>
        public override FilterDescriptor FilterDescriptor => new DoubleNumericFilterDescriptor
        {
            PropertyName = PropertyName,
            FilterOperator = GetCurrentFilterOperator(),
            LowerValue = GetDoubleValue(LowerValueTextBox?.Text),
            UpperValue = GetDoubleValue(UpperValueTextBox?.Text),
        };

        /// <summary>
        /// Gets the Filter Descriptor off of the DataGridState or creates a new one.
        /// </summary>
        /// <param name="dataGridState">DataGridState with Filters</param>
        /// <param name="propertyName">PropertyName</param>
        /// <returns>The existing FilterDescriptor or a new one</returns>
        private DoubleNumericFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<DoubleNumericFilterDescriptor>(propertyName, out var descriptor))
            {
                return new DoubleNumericFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
                };
            }

            return descriptor;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyButton = GetTemplateChild("PART_ApplyButton") as Button;
            ResetButton = GetTemplateChild("PART_ResetButton") as Button;
            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            LowerValueTextBox = GetTemplateChild("PART_LowerValueTextBox") as TextBox;
            UpperValueTextBox = GetTemplateChild("PART_UpperValueTextBox") as TextBox;

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

            UpdateDoubleNumericFilterControls();
        }

        private void RefreshFilterDescriptor()
        {
            var doubleNumericFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = doubleNumericFilterDescriptor.FilterOperator;
            }

            if (UpperValueTextBox != null)
            {
                UpperValueTextBox.Text = doubleNumericFilterDescriptor.LowerValue.ToString();
            }

            if (LowerValueTextBox != null)
            {
                LowerValueTextBox.Text = doubleNumericFilterDescriptor.UpperValue.ToString();
            }

            UpdateDoubleNumericFilterControls();
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

            if (LowerValueTextBox != null)
            {
                LowerValueTextBox.Text = null;
            }

            if (UpperValueTextBox != null)
            {
                UpperValueTextBox.Text = null;
            }

            UpdateDoubleNumericFilterControls();
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(FilterDescriptor);
        }

        private double? GetDoubleValue(string? value)
        {
            if(!double.TryParse(value, out double result))
            {
                return null;
            }

            return result;
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
    }
}
