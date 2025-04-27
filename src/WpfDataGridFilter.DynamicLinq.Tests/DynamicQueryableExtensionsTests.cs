using Newtonsoft.Json.Linq;
using WpfDataGridFilter.DynamicLinq;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.Tests
{
    [TestClass]
    public sealed class ColumnFilterTests
    {
        private class Person
        {
            public required int Id { get; set; }

            public required string? Name { get; set; }

            public required DateTimeOffset? BirthDate { get; set; }

            public required DateTime? BirthDateAsDateTime { get; set; }

            public required bool? RockstarProgrammer { get; set; }

            public required int? NumberOfCars { get; set; }
        }

        // Test Data
        private static List<Person> GetPeople()
        {
            return
            [
                new Person { Id = 1, Name = null, BirthDate = null, BirthDateAsDateTime = null, RockstarProgrammer = null, NumberOfCars = null },
                new Person { Id = 2, Name = "Philipp Wagner", BirthDate = new DateTimeOffset(2000, 3, 1, 0, 0, 0, TimeSpan.Zero),  BirthDateAsDateTime = new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = true, NumberOfCars = 2 },
                new Person { Id = 3, Name = "Ben Statham", BirthDate = new DateTimeOffset(2018, 2, 11, 0, 0, 0, TimeSpan.Zero), BirthDateAsDateTime = new DateTime(2018, 2, 11, 0, 0, 0, DateTimeKind.Utc),RockstarProgrammer = false, NumberOfCars = 3 },
                new Person { Id = 4, Name = "Max Powers", BirthDate = new DateTimeOffset(2020, 7, 24, 0, 0, 0, TimeSpan.Zero), BirthDateAsDateTime = new DateTime(2020, 7, 24, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = false, NumberOfCars = 4 },
                new Person { Id = 5, Name = "JSON Bourne", BirthDate = new DateTimeOffset(2022, 5, 22, 0, 0, 0, TimeSpan.Zero), BirthDateAsDateTime = new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = false, NumberOfCars = 5 },
            ];
        }


        /// <summary>
        /// TestData for Bool Filters.
        /// </summary>
        public static IEnumerable<object?[]> BoolTestData
        {
            get
            {
                return
                [
                    [ FilterOperator.IsNull, new int[] { 1 } ],
                    [ FilterOperator.Yes, new int[] { 2 } ],
                    [ FilterOperator.No, new int[] { 3, 4, 5 } ],
                    [ FilterOperator.All, new int[] { 2, 3, 4, 5 } ],
                ];
            }
        }

        /// <summary>
        /// TestData for String Filters.
        /// </summary>
        public static IEnumerable<object?[]> StringTestData
        {
            get
            {
                return
                [
                    [ FilterOperator.IsNull, default(string?), new int[] { 1 } ],
                    [ FilterOperator.IsNotNull, default(string?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsEqualTo, "Philipp Wagner", new int[] { 2 } ],
                    [ FilterOperator.IsNotEqualTo, "Philipp Wagner", new int[] { 1, 3, 4, 5 } ],
                    [ FilterOperator.Contains, "JSON", new int[] { 5 } ],
                    [ FilterOperator.NotContains, "JSON", new int[] { 2, 3, 4 } ],
                    [ FilterOperator.IsNullOrWhitespace, default(string?), new int[] { 1 } ],
                    [ FilterOperator.IsNotNullOrWhitespace, default(string?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.StartsWith, "JS", new int[] { 5 } ],
                    [ FilterOperator.EndsWith, "ourne", new int[] { 5 } ],

                ];
            }
        }

        /// <summary>
        /// TestData for DateTime Filters.
        /// </summary>
        public static IEnumerable<object?[]> DateTimeTestData
        {
            get
            {
                return
                [
                    [ FilterOperator.IsNull, default(DateTime?), default(DateTime?), new int[] { 1 } ],
                    [ FilterOperator.IsNotNull, default(DateTime?), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2 } ],
                    [ FilterOperator.IsEqualTo, default(DateTime?), default(DateTime?), new int[] { 1 } ],
                    [ FilterOperator.IsNotEqualTo, default(DateTime?), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsNotEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 1, 3, 4, 5 } ],
                    [ FilterOperator.IsGreaterThan, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 3, 4, 5 } ],
                    [ FilterOperator.IsGreaterThanOrEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsLessThan, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { } ],
                    [ FilterOperator.IsLessThanOrEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2 } ],
                    [ FilterOperator.BetweenInclusive, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.BetweenExclusive, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), new int[] { 3, 4 } ],
                ];
            }
        }


        /// <summary>
        /// TestData for Integer Filters.
        /// </summary>
        public static IEnumerable<object?[]> IntegerTestData
        {
            get
            {
                return
                [
                    [ FilterOperator.IsNull, default(int?), default(int?), new int[] { 1 } ],
                    [ FilterOperator.IsNotNull, default(int?), default(int?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsEqualTo, 2, default(int?), new int[] { 2 } ],
                    [ FilterOperator.IsEqualTo, default(int?), default(int?), new int[] { 1 } ],
                    [ FilterOperator.IsNotEqualTo, default(int?), default(int?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsNotEqualTo, 2, default(int?), new int[] { 1, 3, 4, 5 } ],
                    [ FilterOperator.IsGreaterThan, 2, default(int?), new int[] { 3, 4, 5 } ],
                    [ FilterOperator.IsGreaterThanOrEqualTo, 2, default(int?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperator.IsLessThan, 3, default(int?), new int[] { 2 } ],
                    [ FilterOperator.IsLessThanOrEqualTo, 3, default(int?), new int[] { 2, 3 } ],
                    [ FilterOperator.BetweenInclusive, 2, 4, new int[] { 2, 3, 4 } ],
                    [ FilterOperator.BetweenExclusive, 2, 4, new int[] { 3 } ],
                ];
            }
        }

        [TestMethod]
        [DynamicData(nameof(DateTimeTestData))]
        public void DateTimeColumnFilterTests(FilterOperator filterOperator, DateTime? startDate, DateTime? endDate, int[] expected)
        {
            DateTimeFilterDescriptor filterDescriptor = new DateTimeFilterDescriptor
            {
                FilterOperator = filterOperator,
                PropertyName = nameof(Person.BirthDateAsDateTime),
                StartDate = startDate,
                EndDate = endDate,
            };

            int[] filteredResults = GetPeople().AsQueryable()
                .ApplyDataGridState(new DataGridState(filters: [filterDescriptor]))
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }


        [TestMethod]
        [DynamicData(nameof(StringTestData))]
        public void StringColumnFilterTests(FilterOperator filterOperator, string? value, int[] expected)
        {
            StringFilterDescriptor filterDescriptor = new StringFilterDescriptor
            {
                FilterOperator = filterOperator,
                PropertyName = nameof(Person.Name),
                Value = value ?? string.Empty,
            };

            int[] filteredResults = GetPeople().AsQueryable()
                .ApplyDataGridState(new DataGridState(filters: [filterDescriptor]))
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }

        [TestMethod]
        [DynamicData(nameof(IntegerTestData))]
        public void IntegerColumnFilterTests(FilterOperator filterOperator, int? lowerValue, int? upperValue, int[] expected)
        {
            IntNumericFilterDescriptor filterDescriptor = new IntNumericFilterDescriptor
            {
                FilterOperator = filterOperator,
                PropertyName = nameof(Person.NumberOfCars),
                LowerValue = lowerValue,
                UpperValue = upperValue
            };

            int[] filteredResults = GetPeople().AsQueryable()
                .ApplyDataGridState(new DataGridState(filters: [filterDescriptor]))
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }

        [TestMethod]
        [DynamicData(nameof(BoolTestData))]
        public void BoolColumnFilterTests(FilterOperator filterOperator, int[] expected)
        {
            BooleanFilterDescriptor filterDescriptor = new BooleanFilterDescriptor
            {
                FilterOperator = filterOperator,
                PropertyName = nameof(Person.RockstarProgrammer),
            };

            int[] filteredResults = GetPeople().AsQueryable()
                .ApplyDataGridState(new DataGridState(filters: [filterDescriptor]))
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }

    }
}
