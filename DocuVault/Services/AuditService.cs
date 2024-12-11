using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows;
using DocuVault.Data;
using DocuVault.Models;

namespace DocuVault.Services
{
    public class AuditService
    {
        private readonly AccessDB _accessDB;

        public AuditService(AccessDB accessDB)
        {
            _accessDB = accessDB ?? throw new ArgumentNullException(nameof(accessDB));
        }

        // Log audit entry
        public async Task<bool> LogAuditAsync(int userId, string action)
        {
            string query = "INSERT INTO Audit (UserId, [Action], [Timestamp]) VALUES (?, ?, ?)";
            OleDbParameter[] parameters =
            {
                new OleDbParameter("?", OleDbType.Integer) { Value = userId },
                new OleDbParameter("?", OleDbType.VarChar) { Value = action },
                new OleDbParameter("?", OleDbType.Date) { Value = DateTime.Now }
            };
            int rowsAffected = await _accessDB.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        // Get all audit logs with user details for admin view
        public async Task<List<AuditLogWithUserDetails>> GetAuditLogsWithUserDetailsAsync(bool isAdmin)
        {
            string query = @"
        SELECT 
            A.AuditId, 
            A.UserId, 
            A.Action, 
            A.[Timestamp], 
            U.Email 
        FROM 
            Audit A 
        LEFT JOIN 
            Users U ON A.UserId = U.UserId 
        WHERE 
            (@IsAdmin = 1 OR U.IsAdmin = 0)
        ORDER BY 
            A.[Timestamp] DESC";

            var auditLogs = new List<AuditLogWithUserDetails>();

            return await _accessDB.ExecuteAsync(async connection =>
            {
                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var auditLog = new AuditLogWithUserDetails
                            {
                                AuditId = reader.GetInt32(reader.GetOrdinal("AuditId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Action = reader.GetString(reader.GetOrdinal("Action")),
                                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("Email")))
                            {
                                auditLog.Email = reader.GetString(reader.GetOrdinal("Email"));
                            }

                            auditLogs.Add(auditLog);
                        }
                    }
                }
                return auditLogs;
            });
        }


        // Get audit logs for a specific user
        public async Task<List<AuditLog>> GetAuditLogsByUserAsync(int userId)
        {
            string query = "SELECT AuditId, UserId, Action, [Timestamp] FROM Audit WHERE UserId = ? ORDER BY [Timestamp] DESC";
            var auditLogs = new List<AuditLog>();

            return await _accessDB.ExecuteAsync(async connection =>
            {
                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.Add(new OleDbParameter("?", OleDbType.Integer) { Value = userId });
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            auditLogs.Add(new AuditLog
                            {
                                AuditId = reader.GetInt32(reader.GetOrdinal("AuditId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Action = reader.GetString(reader.GetOrdinal("Action")),
                                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                            });
                        }
                    }
                }
                return auditLogs;
            });
        }
    }

    public class AuditLog
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuditLogWithUserDetails : AuditLog
    {
        public string Email { get; set; }
    }
}
