using System.Linq.Expressions;
using WpfDataGridFilter.Filters;
using WpfDataGridFilter.Filters.Models;
using WpfDataGridFilter.Tests.Filters;

namespace WpfDataGridFilter.Tests
{
    [TestClass]
    public sealed class ColumnFilterTests
    {
        private class Person
        {
            public required int Id { get; set; }

            public required string? Name { get; set; }

            public required DateTime? BirthDate { get; set; }

            public required bool? RockstarProgrammer { get; set; }
        }

        // Test Data
        private static List<Person> GetPeople()
        {
            return
            [
                new Person { Id = 1, Name = null, BirthDate = null, RockstarProgrammer = null },
                new Person { Id = 2, Name = "Philipp Wagner", BirthDate = new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = true },
                new Person { Id = 3, Name = "Ben Statham", BirthDate = new DateTime(2018, 2, 11, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = false },
                new Person { Id = 4, Name = "Max Powers", BirthDate = new DateTime(2020, 7, 24, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = false },
                new Person { Id = 5, Name = "JSON Bourne", BirthDate = new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), RockstarProgrammer = false },
            ];
        }


        /// <summary>
        /// TestData for DateTime Filters.
        /// </summary>
        public static IEnumerable<object?[]> StringData
        {
            get
            {
                return
                [
                    [ FilterOperatorEnum.IsNull, default(string?), new int[] { 1 } ],
                    [ FilterOperatorEnum.IsNotNull, default(string?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsEqualTo, "Philipp Wagner", new int[] { 2 } ],
                    [ FilterOperatorEnum.IsEqualTo, default(string?), new int[] { 1 } ],
                    [ FilterOperatorEnum.IsNotEqualTo, "Philipp Wagner", new int[] { 1, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsNotEqualTo, default(string?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.Contains, "JSON", new int[] { 5 } ],
                    [ FilterOperatorEnum.NotContains, "JSON", new int[] { 2, 3, 4 } ],
                    [ FilterOperatorEnum.IsNullOrWhitespace, default(string?), new int[] { 1 } ],
                    [ FilterOperatorEnum.IsNotNullOrWhitespace, default(string?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.StartsWith, "JS", new int[] { 5 } ],
                    [ FilterOperatorEnum.EndsWith, "ourne", new int[] { 5 } ],

                ];
            }
        }

        /// <summary>
        /// TestData for DateTime Filters.
        /// </summary>
        public static IEnumerable<object?[]> DateTimeData
        {
            get
            {
                return
                [
                    [ FilterOperatorEnum.IsNull, default(DateTime?), default(DateTime?), new int[] { 1 } ],
                    [ FilterOperatorEnum.IsNotNull, default(DateTime?), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2 } ],
                    [ FilterOperatorEnum.IsEqualTo, default(DateTime?), default(DateTime?), new int[] { 1 } ],
                    [ FilterOperatorEnum.IsNotEqualTo, default(DateTime?), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsNotEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 1, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsGreaterThan, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsGreaterThanOrEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.IsLessThan, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { } ],
                    [ FilterOperatorEnum.IsLessThanOrEqualTo, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), default(DateTime?), new int[] { 2 } ],
                    [ FilterOperatorEnum.BetweenInclusive, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), new int[] { 2, 3, 4, 5 } ],
                    [ FilterOperatorEnum.BetweenExclusive, new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc), new int[] { 3, 4 } ],
                ];
            }
        }

        [TestMethod]
        [DynamicData(nameof(DateTimeData))]
        public void DateTimeColumnFilterTests(FilterOperatorEnum filterOperator, DateTime? startDate, DateTime? endDate, int[] expected)
        {
            DateTimeColumnFilter<Person> dateTimeColumnFilter = new DateTimeColumnFilter<Person>
            {
                ColumnName = "Birth Date",
                PropertyGetter = (x) => x.BirthDate,
                FilterOperator = filterOperator,
                StartDate = startDate,
                EndDate = endDate,
            };

            Expression<Func<Person, bool>> filterPredicate = dateTimeColumnFilter.GetFilterPredicate();

            int[] filteredResults = GetPeople().AsQueryable()
                .Where(filterPredicate)
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }

        [TestMethod]
        [DynamicData(nameof(StringData))]
        public void StringColumnFilterTests(FilterOperatorEnum filterOperator, string? value, int[] expected)
        {
            StringColumnFilter<Person> stringColumnFilter = new StringColumnFilter<Person>
            {
                ColumnName = "Name",
                PropertyGetter = (x) => x.Name,
                Value = value,
                FilterOperator = filterOperator
            };

            Expression<Func<Person, bool>> filterPredicate = stringColumnFilter.GetFilterPredicate();

            int[] filteredResults = GetPeople().AsQueryable()
                .Where(filterPredicate)
                .Select(x => x.Id)
                .ToArray();

            Assert.AreEqual(true, Enumerable.SequenceEqual(filteredResults, expected));
        }

    }
}
