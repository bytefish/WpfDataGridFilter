// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The Provider is used to provide Filter Translators to use in the library.
    /// </summary>
    public interface IFilterTranslatorProvider
    {
        /// <summary>
        /// Returns the Filter Translator by its name.
        /// </summary>
        /// <param name="name">Filter Type Name</param>
        /// <returns>The Translator, if any</returns>
        IFilterTranslator GetFilterTranslatorByName(string name);
    }

}
