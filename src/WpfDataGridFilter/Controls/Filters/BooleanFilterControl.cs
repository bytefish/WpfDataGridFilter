// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class BooleanFilterControl : FilterControl
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly List<FilterOperator> SupportedFilterOperators =
        [
            FilterOperator.None,
            FilterOperator.Yes,
            FilterOperator.No,
            FilterOperator.All,
            FilterOperator.IsNull,
        ];

        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        
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
            "Translations", typeof(ITranslations), typeof(BooleanFilterControl), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BooleanFilterControl booleanFilterControl)
            {
                booleanFilterControl.Translations = (ITranslations)e.NewValue;
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
            "DataGridState", typeof(DataGridState), typeof(BooleanFilterControl), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BooleanFilterControl booleanFilterControl)
            {
                booleanFilterControl.DataGridState = (DataGridState)e.NewValue;

                booleanFilterControl.RefreshFilterDescriptor();
            }
        }

        /// <summary>
        /// Builds the FilterDescriptor described by this Control.
        /// </summary>
        public override FilterDescriptor FilterDescriptor => new BooleanFilterDescriptor
        {
            PropertyName = PropertyName,
            FilterOperator = GetCurrentFilterOperator(),
        };

        private BooleanFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<BooleanFilterDescriptor>(propertyName, out BooleanFilterDescriptor? booleanFilterDescriptor))
            {
                return new BooleanFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperator.None
                };
            }

            return booleanFilterDescriptor;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyButton = GetTemplateChild("PART_ApplyButton") as Button;
            ResetButton = GetTemplateChild("PART_ResetButton") as Button;
            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            
            FilterOperators = GetTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(Translation<FilterOperator>.Text);
                FilterOperatorsComboBox.SelectedValuePath = nameof(Translation<FilterOperator>.Value);
                FilterOperatorsComboBox.ItemsSource = GetTranslations(Translations, SupportedFilterOperators);
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
        }

        private void RefreshFilterDescriptor()
        {
            var booleanFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = booleanFilterDescriptor.FilterOperator;
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

            if(FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperator.None;
            }
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(FilterDescriptor);
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
            
        }

    }
}
