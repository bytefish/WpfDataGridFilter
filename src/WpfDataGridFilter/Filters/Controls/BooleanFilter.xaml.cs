﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Filters.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Filters.Controls
{
     /// <summary>
    /// ViewModel.
    /// </summary>
    public partial class BooleanFilterViewModel : ObservableObject
    {
        /// <summary>
        /// Supported Filters for this Filter Control.
        /// </summary>
        public static readonly FilterOperatorEnum[] SupportedFilterOperators =
        [
            FilterOperatorEnum.None,
            FilterOperatorEnum.Yes,
            FilterOperatorEnum.No,
            FilterOperatorEnum.All, 
            FilterOperatorEnum.IsNull,
        ];

        [ObservableProperty]
        private ITranslations _translations;

        /// <summary>
        /// Translations used for the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EnumTranslation<FilterOperatorEnum>> _filterOperators = new();

        /// <summary>
        /// Property Name this Filter handles.
        /// </summary>
        public readonly string PropertyName;
                
        public BooleanFilterViewModel(ITranslations translations, BooleanFilterDescriptor booleanFilterDescriptor)
        {
            Translations = translations;

            PropertyName = booleanFilterDescriptor.PropertyName;

            // Sets the
            foreach (var supportedFilterOperator in SupportedFilterOperators)
            {
                EnumTranslation<FilterOperatorEnum> translation = translations.FilterOperatorTranslations.First(t => t.Value == supportedFilterOperator);

                FilterOperators.Add(translation);
            }

            // Sets the currently selected FilterOperator
            SelectedFilterOperator = booleanFilterDescriptor.FilterOperator;


        }

        [ObservableProperty]
        private FilterOperatorEnum _selectedFilterOperator = FilterOperatorEnum.None;

        public FilterDescriptor FilterDescriptor => new BooleanFilterDescriptor
        {
            FilterOperator = SelectedFilterOperator,
            PropertyName = PropertyName, 
        };
    }

    /// <summary>
    /// Interaction logic for BooleanFilter.xaml
    /// </summary>
    public partial class BooleanFilter : UserControl
    {
        /// <summary>
        /// Filter State.
        /// </summary>
        public FilterState FilterState { get; }

        /// <summary>  
        ///  Selected Filter Operator in the ComboBox.
        /// </summary>
        public BooleanFilterViewModel ViewModel { get; set; }

        /// <summary>
        /// Creates a new Boolean Filter.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="filterState">Filter State</param>
        public BooleanFilter(string propertyName, ITranslations translations, FilterState filterState)
        {
            InitializeComponent();

            FilterState = filterState;
            ViewModel = GetFilterViewModel(propertyName, translations, filterState);
            
            DataContext = ViewModel;
            DataContext = this;
        }

        private BooleanFilterViewModel GetFilterViewModel(string propertyName, ITranslations translations, FilterState filterState)
        {
            BooleanFilterDescriptor booleanFilterDescriptor = GetFilterDescriptor(propertyName, filterState);

            return new BooleanFilterViewModel(translations, booleanFilterDescriptor);
        }

        private BooleanFilterDescriptor GetFilterDescriptor(string propertyName, FilterState filterState)
        {
            if(!filterState.TryGetFilter<BooleanFilterDescriptor>(propertyName, out var booleanFilterDescriptor))
            {
                return new BooleanFilterDescriptor
                {
                    PropertyName = propertyName,
                    FilterOperator = FilterOperatorEnum.None
                };
            }

            return booleanFilterDescriptor;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            FilterState.RemoveFilter(ViewModel.PropertyName);

            ViewModel.SelectedFilterOperator = FilterOperatorEnum.None;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            FilterState.AddFilter(ViewModel.FilterDescriptor);
        }
    }
}
