using Microsoft.Data.Sqlite;

namespace CommitmentsService.Persistence;

public class DbConnectionProvider
{
    private readonly string _connectionString;

    public DbConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteConnection GetOpenConnectionString()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
