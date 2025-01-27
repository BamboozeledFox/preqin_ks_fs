using System.IO;

namespace CommitmentsService.ETL;

public interface IFileReader
{
    string[] ReadLines(string fileLocation);
}

internal class FileReader : IFileReader
{
    public string[] ReadLines(string fileLocation)
    {
        return File.ReadAllLines(fileLocation);
    }
}
