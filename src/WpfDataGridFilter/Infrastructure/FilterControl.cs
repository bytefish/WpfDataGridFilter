// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Controls;
using WpfDataGridFilter.Models;
using WpfDataGridFilter.Translations;

namespace WpfDataGridFilter.Infrastructure
{
    /// <summary>
    /// Base class of all <see cref="IFilterControl"/> implementations.
    /// </summary>
    public abstract class FilterControl : Control, IFilterControl
    {
        /// <summary>
        /// Gets or sets the Property.
        /// </summary>
        public abstract string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the DataGrid State.
        /// </summary>
        public abstract DataGridState DataGridState { get; set; }
        
        /// <summary>
        /// Gets or sets the Translations.
        /// </summary>
        public abstract ITranslations Translations { get; set; }

        /// <summary>
        /// Gets or sets the FilterDescriptor.
        /// </summary>
        public abstract FilterDescriptor FilterDescriptor { get;  }
    }
}
