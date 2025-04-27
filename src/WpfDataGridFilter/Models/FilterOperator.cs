namespace WpfDataGridFilter.Models
{
    /// <summary>
    /// A Filter Operator.
    /// </summary>
    public record FilterOperator
    {
        /// <summary>
        /// The Name of the Operator.
        /// </summary>
        public required string Name { get; init; }

        public static readonly FilterOperator None = new () { Name = nameof(None) };
        public static readonly FilterOperator Before = new() { Name = nameof(Before) };
        public static readonly FilterOperator After = new() { Name = nameof(After) };
        public static readonly FilterOperator IsEqualTo = new() { Name = nameof(IsEqualTo) };
        public static readonly FilterOperator IsNotEqualTo = new() { Name = nameof(IsNotEqualTo) };
        public static readonly FilterOperator Contains = new() { Name = nameof(Contains) };
        public static readonly FilterOperator NotContains = new() { Name = nameof(NotContains) };
        public static readonly FilterOperator StartsWith = new() { Name = nameof(StartsWith) };
        public static readonly FilterOperator EndsWith = new() { Name = nameof(EndsWith) };
        public static readonly FilterOperator IsNull = new() { Name = nameof(IsNull) };
        public static readonly FilterOperator IsNotNull = new() { Name = nameof(IsNotNull) };
        public static readonly FilterOperator IsEmpty = new() { Name = nameof(IsEmpty) };
        public static readonly FilterOperator IsNotEmpty = new() { Name = nameof(IsNotEmpty) };
        public static readonly FilterOperator IsGreaterThanOrEqualTo = new() { Name = nameof(IsGreaterThanOrEqualTo) };
        public static readonly FilterOperator IsGreaterThan = new() { Name = nameof(IsGreaterThan) };
        public static readonly FilterOperator IsLessThanOrEqualTo = new() { Name = nameof(IsLessThanOrEqualTo) };
        public static readonly FilterOperator IsLessThan = new() { Name = nameof(IsLessThan) };
        public static readonly FilterOperator BetweenInclusive = new() { Name = nameof(BetweenInclusive) };
        public static readonly FilterOperator BetweenExclusive = new() { Name = nameof(BetweenExclusive) };
        public static readonly FilterOperator Yes = new() { Name = nameof(Yes) };
        public static readonly FilterOperator No = new() { Name = nameof(No) };
        public static readonly FilterOperator All = new() { Name = nameof(All) };
        public static readonly FilterOperator IsNullOrWhitespace = new() { Name = nameof(IsNullOrWhitespace) };
        public static readonly FilterOperator IsNotNullOrWhitespace = new() { Name = nameof(IsNotNullOrWhitespace) };
    }
}
