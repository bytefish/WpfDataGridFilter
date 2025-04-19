using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace WpfDataGridFilter.Example.Controls
{
    [TemplatePart(Name = "PART_CurrentPage", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_FirstPage", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_LastPage", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_PreviousPage", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_NextPage", Type = typeof(ButtonBase))]
    public class PaginationControl : Control
    {
        #region Fields

        private static readonly int[] DefaultPageSizes = { 10 };

        #endregion

        #region Contructors

        static PaginationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationControl),
                new FrameworkPropertyMetadata(typeof(PaginationControl)));
        }

        #endregion

        #region FirstPage command

        public static readonly DependencyProperty FirstPageProperty = DependencyProperty.Register(
            "FirstPage", typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(default(ICommand),
                (o, args) =>
                {
                    ((PaginationControl)o).BindCommand("PART_FirstPage");
                }));

        public ICommand FirstPage
        {
            get { return (ICommand)GetValue(FirstPageProperty); }
            set { SetValue(FirstPageProperty, value); }
        }

        #endregion

        #region End command

        public static readonly DependencyProperty LastPagePropertry = DependencyProperty.Register(
            "LastPage", typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(default(ICommand), (o, args) =>
            {
                ((PaginationControl)o).BindCommand("PART_LastPage");
            }));

        public ICommand LastPage
        {
            get { return (ICommand)GetValue(LastPagePropertry); }
            set { SetValue(LastPagePropertry, value); }
        }

        #endregion

        #region Previous page command

        public static readonly DependencyProperty PreviousPageProperty = DependencyProperty.Register(
            "PreviousPage", typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(default(ICommand),
                (o, args) =>
                {
                    ((PaginationControl)o).BindCommand("PART_PreviousPage");
                }));

        public ICommand PreviousPage
        {
            get { return (ICommand)GetValue(PreviousPageProperty); }
            set { SetValue(PreviousPageProperty, value); }
        }

        #endregion

        #region Next page command

        public static readonly DependencyProperty NextPageProperty = DependencyProperty.Register(
            "NextPage", typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(default(ICommand),
                (o, args) =>
                {
                    ((PaginationControl)o).BindCommand("PART_NextPage");
                }));

        public ICommand NextPage
        {
            get { return (ICommand)GetValue(NextPageProperty); }
            set { SetValue(NextPageProperty, value); }
        }

        #endregion


        #region Current page

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(
            "CurrentPage", typeof(int), typeof(PaginationControl),
            new PropertyMetadata(0));

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        #endregion

        #region Pages count

        public static readonly DependencyProperty PagesCountProperty = DependencyProperty.Register(
            "PagesCount", typeof(int), typeof(PaginationControl), new PropertyMetadata(default(int)));

        public int PagesCount
        {
            get { return (int)GetValue(PagesCountProperty); }
            set { SetValue(PagesCountProperty, value); }
        }

        #endregion

        #region PageSizes

        public static readonly DependencyProperty PageSizesProperty = DependencyProperty.Register(
            "PageSizes", typeof(IEnumerable<int>), typeof(PaginationControl),
            new PropertyMetadata(DefaultPageSizes));

        public IEnumerable<int> PageSizes
        {
            get { return (IEnumerable<int>)GetValue(PageSizesProperty); }
            set { SetValue(PageSizesProperty, value); }
        }

        #endregion

        #region Selected PageSize

        public static readonly DependencyProperty SelectedPageSizeProperty = DependencyProperty.Register(
            "SelectedPageSize", typeof(int), typeof(PaginationControl), new PropertyMetadata(DefaultPageSizes[0]));

        public int SelectedPageSize
        {
            get { return (int)GetValue(SelectedPageSizeProperty); }
            set { SetValue(SelectedPageSizeProperty, value); }
        }

        #endregion

        #region Overridden methods

        public override void OnApplyTemplate()
        {
            BindCommand("PART_FirstPage");
            BindCommand("PART_LastPage");
            BindCommand("PART_PreviousPage");
            BindCommand("PART_NextPage");
            BindPageSizes();
            BindSelectedPageSize();
        }

        #endregion

        #region Private methods

        private void BindCommand(string partName)
        {
            ButtonBase? btn = GetTemplateChild(partName) as ButtonBase;
            if (btn == null)
            {
                return;
            }

            Binding? binding = null;

            switch (partName)
            {
                case "PART_FirstPage":
                    if (FirstPage == null)
                        return;
                    binding = new Binding("FirstPage");
                    break;
                case "PART_LastPage":
                    if (LastPage == null)
                        return;
                    binding = new Binding("LastPage");
                    break;
                case "PART_PreviousPage":
                    if (PreviousPage == null)
                        return;
                    binding = new Binding("PreviousPage");
                    break;
                case "PART_NextPage":
                    if (NextPage == null)
                        return;
                    binding = new Binding("NextPage");
                    break;
                default:
                    throw new InvalidOperationException($"Could not bind to '{partName}'");
            }

            binding.Source = this;
            binding.Mode = BindingMode.OneTime;

            btn.SetBinding(ButtonBase.CommandProperty, binding);
        }

        private void BindPageSizes()
        {
            Selector? selector = GetTemplateChild("PART_PageSizeSelector") as Selector;

            if (selector == null || PageSizes == null)
            {
                return;
            }

            var binding = new Binding("PageSizes")
            {
                Source = this,
                Mode = BindingMode.OneWay
            };

            selector.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void BindSelectedPageSize()
        {
            Selector? selector = GetTemplateChild("PART_PageSizeSelector") as Selector;

            if (selector == null)
            {
                return;
            }

            Binding binding = new Binding("SelectedPageSize")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };

            selector.SetBinding(Selector.SelectedItemProperty, binding);
        }

        #endregion
    }
}