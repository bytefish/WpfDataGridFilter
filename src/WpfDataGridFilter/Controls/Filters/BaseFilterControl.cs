﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Controls
{
    [TemplatePart(Name = "PART_ApplyButton")]
    [TemplatePart(Name = "PART_ResetButton")]
    public abstract class BaseFilterControl<TFilterDescriptor> : FilterControl
        where TFilterDescriptor : FilterDescriptor
    {
        private Button? ApplyButton;
        private Button? ResetButton;

        /// <summary>  
        ///  Translations
        /// </summary>
        public override ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register(
            "Translations", typeof(ITranslations), typeof(BaseFilterControl<TFilterDescriptor>), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaseFilterControl<TFilterDescriptor> control)
            {
                control.Translations = (ITranslations)e.NewValue;
            }
        }

        /// <summary>  
        ///  DataGridState
        /// </summary>
        public override DataGridState DataGridState
        {
            get { return (DataGridState)GetValue(DataGridStateProperty); }
            set { SetValue(DataGridStateProperty, value); }
        }

        public static readonly DependencyProperty DataGridStateProperty = DependencyProperty.Register(
            "DataGridState", typeof(DataGridState), typeof(BaseFilterControl<TFilterDescriptor>), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaseFilterControl<TFilterDescriptor> control)
            {
                DataGridState dataGridState = (DataGridState)e.NewValue;

                control.DataGridState = dataGridState;
                control.OnDataGridStateChanged();
            }
        }

        public override string PropertyName { get; set; } = string.Empty;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyButton = GetTemplateChild("PART_ApplyButton") as Button;
            ResetButton = GetTemplateChild("PART_ResetButton") as Button;

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
                OnDataGridStateChanged();
            }
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.RemoveFilter(PropertyName);
            OnDataGridStateChanged();
            OnResetFilter();
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridState.AddFilter(FilterDescriptor);
            OnDataGridStateChanged();
            OnApplyFilter();
        }

        protected List<Translation<FilterOperator>> GetFilterOperatorTranslations(ITranslations translations, List<FilterOperator> filterOperators)
        {
            List<Translation<FilterOperator>> filterOperatorTranslations =
            [
                ..translations.FilterOperatorTranslations,
                ..GetAdditionalTranslations()
            ];

            List<Translation<FilterOperator>> results = [];

            foreach (var filterOperator in filterOperators)
            {
                Translation<FilterOperator>? translation = filterOperatorTranslations.FirstOrDefault(t => t.Value == filterOperator);

                if (translation == null)
                {
                    translation = new Translation<FilterOperator> 
                    { 
                        Value = filterOperator, 
                        Text = filterOperator.Name 
                    };
                }

                results.Add(translation);
            }

            return results;
        }

        protected TFilterDescriptor GetFilterDescriptor(DataGridState dataGridState, string propertyName)
        {
            if (!dataGridState.TryGetFilter<TFilterDescriptor>(propertyName, out TFilterDescriptor? filterDescriptor))
            {
                return GetDefaultFilterDescriptor();
            }

            return filterDescriptor;
        }

        /// <summary>
        /// Filter Descriptor.
        /// </summary>
        public override FilterDescriptor FilterDescriptor => GetFilterDescriptor();

        /// <summary>
        /// Invoked, if the <see cref="DataGridState"> has been changed.
        /// </summary>
        protected abstract void OnDataGridStateChanged();

        /// <summary>
        /// Invoked, if the Filter has been applied.
        /// </summary>
        protected abstract void OnApplyFilter();

        /// <summary>
        /// Invoked, if the Filter has been reset.
        /// </summary>
        protected abstract void OnResetFilter();

        /// <summary>
        /// Build the initial FilterDescriptor, if the <see cref="DataGridState"> does not
        /// contain a Filter for the current Property.
        /// </summary>
        /// <returns></returns>
        protected abstract TFilterDescriptor GetDefaultFilterDescriptor();

        /// <summary>
        /// Creates the <see cref="FilterDescriptor"> this FilterControl describes.
        /// </summary>
        /// <returns></returns>
        protected abstract FilterDescriptor GetFilterDescriptor();

        protected abstract List<Translation<FilterOperator>> GetAdditionalTranslations();
    }
}
