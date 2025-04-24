using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    public class BooleanFilterControl : Control 
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly List<FilterOperatorEnum> SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
            FilterOperatorEnum.Yes,
            FilterOperatorEnum.No,
            FilterOperatorEnum.All,
            FilterOperatorEnum.IsNull,
        ];


        #region Controls 

        Button? ApplyButton;
        Button? ResetButton;
        ComboBox? FilterOperatorsComboBox;
        
        #endregion Controls

        public string PropertyName { get; set; } = string.Empty;

        public List<EnumTranslation<FilterOperatorEnum>> FilterOperators { get; private set; } = [];

        /// <summary>  
        ///  Translations
        /// </summary>
        public ITranslations Translations
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
        public DataGridState DataGridState
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

        private BooleanFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<BooleanFilterDescriptor>(propertyName, out BooleanFilterDescriptor? booleanFilterDescriptor))
            {
                return new BooleanFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
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
        }

        private void RefreshFilterDescriptor()
        {
            var booleanFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = booleanFilterDescriptor.FilterOperator;
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

            if(FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperatorEnum.None;
            }
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            FilterDescriptor booleanFilterDescriptor = new BooleanFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = GetCurrentFilterOperator(),
            };

            DataGridState.AddFilter(booleanFilterDescriptor);
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
            
        }

    }
}
