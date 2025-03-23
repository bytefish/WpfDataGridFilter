using System.IO;

namespace WpfDataGridFilter.Example.Models;

public class Person
{
    public required int PersonID { get; set; }

    public required string FullName { get; set; }

    public required string PreferredName { get; set; }
    
    public required string SearchName { get; set; }
    
    public required string IsPermittedToLogon { get; set; }

    public required string? LogonName { get; set; }

    public required bool IsExternalLogonProvider { get; set; }

    public required bool IsSystemUser { get; set; }

    public required bool IsEmployee { get; set; }

    public required bool IsSalesperson { get; set; }

    public required string? PhoneNumber { get; set; }

    public required string? FaxNumber { get; set; }

    public required string? EmailAddress { get; set; }

    public required DateTime ValidFrom { get; set; }

    public required DateTime ValidTo { get; set; }
}

public static class MockData
{
    /// <summary>
    /// Mock Data File Path.
    /// </summary>
    public static readonly string CsvFilename = Path.Combine(AppContext.BaseDirectory, "Assets", "people.csv");

    /// <summary>
    /// Mock Data.
    /// </summary>
    public static List<Person> People = CsvReader.GetFromFile(CsvFilename);
}

public static class CsvReader
{
    public static List<Person> GetFromFile(string path)
    {
        return File.ReadLines(path)
            .Skip(1) // Skip Header
            .Select(x => x.Split(',')) // Split into Components
            .Select(x => Convert(x)) // Convert to the C# class
            .ToList();
    }

    public static Person Convert(string[] values)
    {
        return new Person
        {
            PersonID = int.Parse(values[0]),
            FullName = values[1],
            PreferredName = values[2],
            SearchName = values[3],
            IsPermittedToLogon = values[4],
            LogonName = values[5],
            IsExternalLogonProvider = int.Parse(values[6]) == 1 ? true : false,
            IsSystemUser = int.Parse(values[7]) == 1 ? true : false,
            IsEmployee = int.Parse(values[8]) == 1 ? true : false,
            IsSalesperson = int.Parse(values[9]) == 1 ? true : false,
            PhoneNumber = values[10],
            FaxNumber = values[11],
            EmailAddress = values[12],
            ValidFrom = DateTime.Parse(values[13]),
            ValidTo = DateTime.Parse(values[14]),
        };
    }
}

