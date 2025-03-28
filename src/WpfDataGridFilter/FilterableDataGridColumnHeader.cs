﻿
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfDataGridFilter.Filters;
using WpfDataGridFilter.Filters.Controls;
using WpfDataGridFilter.Filters.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter
{
    public class FilterableDataGridColumnHeader : DataGridColumnHeader
    {
        public static Thickness StandardHeaderTextBoxBorderThickness = new Thickness(0.5, 1, 0, 1);

        // Header Element
        DependencyObject RootObject;

        // Header Layout Elements
        Border HeaderBorder;

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
        public FilterState FilterState
        {
            get { return (FilterState)GetValue(FilterStateProperty); }
            set { SetValue(FilterStateProperty, value); }
        }

        public static readonly DependencyProperty FilterStateProperty = DependencyProperty.Register(
            "FilterState", typeof(FilterState), typeof(FilterableDataGridColumnHeader), new PropertyMetadata(null,
                propertyChangedCallback: (d, e) => HandlePropertyChange(d, e, (f, e) => 
                {
                    f.FilterState = (FilterState)e.NewValue;

                    // Wow, this is super ugly. This needs to be designed much better.
                    f.FilterState.FilterStateChanged += delegate (object? sender, FilterStateChangedEventArgs filterStateChangedEventArgs)
                    {
                        f.IsFiltered = filterStateChangedEventArgs.FilterState.Filters.ContainsKey(f.PropertyName);

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

            HeaderTextBlock = (TextBlock)LogicalTreeHelper.FindLogicalNode(RootObject, "ControlTextBlock");
            HeaderTextBlock.MinWidth = 75;

            // Configure the Toggle Button for opening and closing the Filter Popup
            HeaderToggle = (ToggleButton)LogicalTreeHelper.FindLogicalNode(RootObject, "ControlToggle");
            HeaderToggle.Content = ImageFilterBlack;
            HeaderToggle.Visibility = Visibility.Visible;

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
            HeaderPopUp.StaysOpen = true;
        }

        private UserControl CreateFilterControl()
        {
            switch (FilterType)
            {
                case FilterTypeEnum.BooleanFilter:
                    return new BooleanFilter(PropertyName, Translations, FilterState);
                case FilterTypeEnum.StringFilter:
                    return new StringFilter(PropertyName, Translations, FilterState);
                case FilterTypeEnum.DateFilter:
                    return new DateFilter(PropertyName, Translations, FilterState);
                case FilterTypeEnum.IntNumericFilter:
                    return new IntNumericFilter(PropertyName, Translations, FilterState);
                case FilterTypeEnum.DoubleNumericFilter:
                    return new DoubleNumericFilter(PropertyName, Translations, FilterState);
                default:
                    throw new InvalidOperationException($"Filter Type '{FilterType}' is not supported");
            }
        }
    }
}
