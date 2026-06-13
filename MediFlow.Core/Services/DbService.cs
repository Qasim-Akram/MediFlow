using Microsoft.Data.SqlClient;

namespace MediFlow.Core.Services;

public abstract class DbService
{
    protected readonly string _connectionString;

    protected DbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected SqlConnection CreateConnection() => new SqlConnection(_connectionString);

    protected static string NewId() => Guid.NewGuid().ToString("N")[..12].ToUpper();
}
