using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        public static readonly List<FilterOperatorEnum> SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
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

        private static List<FilterOperatorEnum> ValidOperatorsForValue =
        [
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.Contains,
            FilterOperatorEnum.NotContains,
            FilterOperatorEnum.StartsWith,
            FilterOperatorEnum.EndsWith,
        ];

        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        TextBox? ValueTextBox;

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
                    FilterOperator = FilterOperatorEnum.None,
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

            FilterOperators = GetEnumTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(EnumTranslation<FilterOperatorEnum>.Translation);
                FilterOperatorsComboBox.SelectedValuePath = nameof(EnumTranslation<FilterOperatorEnum>.Value);
                FilterOperatorsComboBox.ItemsSource = GetEnumTranslations(Translations, SupportedFilterOperators);
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

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperatorEnum.None;
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

        private FilterOperatorEnum GetCurrentFilterOperator()
        {
            if (FilterOperatorsComboBox == null)
            {
                return FilterOperatorEnum.None;
            }

            if (FilterOperatorsComboBox.SelectedValue == null)
            {
                return FilterOperatorEnum.None;
            }

            FilterOperatorEnum currentFilterOperator = (FilterOperatorEnum)FilterOperatorsComboBox.SelectedValue;

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
