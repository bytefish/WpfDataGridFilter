// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfDataGridFilter.Models
{
    /// <summary>
    /// Filter Descriptor to filter for a property.
    /// </summary>
    public abstract class FilterDescriptor
    {
        /// <summary>
        /// Gets or sets the Property to filter.
        /// </summary>
        public required string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the Filter Operator.
        /// </summary>
        public required FilterOperatorEnum FilterOperator { get; set; }

        /// <summary>
        /// Gets or sets the Filter Type.
        /// </summary>
        public abstract string FilterType { get; }
    }

    /// <summary>
    /// A Boolean Filter to filter for Boolean values.
    /// </summary>
    public class BooleanFilterDescriptor : FilterDescriptor
    {
        /// <summary>
        /// Gets the Filter Type.
        /// </summary>
        public override string FilterType => FilterTypes.BooleanFilter;
    }

    /// <summary>
    /// A String Filter to filter for text.
    /// </summary>
    public class StringFilterDescriptor : FilterDescriptor
    {
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets the Filter Type.
        /// </summary>
        public override string FilterType => FilterTypes.StringFilter;
    }

    /// <summary>
    /// Numeric Filter to filter between an lower and upper value.
    /// </summary>
    public class IntNumericFilterDescriptor : FilterDescriptor
    {
        /// <summary>
        /// Gets or sets the lower value.
        /// </summary>
        public int? LowerValue { get; set; }

        /// <summary>
        /// Gets or sets the upper value.
        /// </summary>
        public int? UpperValue { get; set; }

        /// <summary>
        /// Gets the Filter Type.
        /// </summary>
        public override string FilterType => FilterTypes.IntNumericFilter;
    }

    /// <summary>
    /// Numeric Filter to filter between an lower and upper value.
    /// </summary>
    public class DoubleNumericFilterDescriptor : FilterDescriptor
    {
        /// <summary>
        /// Gets or sets the lower value.
        /// </summary>
        public double? LowerValue { get; set; }

        /// <summary>
        /// Gets or sets the upper value.
        /// </summary>
        public double? UpperValue { get; set; }

        /// <summary>
        /// Gets the Filter Type.
        /// </summary>
        public override string FilterType => FilterTypes.DoubleNumericFilter;
    }

    /// <summary>
    /// Date Range Filter to filter between a start and end date.
    /// </summary>
    public class DateTimeFilterDescriptor : FilterDescriptor
    {
        /// <summary>
        /// Start Date for range filtering.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End Date for range filtering.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets the Filter Type.
        /// </summary>
        public override string FilterType => FilterTypes.DateTimeFilter;
    }
}
