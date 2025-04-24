// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Infrastructure
{
    /// <summary>
    /// Interface for a Filter Control.
    /// </summary>
    public interface IFilterControl
    {
        /// <summary>
        /// Name of the Filter Control.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Property Name to filter for.
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// DataGridState.
        /// </summary>
        DataGridState DataGridState { get; set; }

        /// <summary>
        /// Translations the Filter Control uses.
        /// </summary>
        ITranslations Translations { get; set; }

        /// <summary>
        /// Gets the Filter Descriptor for this Filter Control.
        /// </summary>
        FilterDescriptor FilterDescriptor { get; }        
    }
}
