using System.Linq.Expressions;
using WpfDataGridFilter.Filters;

namespace WpfDataGridFilter.Tests
{
    [TestClass]
    public sealed class ColumnFilterTests
    {
        private class Person
        {
            public required string Name { get; set; }

            public required DateTime BirthDate { get; set; }
        }

        [TestMethod]
        public void TestMethod1()
        {
            DateTimeColumnFilter<Person> dateTimeColumnFilter = new DateTimeColumnFilter<Person>
            {
                ColumnName = "Birth Date",
                PropertyGetter = (x) => x.BirthDate,
                StartDate = new DateTime(2018, 1, 1, 0, 0, 0),
                FilterOperator = Filters.Models.FilterOperatorEnum.IsGreaterThan
            };

            Expression<Func<Person, bool>> filterPredicate = dateTimeColumnFilter.GetFilterPredicate();

            // Test Data
            List<Person> people =
            [
                new Person { Name = "Philipp Wagner", BirthDate = new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc)},
                new Person { Name = "Ben Statham", BirthDate = new DateTime(2018, 2, 11, 0, 0, 0, DateTimeKind.Utc)},
                new Person { Name = "Max Powers", BirthDate = new DateTime(2020, 7, 24, 0, 0, 0, DateTimeKind.Utc)},
                new Person { Name = "JSON Bourne", BirthDate = new DateTime(2022, 5, 22, 0, 0, 0, DateTimeKind.Utc)},
            ];

            List<Person> filteredResults = people.AsQueryable()
                .Where(filterPredicate)
                .ToList();

        }
    }
}
