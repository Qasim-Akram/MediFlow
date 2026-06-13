using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;
using Microsoft.Data.SqlClient;

namespace MediFlow.Core.Services;

public class AppointmentService : DbService, IAppointmentService
{
    public AppointmentService(string connectionString) : base(connectionString) { }

    public async Task<List<Appointment>> GetAllAsync()
    {
        var list = new List<Appointment>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"
            SELECT a.*, p.FullName AS PatientName, d.FullName AS DoctorName
            FROM Appointments a
            JOIN Patients p ON a.PatientId = p.PatientId
            JOIN Doctors  d ON a.DoctorId  = d.DoctorId
            ORDER BY a.AppointmentDate DESC", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<List<Appointment>> SearchAsync(string keyword, AppointmentStatus? status, DateTime? date)
    {
        var list = new List<Appointment>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        var sql = @"SELECT a.*, p.FullName AS PatientName, d.FullName AS DoctorName
                    FROM Appointments a
                    JOIN Patients p ON a.PatientId = p.PatientId
                    JOIN Doctors  d ON a.DoctorId  = d.DoctorId
                    WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(keyword))
            sql += " AND (p.FullName LIKE @kw OR d.FullName LIKE @kw OR a.TokenNumber LIKE @kw)";
        if (status.HasValue)
            sql += " AND a.Status = @status";
        if (date.HasValue)
            sql += " AND CAST(a.AppointmentDate AS DATE) = @date";
        sql += " ORDER BY a.AppointmentDate DESC";
        using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(keyword))
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        if (status.HasValue)
            cmd.Parameters.AddWithValue("@status", status.Value.ToString());
        if (date.HasValue)
            cmd.Parameters.AddWithValue("@date", date.Value.Date);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"
            SELECT a.*, p.FullName AS PatientName, d.FullName AS DoctorName
            FROM Appointments a
            JOIN Patients p ON a.PatientId = p.PatientId
            JOIN Doctors  d ON a.DoctorId  = d.DoctorId
            WHERE a.AppointmentId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task AddAsync(Appointment a)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"INSERT INTO Appointments
            (AppointmentId, PatientId, DoctorId, AppointmentDate, Reason, Status, Notes, TokenNumber)
            VALUES (@id, @pid, @did, @date, @reason, @status, @notes, @token)", conn);
        cmd.Parameters.AddWithValue("@id", NewId());
        cmd.Parameters.AddWithValue("@pid", a.PatientId);
        cmd.Parameters.AddWithValue("@did", a.DoctorId);
        cmd.Parameters.AddWithValue("@date", a.AppointmentDate);
        cmd.Parameters.AddWithValue("@reason", (object?)a.Reason ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@status", a.Status.ToString());
        cmd.Parameters.AddWithValue("@notes", (object?)a.Notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@token", a.TokenNumber);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Appointment a)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"UPDATE Appointments SET
            PatientId=@pid, DoctorId=@did, AppointmentDate=@date,
            Reason=@reason, Status=@status, Notes=@notes
            WHERE AppointmentId=@id", conn);
        cmd.Parameters.AddWithValue("@id", a.AppointmentId);
        cmd.Parameters.AddWithValue("@pid", a.PatientId);
        cmd.Parameters.AddWithValue("@did", a.DoctorId);
        cmd.Parameters.AddWithValue("@date", a.AppointmentDate);
        cmd.Parameters.AddWithValue("@reason", (object?)a.Reason ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@status", a.Status.ToString());
        cmd.Parameters.AddWithValue("@notes", (object?)a.Notes ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("DELETE FROM Appointments WHERE AppointmentId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Appointments", conn);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task<Dictionary<string, int>> GetStatusStatsAsync()
    {
        var dict = new Dictionary<string, int>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT Status, COUNT(*) as Cnt FROM Appointments GROUP BY Status", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dict[reader["Status"].ToString()!] = (int)reader["Cnt"];
        return dict;
    }

    public async Task<Dictionary<string, int>> GetMonthlyStatsAsync()
    {
        var dict = new Dictionary<string, int>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(@"
            SELECT FORMAT(AppointmentDate, 'MMM yyyy') as Month,
                   COUNT(*) as Cnt
            FROM Appointments
            WHERE AppointmentDate >= DATEADD(MONTH, -6, GETDATE())
            GROUP BY FORMAT(AppointmentDate, 'MMM yyyy'), YEAR(AppointmentDate), MONTH(AppointmentDate)
            ORDER BY YEAR(AppointmentDate), MONTH(AppointmentDate)", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dict[reader["Month"].ToString()!] = (int)reader["Cnt"];
        return dict;
    }

    public async Task<int> GetTodaysCountAsync()
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)", conn);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    private static Appointment Map(SqlDataReader r) => new()
    {
        AppointmentId   = r["AppointmentId"].ToString()!,
        PatientId       = r["PatientId"].ToString()!,
        DoctorId        = r["DoctorId"].ToString()!,
        PatientName     = r["PatientName"].ToString()!,
        DoctorName      = r["DoctorName"].ToString()!,
        AppointmentDate = (DateTime)r["AppointmentDate"],
        Reason          = r["Reason"] as string ?? "",
        Status          = Enum.Parse<AppointmentStatus>(r["Status"].ToString()!),
        Notes           = r["Notes"] as string ?? "",
        TokenNumber     = r["TokenNumber"].ToString()!
    };
}
