using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfDataGridFilter.Controls;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;
using Path = System.Windows.Shapes.Path;

namespace WpfDataGridFilter
{
    public class FilterableDataGridColumnHeader : DataGridColumnHeader
    {
        public static Thickness StandardHeaderTextBoxBorderThickness = new Thickness(0.5, 1, 0, 1);

        // Header Element
        DependencyObject RootObject;

        // Header Layout Elements
        Border HeaderBorder;

        // Sort Button
        Button? SortButton;

        Path? SortArrowNone;
        Path? SortArrowAsc;
        Path? SortArrowDesc;

        // Toggle for Filter Indicator
        TextBlock HeaderTextBlock;
        ToggleButton HeaderToggle;

        // Images for Header Icons
        Image ImageFilterBlack;
        Image ImageFilterRed;

        // Popup invoked on User Click
        Popup? HeaderPopUp;

        #region Dependency Properties

        /// <summary>  
        ///  Property for the current font of the text used inside the header
        /// </summary>
        public FontFamily HeaderTextFamily
        {
            get { return (FontFamily)GetValue(HeaderTextFamilyProperty); }
            set { SetValue(HeaderTextFamilyProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextFamilyProperty = DependencyProperty.Register(
            "HeaderTextFamily", typeof(FontFamily), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(new FontFamily("Verdana"),
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.FontFamily = (FontFamily)e.NewValue)));

        /// <summary>  
        ///  Property for the current stretch type of the text used inside the header
        /// </summary>
        public SortDirectionEnum? ColumnSortDirection
        {
            get { return (SortDirectionEnum?)GetValue(ColumnSortDirectoryProperty); }
            set { SetValue(ColumnSortDirectoryProperty, value); }
        }

        public static readonly DependencyProperty ColumnSortDirectoryProperty = DependencyProperty.Register(
            "`ColumnSortDirection", typeof(SortDirectionEnum?), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(null,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.OnColumnSortChanged((SortDirectionEnum?) e.NewValue))));
        
        /// <summary>  
        ///  Property for the current stretch type of the text used inside the header
        /// </summary>
        public FontStretch HeaderTextStretch
        {
            get { return (FontStretch)GetValue(HeaderTextStretchProperty); }
            set { SetValue(HeaderTextStretchProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextStretchProperty = DependencyProperty.Register(
            "HeaderTextStretch", typeof(FontStretch), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(FontStretches.Normal,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.FontStretch = (FontStretch)e.NewValue)));

        /// <summary>  
        ///  Property for the current style of the text used inside the header
        /// </summary>
        public FontStyle HeaderTextStyle
        {
            get { return (FontStyle)GetValue(HeaderTextStyleProperty); }
            set { SetValue(HeaderTextStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextStyleProperty = DependencyProperty.Register(
            "HeaderTextStyle", typeof(FontStyle), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(FontStyles.Normal,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.FontStyle = (FontStyle)e.NewValue)));

        /// <summary>  
        ///  Property for the current weight of the text used inside the header
        /// </summary>
        public FontWeight HeaderTextWeight
        {
            get { return (FontWeight)GetValue(HeaderTextWeightProperty); }
            set { SetValue(HeaderTextWeightProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextWeightProperty = DependencyProperty.Register(
            "HeaderTextWeight", typeof(FontWeight), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(FontWeights.Normal,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.FontWeight = (FontWeight)e.NewValue)));

        /// <summary>
        ///  Property for the current decoration of the text used inside the header
        /// </summary>
        public TextDecorationCollection HeaderTextDecoration
        {
            get { return (TextDecorationCollection)GetValue(HeaderTextDecorationProperty); }
            set { SetValue(HeaderTextDecorationProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextDecorationProperty = DependencyProperty.Register(
            "HeaderTextDecoration", typeof(TextDecorationCollection), typeof(FilterableDataGridColumnHeader),
                new PropertyMetadata(propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.TextDecorations = (TextDecorationCollection)e.NewValue)));

        /// <summary>  
        ///  Property for the current tickness of the border around the header
        /// </summary>
        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty HeaderBorderThicknessProperty = DependencyProperty.Register(
            "HeaderBorderThickness", typeof(Thickness), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(StandardHeaderTextBoxBorderThickness,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderBorder.BorderThickness = (Thickness)e.NewValue)));

        /// <summary>  
        ///  Property for the current tickness of the border around the header
        /// </summary>
        public Thickness HeaderBorderMargin
        {
            get { return (Thickness)GetValue(HeaderBorderMarginProperty); }
            set { SetValue(HeaderBorderMarginProperty, value); }
        }

        public static readonly DependencyProperty HeaderBorderMarginProperty = DependencyProperty.Register(
            "HeaderBorderMargin", typeof(Thickness), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(StandardHeaderTextBoxBorderThickness,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderBorder.Margin = (Thickness)e.NewValue)));

        /// <summary>  
        ///  FilterState of the current DataGrid.
        /// </summary>
        public DataGridState DataGridState
        {
            get { return (DataGridState)GetValue(DataGridStateProperty); }
            set { SetValue(DataGridStateProperty, value); }
        }

        public static readonly DependencyProperty DataGridStateProperty = DependencyProperty.Register(
            "DataGridState", typeof(DataGridState), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(null,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => 
                {
                    f.DataGridState = (DataGridState)e.NewValue;

                    // Wow, this is super ugly. This needs to be designed much better.
                    f.DataGridState.DataGridStateChanged += delegate (object? sender, DataGridStateChangedEventArgs filterStateChangedEventArgs)
                    {
                        // Something has been changed in the Filter. Set the new Column Sort Direction.
                        f.ColumnSortDirection = string.Equals(f.DataGridState.SortColumn?.PropertyName, f.PropertyName) ? f.DataGridState.SortColumn?.SortDirection : null;

                        // We will be able to bind to this Property
                        f.IsFiltered = filterStateChangedEventArgs.DataGridState.Filters.ContainsKey(f.PropertyName);

                        f.HeaderToggle.Content = f.IsFiltered ? f.ImageFilterRed : (object)f.ImageFilterBlack;
                    };
                })));

        /// <summary>  
        ///  FilterType of the Column.
        /// </summary>
        public FilterTypeEnum FilterType
        {
            get { return (FilterTypeEnum)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(FilterTypeEnum), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(FilterTypeEnum.StringFilter,
            propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.FilterType = (FilterTypeEnum)e.NewValue)));


        /// <summary>  
        ///  Translations
        /// </summary>
        public ITranslations Translations
        {
            get { return (ITranslations)GetValue(TranslationsProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register("Translations", typeof(ITranslations), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(new NeutralTranslations(),
            propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.Translations = (ITranslations)e.NewValue)));

        /// <summary>  
        ///  Property showing whether a filtered is applied in the column
        /// </summary>
        public bool IsFiltered
        {
            get { return (bool)GetValue(IsFilteredProperty); }
            set { SetValue(IsFilteredProperty, value); }
        }

        public static readonly DependencyProperty IsFilteredProperty = DependencyProperty.Register(
            "IsFiltered", typeof(bool), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(false,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.IsFiltered = (bool)e.NewValue)));

        /// <summary>  
        ///  Property for the Column Header Text
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof(string), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f .HeaderTextBlock.Text = (string)e.NewValue)));

        /// <summary>  
        ///  Property for the current size of the text used inside the header
        /// </summary>
        public double HeaderTextSize
        {
            get { return (double)GetValue(HeaderTextSizeProperty); }
            set { SetValue(HeaderTextSizeProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextSizeProperty = DependencyProperty.Register(
            "HeaderTextSize", typeof(double), typeof(FilterableDataGridColumnHeader),
            new PropertyMetadata(17.0, propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.HeaderTextBlock.FontSize = (double)e.NewValue)));

        /// <summary>  
        ///  Property for the Filter Property we are filtering for
        /// </summary>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            "PropertyName", typeof(string), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(string.Empty, 
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => f.PropertyName = (string)e.NewValue)));

        private static void HandlePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e, Action<FilterableDataGridColumnHeader, DependencyPropertyChangedEventArgs> action)
        {
            if(d is not FilterableDataGridColumnHeader f)
            {
                throw new InvalidOperationException("Cannot cast DependencyObject to the FilterableDataGridColumnHeader");
            }

            action(f, e);
        }

        #endregion Dependency Properties

        public FilterableDataGridColumnHeader()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Read the Embedded Images from the current Assembly
            using (Stream BlackFilterImageStream = assembly.GetManifestResourceStream("WpfDataGridFilter.Resources.Pictures.FilterBlack.png")!,
                RedFilterImageStream = assembly.GetManifestResourceStream("WpfDataGridFilter.Resources.Pictures.FilterRed.png")!,
                LayoutStream = assembly.GetManifestResourceStream("WpfDataGridFilter.Resources.Layouts.FilterableDataGridColumnHeader.xaml")!)
            {
                BitmapImage ImageBlackFilterBitmap = new BitmapImage();

                ImageBlackFilterBitmap.BeginInit();
                ImageBlackFilterBitmap.StreamSource = BlackFilterImageStream;
                ImageBlackFilterBitmap.EndInit();

                ImageFilterBlack = new Image();
                ImageFilterBlack.Source = ImageBlackFilterBitmap;

                BitmapImage ImageRedFilterBitmap = new BitmapImage();

                ImageRedFilterBitmap.BeginInit();
                ImageRedFilterBitmap.StreamSource = RedFilterImageStream;
                ImageRedFilterBitmap.EndInit();

                ImageFilterRed = new Image();
                ImageFilterRed.Source = ImageRedFilterBitmap;

                StreamReader LayoutStreamReader = new StreamReader(LayoutStream);

                RootObject = (DependencyObject)XamlReader.Load(LayoutStreamReader.BaseStream);
            }

            HeaderBorder = (Border)LogicalTreeHelper.FindLogicalNode(RootObject, "ControlBorder");

            // Header Text
            HeaderTextBlock = (TextBlock)LogicalTreeHelper.FindLogicalNode(RootObject, "ControlTextBlock");
            HeaderTextBlock.MinWidth = 75;

            // Sort Icons
            SortArrowNone = (Path)LogicalTreeHelper.FindLogicalNode(RootObject, "SortArrowNone");
            SortArrowAsc = (Path)LogicalTreeHelper.FindLogicalNode(RootObject, "SortArrowAsc");
            SortArrowDesc = (Path)LogicalTreeHelper.FindLogicalNode(RootObject, "SortArrowDesc");
            
            SortButton = (Button)LogicalTreeHelper.FindLogicalNode(RootObject, "SortButton");

            // Configure the Toggle Button for opening and closing the Filter Popup
            HeaderToggle = (ToggleButton)LogicalTreeHelper.FindLogicalNode(RootObject, "ControlToggle");
            HeaderToggle.Content = ImageFilterBlack;
            HeaderToggle.Visibility = Visibility.Visible;

            SortButton.Click += delegate (object sender, RoutedEventArgs e) 
            {
                DataGridState.SetSortColumn(new SortColumn(PropertyName, GetNextSortDirection()));
            };

            // Close the Popup, if we uncheck the Filter Toggle
            HeaderToggle.Unchecked += delegate (object sender, RoutedEventArgs rea)
            {
                if (HeaderPopUp != null)
                {
                    HeaderPopUp.IsOpen = false;
                }
            };

            // Open the Popup, if we uncheck the Filter Toggle
            HeaderToggle.Checked += delegate (object sender, RoutedEventArgs rea)
            {
                if (HeaderPopUp == null)
                {
                    CreateHeaderPopUp();
                }

                if (HeaderPopUp != null)
                {
                    HeaderPopUp.IsOpen = true;
                }
            };

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            Content = RootObject;
        }

        /// <summary>
        /// Creates the Popup with the Filter Control included.
        /// </summary>
        private void CreateHeaderPopUp()
        {
            // Build the Filter Controls:
            UserControl filterControl = CreateFilterControl();

            // Add a nice border around the Filter Control
            Border filterControlAroundBorder = new Border();

            filterControlAroundBorder.Background = new SolidColorBrush(Colors.White);
            filterControlAroundBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            filterControlAroundBorder.BorderThickness = new Thickness(1);
            filterControlAroundBorder.Child = filterControl;

            // Build the Popup:
            HeaderPopUp = new Popup();

            HeaderPopUp.Closed += delegate (object? sender, EventArgs ea)
            {
                if (!HeaderToggle.IsPressed)
                {
                    HeaderToggle.IsChecked = false;
                }
            };

            HeaderPopUp.PlacementTarget = this;
            HeaderPopUp.AllowsTransparency = false;
            HeaderPopUp.Child = filterControlAroundBorder;
            HeaderPopUp.Placement = PlacementMode.Bottom;
            HeaderPopUp.Width = 400;
            HeaderPopUp.MinWidth = 400;
            HeaderPopUp.PopupAnimation = PopupAnimation.Slide;
            HeaderPopUp.StaysOpen = false;
        }

        private UserControl CreateFilterControl()
        {
            switch (FilterType)
            {
                case FilterTypeEnum.BooleanFilter:
                    return new BooleanFilter(PropertyName, Translations, DataGridState);
                case FilterTypeEnum.StringFilter:
                    return new StringFilter(PropertyName, Translations, DataGridState);
                case FilterTypeEnum.DateTimeFilter:
                    return new DateTimeFilter(PropertyName, Translations, DataGridState);
                case FilterTypeEnum.IntNumericFilter:
                    return new IntNumericFilter(PropertyName, Translations, DataGridState);
                case FilterTypeEnum.DoubleNumericFilter:
                    return new DoubleNumericFilter(PropertyName, Translations, DataGridState);
                default:
                    throw new InvalidOperationException($"Filter Type '{FilterType}' is not supported");
            }
        }

        private SortDirectionEnum? GetNextSortDirection()
        {
            switch(ColumnSortDirection)
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

        private void OnColumnSortChanged(SortDirectionEnum? sortDirection)
        {
            // Hide all:
            SortArrowAsc!.Visibility = Visibility.Collapsed;
            SortArrowDesc!.Visibility = Visibility.Collapsed;
            SortArrowNone!.Visibility = Visibility.Collapsed;

            // And show the current one
            switch(sortDirection)
            {
                case null:
                    SortArrowNone.Visibility = Visibility.Visible;
                    break;
                case SortDirectionEnum.Ascending:
                    SortArrowAsc.Visibility = Visibility.Visible;
                    break;
                case SortDirectionEnum.Descending:
                    SortArrowDesc.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
