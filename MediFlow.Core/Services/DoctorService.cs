using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;
using Microsoft.Data.SqlClient;

namespace MediFlow.Core.Services;

public class DoctorService : DbService, IDoctorService
{
    public DoctorService(string connectionString) : base(connectionString) { }

    public async Task<List<Doctor>> GetAllAsync()
    {
        var list = new List<Doctor>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT * FROM Doctors ORDER BY FullName", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<List<Doctor>> SearchAsync(string keyword, string specialization, bool? isAvailable)
    {
        var list = new List<Doctor>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        var sql = "SELECT * FROM Doctors WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(keyword))
            sql += " AND (FullName LIKE @kw OR Email LIKE @kw OR DoctorId LIKE @kw)";
        if (!string.IsNullOrWhiteSpace(specialization) && specialization != "All")
            sql += " AND Specialization = @spec";
        if (isAvailable.HasValue)
            sql += " AND IsAvailable = @avail";
        sql += " ORDER BY FullName";
        using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(keyword))
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        if (!string.IsNullOrWhiteSpace(specialization) && specialization != "All")
            cmd.Parameters.AddWithValue("@spec", specialization);
        if (isAvailable.HasValue)
            cmd.Parameters.AddWithValue("@avail", isAvailable.Value);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<Doctor?> GetByIdAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT * FROM Doctors WHERE DoctorId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task AddAsync(Doctor d)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"INSERT INTO Doctors
            (DoctorId, FullName, Specialization, Phone, Email, IsAvailable, YearsOfExperience, Qualification)
            VALUES (@id, @name, @spec, @phone, @email, @avail, @yoe, @qual)", conn);
        cmd.Parameters.AddWithValue("@id", NewId());
        cmd.Parameters.AddWithValue("@name", d.FullName);
        cmd.Parameters.AddWithValue("@spec", d.Specialization);
        cmd.Parameters.AddWithValue("@phone", (object?)d.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)d.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@avail", d.IsAvailable);
        cmd.Parameters.AddWithValue("@yoe", d.YearsOfExperience);
        cmd.Parameters.AddWithValue("@qual", (object?)d.Qualification ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Doctor d)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"UPDATE Doctors SET
            FullName=@name, Specialization=@spec, Phone=@phone,
            Email=@email, IsAvailable=@avail, YearsOfExperience=@yoe, Qualification=@qual
            WHERE DoctorId=@id", conn);
        cmd.Parameters.AddWithValue("@id", d.DoctorId);
        cmd.Parameters.AddWithValue("@name", d.FullName);
        cmd.Parameters.AddWithValue("@spec", d.Specialization);
        cmd.Parameters.AddWithValue("@phone", (object?)d.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)d.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@avail", d.IsAvailable);
        cmd.Parameters.AddWithValue("@yoe", d.YearsOfExperience);
        cmd.Parameters.AddWithValue("@qual", (object?)d.Qualification ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("DELETE FROM Doctors WHERE DoctorId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Doctors", conn);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task<Dictionary<string, int>> GetSpecializationStatsAsync()
    {
        var dict = new Dictionary<string, int>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT Specialization, COUNT(*) as Cnt FROM Doctors GROUP BY Specialization", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dict[reader["Specialization"].ToString()!] = (int)reader["Cnt"];
        return dict;
    }

    public async Task<List<string>> GetSpecializationsAsync()
    {
        var list = new List<string>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT DISTINCT Specialization FROM Doctors ORDER BY Specialization", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(reader[0].ToString()!);
        return list;
    }

    private static Doctor Map(SqlDataReader r) => new()
    {
        DoctorId          = r["DoctorId"].ToString()!,
        FullName          = r["FullName"].ToString()!,
        Specialization    = r["Specialization"].ToString()!,
        Phone             = r["Phone"] as string ?? "",
        Email             = r["Email"] as string ?? "",
        IsAvailable       = (bool)r["IsAvailable"],
        YearsOfExperience = r["YearsOfExperience"] != DBNull.Value ? (int)r["YearsOfExperience"] : 0,
        Qualification     = r["Qualification"] as string ?? ""
    };
}
