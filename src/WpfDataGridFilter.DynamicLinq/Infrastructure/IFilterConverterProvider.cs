using WpfDataGridFilter.DynamicLinq.Converters;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The FilterConverterProvider is used to provider Filter Converters to use in the library.
    /// </summary>
    public interface IFilterConverterProvider
    {
        /// <summary>
        /// Returns the Filter Converter by its name.
        /// </summary>
        /// <param name="name">Filter Type Name</param>
        /// <returns>FilterConverter, if any</returns>
        FilterConverter GetFilterConverterByName(string name);
    }

}
