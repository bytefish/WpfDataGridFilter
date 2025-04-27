// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class StringFilterControl : FilterControl
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly List<FilterOperator> SupportedFilterOperators =
        [
            FilterOperator.None,
            FilterOperator.IsEmpty,
            FilterOperator.IsNotEmpty,
            FilterOperator.IsNull,
            FilterOperator.IsNotNull,
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.Contains,
            FilterOperator.NotContains,
            FilterOperator.StartsWith,
            FilterOperator.EndsWith,
        ];

        private static List<FilterOperator> ValidOperatorsForValue =
        [
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.Contains,
            FilterOperator.NotContains,
            FilterOperator.StartsWith,
            FilterOperator.EndsWith,
        ];

        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        TextBox? ValueTextBox;

        #endregion Controls

        public override string PropertyName { get; set; } = string.Empty;

        public List<Translation<FilterOperator>> FilterOperators { get; private set; } = [];

        /// <summary>  
        ///  Translations
        /// </summary>
        public override ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register(
            "Translations", typeof(ITranslations), typeof(StringFilterControl), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringFilterControl control)
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
            "DataGridState", typeof(DataGridState), typeof(StringFilterControl), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringFilterControl control)
            {
                control.DataGridState = (DataGridState)e.NewValue;

                control.RefreshFilterDescriptor();
            }
        }

        /// <summary>
        /// Value Text Boxis only visible for these Values.
        /// </summary>
        public bool IsValueEnabled => ValidOperatorsForValue.Contains(GetCurrentFilterOperator());

        /// <summary>
        /// Builds a FilterDescriptor described by this Control.
        /// </summary>
        public override FilterDescriptor FilterDescriptor => new StringFilterDescriptor
        {
            PropertyName = PropertyName,
            FilterOperator = GetCurrentFilterOperator(),
            Value = ValueTextBox?.Text ?? string.Empty
        };

        /// <summary>
        /// Gets the Filter Descriptor off of the DataGridState or creates a new one.
        /// </summary>
        /// <param name="dataGridState">DataGridState with Filters</param>
        /// <param name="propertyName">PropertyName</param>
        /// <returns>The existing FilterDescriptor or a new one</returns>
        private StringFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<StringFilterDescriptor>(propertyName, out StringFilterDescriptor? stringFilterDescriptor))
            {
                return new StringFilterDescriptor
                {
                    FilterOperator = FilterOperator.None,
                    PropertyName = propertyName,                    
                };
            }

            return stringFilterDescriptor;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyButton = GetTemplateChild("PART_ApplyButton") as Button;
            ResetButton = GetTemplateChild("PART_ResetButton") as Button;
            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            ValueTextBox = GetTemplateChild("PART_ValueTextBox") as TextBox;

            FilterOperators = GetTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(Translation<FilterOperator>.Text);
                FilterOperatorsComboBox.SelectedValuePath = nameof(Translation<FilterOperator>.Value);
                FilterOperatorsComboBox.ItemsSource = GetTranslations(Translations, SupportedFilterOperators);
            }

            if (ApplyButton != null)
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

            UpdateFilterControls();
        }

        private void RefreshFilterDescriptor()
        {
            var stringFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = stringFilterDescriptor.FilterOperator;
            }

            if (ValueTextBox != null)
            {
                ValueTextBox.Text = stringFilterDescriptor.Value;
            }
        }

        private List<Translation<FilterOperator>> GetTranslations(ITranslations translations, List<FilterOperator> source)
        {
            List<Translation<FilterOperator>> results = [];

            foreach (var enumValue in source)
            {
                Translation<FilterOperator> translation = translations.FilterOperatorTranslations.First(t => t.Value == enumValue);

                results.Add(translation);
            }

            return results;
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.RemoveFilter(PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperator.None;
            }

            if(ValueTextBox != null)
            {
                ValueTextBox.Text = null;
            }
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(FilterDescriptor);
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
            UpdateFilterControls();
        }

        private void UpdateFilterControls()
        {
            if (ValueTextBox != null)
            {
                ValueTextBox.IsEnabled = IsValueEnabled;
            }
        }
    }
}
