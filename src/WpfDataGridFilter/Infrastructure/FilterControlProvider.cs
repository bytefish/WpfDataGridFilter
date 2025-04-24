using System.Windows.Controls;
using WpfDataGridFilter.Controls;

namespace WpfDataGridFilter.Infrastructure
{
    public interface IFilterControlProvider
    {
        /// <summary>
        /// Creates a new <see cref="IFilterControl"> for the given Fitler Type.
        /// </summary>
        /// <param name="filterType">Filter Type to create</param>
        /// <returns>The <see cref="IFilterControl"/></returns>
        FilterControl CreateFilterControl(string name);
    }

    /// <summary>
    /// Default <see cref="IFilterControlProvider"> implementation.
    /// </summary>
    public class FilterControlProvider : IFilterControlProvider
    {
        private readonly Dictionary<string, Func<FilterControl>> _filterControls = [];

        public FilterControlProvider()
        {
            _filterControls.Add("BooleanFilter", () => new BooleanFilterControl());
            _filterControls.Add("DateTimeFilter", () => new DateTimeFilterControl());
            _filterControls.Add("IntNumericFilter", () => new IntNumericFilterControl());
            _filterControls.Add("DoubleNumericFilter", () => new DoubleNumericFilterControl());
            _filterControls.Add("StringFilter", () => new StringFilterControl());
        }

        public void AddFilterControl(string name, Func<FilterControl> filterControlFunc)
        {
            if (_filterControls.ContainsKey(name))
            {
                throw new InvalidOperationException($"The Provider already contains a Filter Control for '{name}'");
            }

            _filterControls.Add(name, filterControlFunc);
        }

        public FilterControl CreateFilterControl(string filterType)
        {
            if(!_filterControls.TryGetValue(filterType, out var filterControlFunc))
            {
                throw new InvalidOperationException($"No Filter Control for Name '{filterType}' available");
            }

            return filterControlFunc.Invoke();
        }
    }
}