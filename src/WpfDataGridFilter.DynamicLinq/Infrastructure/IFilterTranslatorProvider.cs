// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The Provider is used to provide Filter Translators to use in the library.
    /// </summary>
    public interface IFilterTranslatorProvider
    {
        /// <summary>
        /// Returns the Filter Translator.
        /// </summary>
        /// <param name="filterType">Filter Type</param>
        /// <returns>The Translator, if any</returns>
        IFilterTranslator GetFilterTranslator(FilterType filterType);
    }

}
