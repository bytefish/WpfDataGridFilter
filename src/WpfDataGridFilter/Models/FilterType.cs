// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfDataGridFilter.Models
{
    public record FilterType
    {
        public required string Name { get; init; }

        public static FilterType BooleanFilter = new() { Name = nameof(BooleanFilter) };
        public static FilterType StringFilter = new() { Name = nameof(StringFilter) };
        public static FilterType IntNumericFilter = new() { Name = nameof(IntNumericFilter) };
        public static FilterType DoubleNumericFilter = new() { Name = nameof(DoubleNumericFilter) };
        public static FilterType DateTimeFilter = new() { Name = nameof(DateTimeFilter) };
    }
}
