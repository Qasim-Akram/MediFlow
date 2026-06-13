using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;
using Microsoft.Data.SqlClient;

namespace MediFlow.Core.Services;

public class PatientService : DbService, IPatientService
{
    public PatientService(string connectionString) : base(connectionString) { }

    public async Task<List<Patient>> GetAllAsync()
    {
        var list = new List<Patient>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT * FROM Patients ORDER BY RegisteredOn DESC", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<List<Patient>> SearchAsync(string keyword, string gender, string bloodGroup)
    {
        var list = new List<Patient>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        var sql = "SELECT * FROM Patients WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(keyword))
            sql += " AND (FullName LIKE @kw OR Phone LIKE @kw OR PatientId LIKE @kw)";
        if (!string.IsNullOrWhiteSpace(gender) && gender != "All")
            sql += " AND Gender = @gender";
        if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "All")
            sql += " AND BloodGroup = @bg";
        sql += " ORDER BY RegisteredOn DESC";
        using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(keyword))
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        if (!string.IsNullOrWhiteSpace(gender) && gender != "All")
            cmd.Parameters.AddWithValue("@gender", gender);
        if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "All")
            cmd.Parameters.AddWithValue("@bg", bloodGroup);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<Patient?> GetByIdAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT * FROM Patients WHERE PatientId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task AddAsync(Patient p)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"INSERT INTO Patients
            (PatientId, FullName, Gender, DateOfBirth, Phone, Address, BloodGroup, RegisteredOn)
            VALUES (@id, @name, @gender, @dob, @phone, @address, @bg, @reg)", conn);
        cmd.Parameters.AddWithValue("@id", NewId());
        cmd.Parameters.AddWithValue("@name", p.FullName);
        cmd.Parameters.AddWithValue("@gender", p.Gender);
        cmd.Parameters.AddWithValue("@dob", p.DateOfBirth);
        cmd.Parameters.AddWithValue("@phone", (object?)p.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@address", (object?)p.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@bg", (object?)p.BloodGroup ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@reg", p.RegisteredOn);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Patient p)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"UPDATE Patients SET
            FullName=@name, Gender=@gender, DateOfBirth=@dob,
            Phone=@phone, Address=@address, BloodGroup=@bg
            WHERE PatientId=@id", conn);
        cmd.Parameters.AddWithValue("@id", p.PatientId);
        cmd.Parameters.AddWithValue("@name", p.FullName);
        cmd.Parameters.AddWithValue("@gender", p.Gender);
        cmd.Parameters.AddWithValue("@dob", p.DateOfBirth);
        cmd.Parameters.AddWithValue("@phone", (object?)p.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@address", (object?)p.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@bg", (object?)p.BloodGroup ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("DELETE FROM Patients WHERE PatientId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Patients", conn);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task<Dictionary<string, int>> GetGenderStatsAsync()
    {
        var dict = new Dictionary<string, int>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT Gender, COUNT(*) as Cnt FROM Patients GROUP BY Gender", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dict[reader["Gender"].ToString()!] = (int)reader["Cnt"];
        return dict;
    }

    public async Task<Dictionary<string, int>> GetBloodGroupStatsAsync()
    {
        var dict = new Dictionary<string, int>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT BloodGroup, COUNT(*) as Cnt FROM Patients WHERE BloodGroup IS NOT NULL GROUP BY BloodGroup", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dict[reader["BloodGroup"].ToString()!] = (int)reader["Cnt"];
        return dict;
    }

    private static Patient Map(SqlDataReader r) => new()
    {
        PatientId    = r["PatientId"].ToString()!,
        FullName     = r["FullName"].ToString()!,
        Gender       = r["Gender"].ToString()!,
        DateOfBirth  = (DateTime)r["DateOfBirth"],
        Phone        = r["Phone"] as string ?? "",
        Address      = r["Address"] as string ?? "",
        BloodGroup   = r["BloodGroup"] as string ?? "",
        RegisteredOn = (DateTime)r["RegisteredOn"]
    };
}
