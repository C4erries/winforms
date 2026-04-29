using System.Data;
using Npgsql;

namespace StationeryStore.Data;

public sealed class Db : IDisposable
{
    private readonly NpgsqlConnection _connection;

    public Db()
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Username=postgres;Password=12345;Database=stationery";

        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public DataTable Query(string sql, params NpgsqlParameter[] parameters)
    {
        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddRange(parameters);
        using var adapter = new NpgsqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public object? Scalar(string sql, params NpgsqlParameter[] parameters)
    {
        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddRange(parameters);
        return command.ExecuteScalar();
    }

    public int Execute(string sql, params NpgsqlParameter[] parameters)
    {
        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddRange(parameters);
        return command.ExecuteNonQuery();
    }

    public void Dispose() => _connection.Dispose();
}
