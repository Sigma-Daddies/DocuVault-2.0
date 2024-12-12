using System;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using DocuVault.Models;

namespace DocuVault.Data
{
    public class AccessDB
    {
        private readonly string _connectionString;

        // Constructor that reads the connection string from App.config
        public AccessDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DocuVaultDB"]?.ConnectionString
                                ?? throw new Exception("Connection string 'DocuVaultDB' not found in configuration.");
        }

        // Get the database connection
        private OleDbConnection GetConnection()
        {
            return new OleDbConnection(_connectionString);
        }

        // Rest of the methods remain unchanged
        public void Execute(Action<OleDbConnection> action)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    action(connection);
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public async Task ExecuteAsync(Func<OleDbConnection, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    await action(connection);
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public T Execute<T>(Func<OleDbConnection, T> func)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    return func(connection);
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<OleDbConnection, Task<T>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    return await func(connection);
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new OleDbCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new OleDbCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        var result = await command.ExecuteScalarAsync();
                        return result == DBNull.Value ? default : (T)result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Database operation failed: " + ex.Message, ex);
                }
            }
        }

        public async Task<DbDataReader> ExecuteReaderAsync(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            var connection = GetConnection();
            try
            {
                await connection.OpenAsync();
                var command = new OleDbCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                connection.Dispose();
                throw new Exception("Database operation failed: " + ex.Message, ex);
            }
        }

        // The rest of the methods (InsertAuditLogAsync, GetAuditLogsAsync, etc.) remain unchanged
    }
}