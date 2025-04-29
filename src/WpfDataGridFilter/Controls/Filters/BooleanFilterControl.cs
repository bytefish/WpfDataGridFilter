// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Markup;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{

    public class BooleanFilterControl : BaseFilterControl<BooleanFilterDescriptor>
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

        ComboBox? FilterOperatorsComboBox;
        
        #endregion Controls


        public List<Translation<FilterOperator>> FilterOperators { get; private set; } = [];


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FilterOperatorsComboBox = GetTemplateChild("PART_FilterOperators") as ComboBox;
            
            FilterOperators = GetFilterOperatorTranslations(Translations, SupportedFilterOperators);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectionChanged -= OnFilterOperatorSelectionChanged;
                FilterOperatorsComboBox.SelectionChanged += OnFilterOperatorSelectionChanged;

                FilterOperatorsComboBox.DisplayMemberPath = nameof(Translation<FilterOperator>.Text);
                FilterOperatorsComboBox.SelectedValuePath = nameof(Translation<FilterOperator>.Value);
                FilterOperatorsComboBox.ItemsSource = GetFilterOperatorTranslations(Translations, SupportedFilterOperators);
            }

            // We need to check if this is required ...
            if (DataGridState != null)
            {
                OnDataGridStateChanged();
            }
        }

        protected override void OnDataGridStateChanged()
        {
            BooleanFilterDescriptor booleanFilterDescriptor = GetFilterDescriptor(DataGridState, PropertyName);

            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = booleanFilterDescriptor.FilterOperator;
            }
        }

        protected override void OnResetFilter()
        {
            if (FilterOperatorsComboBox != null)
            {
                FilterOperatorsComboBox.SelectedValue = FilterOperator.None;
            }
        }

        protected override void OnApplyFilter()
        {
            // Nothing to do yet...
        }

        protected override BooleanFilterDescriptor GetDefaultFilterDescriptor()
        {
            return new BooleanFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = FilterOperator.None
            };
        }

        protected override FilterDescriptor GetFilterDescriptor()
        {
            return new BooleanFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = GetCurrentFilterOperator(),
            };
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
            // Probably useful ...
        }

    }
}
