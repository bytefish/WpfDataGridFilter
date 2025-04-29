// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class StringFilterControl : BaseFilterControl<StringFilterDescriptor>
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

        ComboBox? FilterOperatorsComboBox;
        TextBox? ValueTextBox;

        #endregion Controls

        public List<Translation<FilterOperator>> FilterOperators { get; private set; } = [];

        /// <summary>  
        ///  Translations
        /// </summary>
        public override ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        /// <summary>
        /// Value Text Boxis only visible for these Values.
        /// </summary>
        public bool IsValueEnabled => ValidOperatorsForValue.Contains(GetCurrentFilterOperator());

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

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

            if (DataGridState != null)
            {
                OnDataGridStateChanged();
            }

            UpdateFilterControls();
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

        protected override void OnDataGridStateChanged()
        {
            StringFilterDescriptor stringFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = stringFilterDescriptor.FilterOperator;
            }

            if (ValueTextBox != null)
            {
                ValueTextBox.Text = stringFilterDescriptor.Value;
            }

            UpdateFilterControls();
        }

        protected override void OnApplyFilter()
        {
            // Nothing to do...
        }

        protected override void OnResetFilter()
        {
            if (ValueTextBox != null)
            {
                ValueTextBox.Text = string.Empty;
            }
        }

        protected override StringFilterDescriptor GetDefaultFilterDescriptor()
        {
            return new StringFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = FilterOperator.None,
                Value = string.Empty
            };

        }

        protected override FilterDescriptor GetFilterDescriptor()
        {
            return new StringFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = GetCurrentFilterOperator(),
                Value = ValueTextBox?.Text ?? string.Empty
            };
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

        protected override List<Translation<FilterOperator>> GetAdditionalTranslations()
        {
            return [];
        }
    }
}
