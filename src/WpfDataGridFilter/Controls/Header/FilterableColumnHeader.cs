// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WpfDataGridFilter.Infrastructure;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;
using Path = System.Windows.Shapes.Path;

namespace WpfDataGridFilter.Controls
{
    [TemplatePart(Name = PartName_HeaderLayoutRoot)]
    [TemplatePart(Name = PartName_HeaderSortButton)]
    [TemplatePart(Name = PartName_HeaderAroundBorder)]
    [TemplatePart(Name = PartName_HeaderTextBlock)]
    [TemplatePart(Name = PartName_HeaderFilterToggle)]
    [TemplatePart(Name = PartName_SortArrowNone)]
    [TemplatePart(Name = PartName_SortArrowAsc)]
    [TemplatePart(Name = PartName_SortArrowDesc)]
    [TemplatePart(Name = PartName_Popup)]
    [TemplatePart(Name = PartName_PopupFilterContainer)]
    public class FilterableColumnHeader : DataGridColumnHeader
    {
        public const string PartName_Popup = "PART_FilterPopup";

        public const string PartName_HeaderLayoutRoot = "PART_HeaderLayoutRoot";
        public const string PartName_PopupFilterContainer = "PART_PopupFilterContainer";
        public const string PartName_HeaderAroundBorder = "PART_HeaderAroundBorder";
        public const string PartName_HeaderTextBlock = "PART_HeaderTextBlock";
        public const string PartName_HeaderFilterToggle = "PART_HeaderFilterToggle";
        public const string PartName_HeaderSortButton = "PART_HeaderSortButton";
        public const string PartName_SortArrowNone = "PART_SortArrowNone";
        public const string PartName_SortArrowAsc = "PART_SortArrowAsc";
        public const string PartName_SortArrowDesc = "PART_SortArrowDesc";
        
        public static Thickness StandardHeaderTextBoxBorderThickness = new Thickness(0.5, 1, 0, 1);

        // Header Layout Elements
        Border? HeaderBorder;
        Border? HeaderPopupFilterContainer;

        // Sort Button
        Button? HeaderSortButton;

        // Icons for Arrows
        Path? SortArrowNone;
        Path? SortArrowAsc;
        Path? SortArrowDesc;

        // Toggle for Filter Indicator
        TextBlock? HeaderTextBlock;
        ToggleButton? HeaderToggleButton;

        // Popup invoked on User Click
        Popup? HeaderPopup;

        static FilterableColumnHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterableColumnHeader), new FrameworkPropertyMetadata(typeof(FilterableColumnHeader)));
        }

        #region Dependency Properties

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(FilterableColumnHeader),
            new PropertyMetadata(false)
        );

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="Flyout" /> is visible.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        /// <summary>Identifies the <see cref="PopupOpened"/> routed event.</summary>
        public static readonly RoutedEvent PopupOpenedEvent = EventManager.RegisterRoutedEvent(
            nameof(PopupOpened),
            RoutingStrategy.Bubble,
            typeof(TypedEventHandler<FilterableColumnHeader, RoutedEventArgs>),
            typeof(FilterableColumnHeader)
        );

        /// <summary>Identifies the <see cref="PopupClosed"/> routed event.</summary>
        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
            nameof(PopupClosed),
            RoutingStrategy.Bubble,
            typeof(TypedEventHandler<FilterableColumnHeader, RoutedEventArgs>),
            typeof(FilterableColumnHeader)
        );

        /// <summary>
        /// Event triggered when <see cref="Flyout" /> is opened.
        /// </summary>
        public event TypedEventHandler<FilterableColumnHeader, RoutedEventArgs> PopupOpened
        {
            add => AddHandler(PopupOpenedEvent, value);
            remove => RemoveHandler(PopupOpenedEvent, value);
        }

        /// <summary>
        /// Event triggered when <see cref="FilterableColumnHeader" /> is closed.
        /// </summary>
        public event TypedEventHandler<FilterableColumnHeader, RoutedEventArgs> PopupClosed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        /// <summary>  
        ///  Property for the current font of the text used inside the header
        /// </summary>
        public FontFamily HeaderTextFamily
        {
            get { return (FontFamily)GetValue(HeaderTextFamilyProperty); }
            set { SetValue(HeaderTextFamilyProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextFamilyProperty = DependencyProperty.Register(
            "HeaderTextFamily", typeof(FontFamily), typeof(FilterableColumnHeader), new PropertyMetadata(new FontFamily("Verdana"), OnHeaderTextFamilyChanged));

        private static void OnHeaderTextFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.FontFamily = e.NewValue as FontFamily;
            }
        }

        /// <summary>  
        ///  Property for the current stretch type of the text used inside the header
        /// </summary>
        public SortDirectionEnum? ColumnSortDirection
        {
            get { return (SortDirectionEnum?)GetValue(ColumnSortDirectoryProperty); }
            set { SetValue(ColumnSortDirectoryProperty, value); }
        }

        public static readonly DependencyProperty ColumnSortDirectoryProperty = DependencyProperty.Register(
            "ColumnSortDirection", typeof(SortDirectionEnum?), typeof(FilterableColumnHeader), new PropertyMetadata(null));

        /// <summary>  
        ///  Property for the current stretch type of the text used inside the header
        /// </summary>
        public FontStretch HeaderTextStretch
        {
            get { return (FontStretch)GetValue(HeaderTextStretchProperty); }
            set { SetValue(HeaderTextStretchProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextStretchProperty = DependencyProperty.Register(
            "HeaderTextStretch", typeof(FontStretch), typeof(FilterableColumnHeader), new PropertyMetadata(FontStretches.Normal, OnHeaderTextStretchChanged));

        private static void OnHeaderTextStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.FontStretch = (FontStretch)e.NewValue;
            }
        }

        /// <summary>  
        ///  Property for the current style of the text used inside the header
        /// </summary>
        public FontStyle HeaderTextStyle
        {
            get { return (FontStyle)GetValue(HeaderTextStyleProperty); }
            set { SetValue(HeaderTextStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextStyleProperty = DependencyProperty.Register(
            "HeaderTextStyle", typeof(FontStyle), typeof(FilterableColumnHeader), new PropertyMetadata(FontStyles.Normal, OnHeaderTextStyleChanged));

        private static void OnHeaderTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderTextBlock != null)
                {
                    header.HeaderTextBlock.FontStyle = (FontStyle)e.NewValue;
                }
            }
        }

        /// <summary>  
        ///  Property for the current weight of the text used inside the header
        /// </summary>
        public FontWeight HeaderTextWeight
        {
            get { return (FontWeight)GetValue(HeaderTextWeightProperty); }
            set { SetValue(HeaderTextWeightProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextWeightProperty = DependencyProperty.Register(
            "HeaderTextWeight", typeof(FontWeight), typeof(FilterableColumnHeader), new PropertyMetadata(FontWeights.Normal, OnHeaderTextWeightChanged));

        private static void OnHeaderTextWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderTextBlock != null)
                {
                    header.HeaderTextBlock.FontWeight = (FontWeight)e.NewValue;
                }
            }
        }

        /// <summary>
        ///  Property for the current decoration of the text used inside the header
        /// </summary>
        public TextDecorationCollection HeaderTextDecoration
        {
            get { return (TextDecorationCollection)GetValue(HeaderTextDecorationProperty); }
            set { SetValue(HeaderTextDecorationProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextDecorationProperty = DependencyProperty.Register(
            "HeaderTextDecoration", typeof(TextDecorationCollection), typeof(FilterableColumnHeader), new PropertyMetadata(OnHeaderTextDecorationChanged));

        private static void OnHeaderTextDecorationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderTextBlock != null)
                {
                    header.HeaderTextBlock.TextDecorations = (TextDecorationCollection)e.NewValue;
                }
            }
        }

        /// <summary>  
        ///  Property for the current tickness of the border around the header
        /// </summary>
        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty HeaderBorderThicknessProperty = DependencyProperty.Register(
            "HeaderBorderThickness", typeof(Thickness), typeof(FilterableColumnHeader), new PropertyMetadata(StandardHeaderTextBoxBorderThickness, OnHeaderBorderThicknessChanged));

        private static void OnHeaderBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderBorder != null)
                {
                    header.HeaderBorder.BorderThickness = (Thickness)e.NewValue;
                }
            }
        }

        /// <summary>  
        ///  Property for the current tickness of the border around the header
        /// </summary>
        public Thickness HeaderBorderMargin
        {
            get { return (Thickness)GetValue(HeaderBorderMarginProperty); }
            set { SetValue(HeaderBorderMarginProperty, value); }
        }

        public static readonly DependencyProperty HeaderBorderMarginProperty = DependencyProperty.Register(
            "HeaderBorderMargin", typeof(Thickness), typeof(FilterableColumnHeader), new PropertyMetadata(StandardHeaderTextBoxBorderThickness, OnHeaderBorderMarginChanged));

        private static void OnHeaderBorderMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderBorder != null)
                {
                    header.HeaderBorder.Margin = (Thickness)e.NewValue;
                }
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
            "DataGridState", typeof(DataGridState), typeof(FilterableColumnHeader), new PropertyMetadata(propertyChangedCallback: OnDataGridStateChanged));

        private static void OnDataGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.DataGridState = (DataGridState)e.NewValue;
            }
        }

        /// <summary>  
        ///  FilterType of the Column.
        /// </summary>
        public string FilterType
        {
            get { return (string)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", 
            typeof(string), typeof(FilterableColumnHeader), new PropertyMetadata("StringFilter", OnFilterTypeChanged));

        private static void OnFilterTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.FilterType = (string)e.NewValue;
            }
        }
        
        /// <summary>  
        ///  FilterProvider for the FilterControls.
        /// </summary>
        public IFilterControlProvider FilterControlProvider
        {
            get { return (IFilterControlProvider)GetValue(FilterControlProviderProperty); }
            set { SetValue(FilterControlProviderProperty, value); }
        }

        public static readonly DependencyProperty FilterControlProviderProperty = DependencyProperty.Register("FilterControlProvider", 
            typeof(IFilterControlProvider), typeof(FilterableColumnHeader), new PropertyMetadata(new FilterControlProvider(), OnFilterControlProviderChanged));

        private static void OnFilterControlProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.FilterControlProvider = (IFilterControlProvider)e.NewValue;
            }
        }

        /// <summary>  
        ///  Translations
        /// </summary>
        public ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register(
            "Translations", typeof(ITranslations), typeof(FilterableColumnHeader), new PropertyMetadata(new NeutralTranslations(), OnTranslationsChanged));

        private static void OnTranslationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.Translations = (ITranslations)e.NewValue;
            }
        }

        /// <summary>  
        ///  Property showing whether a filtered is applied in the column
        /// </summary>
        public bool IsFiltered
        {
            get { return (bool)GetValue(IsFilteredProperty); }
            set { SetValue(IsFilteredProperty, value); }
        }

        public static readonly DependencyProperty IsFilteredProperty = DependencyProperty.Register(
            "IsFiltered", typeof(bool), typeof(FilterableColumnHeader), new PropertyMetadata(false, OnIsFilteredChanged));

        private static void OnIsFilteredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.IsFiltered = (bool)e.NewValue;
            }
        }

        /// <summary>  
        ///  Property for the Column Header Text
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof(string), typeof(FilterableColumnHeader), new PropertyMetadata(OnHeaderTextChanged));

        private static void OnHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderTextBlock != null)
                {
                    header.HeaderTextBlock.Text = (string)e.NewValue;
                }
            }
        }

        /// <summary>  
        ///  Property for the current size of the text used inside the header
        /// </summary>
        public double HeaderTextSize
        {
            get { return (double)GetValue(HeaderTextSizeProperty); }
            set { SetValue(HeaderTextSizeProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextSizeProperty = DependencyProperty.Register(
            "HeaderTextSize", typeof(double), typeof(FilterableColumnHeader), new PropertyMetadata(17.0, OnHeaderTextSizeChanged));

        private static void OnHeaderTextSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                if (header.HeaderTextBlock != null)
                {
                    header.HeaderTextBlock.FontSize = (double)e.NewValue;
                }
            }
        }

        /// <summary>  
        ///  Property for the Filter Property we are filtering for
        /// </summary>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            "PropertyName", typeof(string), typeof(FilterableColumnHeader), new PropertyMetadata(string.Empty,
                propertyChangedCallback: OnPropertyNameChanged));

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterableColumnHeader header)
            {
                header.PropertyName = (string)e.NewValue;
            }
        }

        #endregion Dependency Properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            HeaderPopup = GetTemplateChild(PartName_Popup) as Popup;
            HeaderBorder = GetTemplateChild(PartName_HeaderAroundBorder) as Border;
            HeaderPopupFilterContainer = GetTemplateChild(PartName_PopupFilterContainer) as Border;
            HeaderTextBlock = GetTemplateChild(PartName_HeaderTextBlock) as TextBlock;
            HeaderToggleButton = GetTemplateChild(PartName_HeaderFilterToggle) as ToggleButton;
            HeaderSortButton = GetTemplateChild(PartName_HeaderSortButton) as Button;

            SortArrowAsc = GetTemplateChild(PartName_SortArrowAsc) as Path;
            SortArrowDesc = GetTemplateChild(PartName_SortArrowDesc) as Path;
            SortArrowNone = GetTemplateChild(PartName_SortArrowNone) as Path;
            
            // Subscribe to Events
            if (HeaderPopup != null)
            {
                HeaderPopup.MouseDown -= OnHeaderPopupMouseDown;
                HeaderPopup.MouseDown += OnHeaderPopupMouseDown;

                HeaderPopup.Closed -= OnHeaderPopupClosed;
                HeaderPopup.Closed += OnHeaderPopupClosed;

                HeaderPopup.Opened -= OnHeaderPopupOpened;
                HeaderPopup.Opened += OnHeaderPopupOpened;
            }

            if (HeaderToggleButton != null)
            {
                HeaderToggleButton.Checked -= OnHeaderToggleChecked;
                HeaderToggleButton.Checked += OnHeaderToggleChecked;

                HeaderToggleButton.Unchecked -= OnHeaderToggleUnchecked;
                HeaderToggleButton.Unchecked += OnHeaderToggleUnchecked;
            }

            if (HeaderSortButton != null)
            {
                HeaderSortButton.Click -= OnSortButtonClick;
                HeaderSortButton.Click += OnSortButtonClick;
            }

            if (DataGridState != null)
            {
                DataGridState.DataGridStateChanged -= OnDataGridStateChanged;
                DataGridState.DataGridStateChanged += OnDataGridStateChanged;

                IsFiltered = DataGridState.Filters.ContainsKey(PropertyName);
            }

            if(HeaderTextBlock != null)
            {
                HeaderTextBlock.Text = HeaderText;
            }
        }

        private void OnHeaderPopupMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnHeaderToggleChecked(object sender, RoutedEventArgs e)
        {
            if (HeaderPopup != null)
            {
                HeaderPopup.IsOpen = true;
            }

            if (HeaderPopupFilterContainer != null)
            {
                HeaderPopupFilterContainer!.Child = CreateFilterControl();
            }
        }

        private void UpdateFilterToggleImage()
        {
            if(HeaderToggleButton != null)
            {
                
            }
        }

        private void OnHeaderToggleUnchecked(object sender, RoutedEventArgs e)
        {
            if (HeaderPopup != null)
            {
                HeaderPopup.IsOpen = false;
            }
        }

        private void OnHeaderPopupOpened(object? sender, EventArgs e)
        {
            if (HeaderToggleButton == null)
            {
                return;
            }

            if (!HeaderToggleButton.IsPressed)
            {
                HeaderToggleButton.IsChecked = true;
            }
        }

        private void OnHeaderPopupClosed(object? sender, EventArgs e)
        {
            if (HeaderToggleButton == null)
            {
                return;
            }

            if (!HeaderToggleButton.IsPressed)
            {
                HeaderToggleButton.IsChecked = false;
            }
        }

        private void OnSortButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataGridState == null)
            {
                return;
            }

            SortDirectionEnum? columnSortDirection = GetNextSortDirection();

            DataGridState.SetSortColumn(new SortColumn(PropertyName, columnSortDirection));

            switch(columnSortDirection)
            {
                case null:
                    SortArrowNone!.Visibility = Visibility.Visible;
                    SortArrowAsc!.Visibility = Visibility.Collapsed;
                    SortArrowDesc!.Visibility = Visibility.Collapsed;
                    break;
                case SortDirectionEnum.Ascending:
                    SortArrowNone!.Visibility = Visibility.Collapsed;
                    SortArrowAsc!.Visibility = Visibility.Visible;
                    SortArrowDesc!.Visibility = Visibility.Collapsed;
                    break;
                case SortDirectionEnum.Descending:
                    SortArrowNone!.Visibility = Visibility.Collapsed;
                    SortArrowAsc!.Visibility = Visibility.Collapsed;
                    SortArrowDesc!.Visibility = Visibility.Visible;
                    break;
            }            
        }

        private void OnDataGridStateChanged(object? sender, DataGridStateChangedEventArgs e)
        {
            // The DataGridState allows to sort by one Column only. So all we need to check is, 
            // if this Column Header is being filtered for. If we are being filtered, we set the 
            // Sort Direction, of reset it.
            ColumnSortDirection = GetCurrentSortDirection();

            // The DataGridState has a list of filters, so we need to check, if this Column Header
            // is filtered. We set the IsFiltered Property accordingly.
            IsFiltered = DataGridState.Filters.ContainsKey(PropertyName);

            // Update the Filter Icon Image.
            UpdateFilterToggleImage();
        }

        /// <summary>
        /// Returns this Column Headers Sort Direction.
        /// </summary>
        /// <returns>SortDirection for this Column</returns>
        private SortDirectionEnum? GetCurrentSortDirection()
        {
            if (DataGridState == null)
            {
                return null;
            }

            SortColumn? sortColumn = DataGridState.SortColumn;

            if (sortColumn == null)
            {
                return null;
            }

            if (!string.Equals(sortColumn.PropertyName, PropertyName))
            {
                return null;
            }

            return sortColumn.SortDirection;
        }

        private SortDirectionEnum? GetNextSortDirection()
        {
            switch (ColumnSortDirection)
            {
                case null:
                    return SortDirectionEnum.Ascending;
                case SortDirectionEnum.Ascending:
                    return SortDirectionEnum.Descending;
                case SortDirectionEnum.Descending:
                    return null;
                default:
                    throw new InvalidOperationException("Could not determine next Sort Direction");
            }
        }

        private FilterControl CreateFilterControl()
        {
            // Maybe we could pass the FilterType explicitly, but it would lead to an ugly XAML API.
            FilterType filterType = new FilterType { Name = FilterType };

            FilterControl filterControl = FilterControlProvider.CreateFilterControl(filterType);

            filterControl.PropertyName = PropertyName;
            filterControl.Translations = Translations;
            filterControl.DataGridState = DataGridState;

            return filterControl;
        }
    }
}