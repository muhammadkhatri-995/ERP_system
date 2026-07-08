using ERP_sys.Models;
using ERP_sys.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP_sys.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly string _connectionString;

        public AuditLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task InsertAsync(AuditLogs log)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                INSERT INTO AuditLogs
                    (UserId, UserName, UserRole, IpAddress, Action, HttpMethod, Path, StatusCode, UserAgent, Timestamp)
                VALUES
                    (@UserId, @UserName, @UserRole, @IpAddress, @Action, @HttpMethod, @Path, @StatusCode, @UserAgent, @Timestamp)",
                connection);

            cmd.Parameters.AddWithValue("@UserId", (object?)log.UserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserName", (object?)log.UserName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserRole", (object?)log.UserRole ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IpAddress", (object?)log.IpAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Action", (object?)log.Action ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HttpMethod", log.HttpMethod);
            cmd.Parameters.AddWithValue("@Path", log.Path);
            cmd.Parameters.AddWithValue("@StatusCode", (object?)log.StatusCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserAgent", (object?)log.UserAgent ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Timestamp", log.Timestamp);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<AuditLogs>> GetLogsAsync(AuditLogFilterDto filter)
        {
            var logs = new List<AuditLogs>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT TOP 500 *
                FROM AuditLogs
                WHERE (@IpAddress IS NULL OR @IpAddress = '' OR IpAddress LIKE '%' + @IpAddress + '%')
                    AND (@UserName IS NULL OR @UserName = '' OR UserName LIKE '%' + @UserName + '%')
                    AND (@FromDate IS NULL OR Timestamp >= @FromDate)
                    AND (@ToDate IS NULL OR Timestamp <= @ToDate)
                ORDER BY Timestamp DESC",
                connection);

            cmd.Parameters.AddWithValue("@IpAddress", (object?)filter.IpAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserName", (object?)filter.UserName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", (object?)filter.FromDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)filter.ToDate ?? DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                logs.Add(new AuditLogs
                {
                    Id = (int)reader["Id"],
                    UserId = reader["UserId"] as int?,
                    UserName = reader["UserName"] as string,
                    UserRole = reader["UserRole"] as string,
                    IpAddress = reader["IpAddress"] as string,
                    Action = reader["Action"] as string,
                    HttpMethod = reader["HttpMethod"].ToString()!,
                    Path = reader["Path"].ToString()!,
                    StatusCode = reader["StatusCode"] as int?,
                    UserAgent = reader["UserAgent"] as string,
                    Timestamp = (DateTime)reader["Timestamp"]
                });
            }

            return logs;
        }
    }
}
