namespace AppWorkWithDB.Database.Services;

using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

public class AdoNetService
{
    private readonly string _connectionString;

    public AdoNetService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public DataTable GetPublisherBookCounts(int minBooks)
    {
        var sql = @"
            SELECT p.Name, COUNT(b.BookID) AS BookCount
            FROM Publishers p
            LEFT JOIN Books b ON p.PublisherID = b.PublisherID
            GROUP BY p.Name
            HAVING COUNT(b.BookID) > @minBooks
            ORDER BY BookCount DESC";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@minBooks", minBooks);

        var adapter = new SqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public int BulkUpdatePrice(decimal threshold, decimal percent)
    {
        var sql = "UPDATE Books SET Price = Price * (1 + @percent/100) WHERE Price > @threshold";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@percent", percent);
        command.Parameters.AddWithValue("@threshold", threshold);

        connection.Open();
        return command.ExecuteNonQuery();
    }

    public DataTable GetAuthorsWithBookCount()
    {
        var sql = @"
            SELECT a.FirstName + ' ' + a.LastName AS AuthorName, COUNT(ba.BookID) AS BookCount
            FROM Authors a
            LEFT JOIN BookAuthors ba ON a.AuthorID = ba.AuthorID
            GROUP BY a.FirstName, a.LastName
            ORDER BY BookCount DESC";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }
}