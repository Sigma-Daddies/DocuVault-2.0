using System;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;

namespace DocuVault.Data
{
    public class AccessDB
    {
        private readonly string _connectionString;

        // Constructor to define the connection string, retrieved from App.config
        public AccessDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DocuVaultDB"].ConnectionString;
        }

        // Get the database connection
        private OleDbConnection GetConnection()
        {
            return new OleDbConnection(_connectionString);
        }

        // Executes a method that requires a database connection asynchronously
        public async Task ExecuteAsync(Func<OleDbConnection, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync(); // Open connection asynchronously
                    await action(connection); // Execute the action with the connection
                }
                catch (OleDbException ex)
                {
                    throw new Exception("Database error: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Operation failed: " + ex.Message, ex);
                }
            }
        }

        // Executes a method that returns a value from the database asynchronously
        public async Task<T> ExecuteAsync<T>(Func<OleDbConnection, Task<T>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync(); // Open connection asynchronously
                    return await func(connection); // Execute and return the result
                }
                catch (OleDbException ex)
                {
                    throw new Exception("Database error: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Operation failed: " + ex.Message, ex);
                }
            }
        }

        // Executes a non-query operation (INSERT, UPDATE, DELETE) asynchronously
        public async Task<int> ExecuteNonQueryAsync(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync(); // Open connection asynchronously
                    using (var command = new OleDbCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters); // Add parameters if any
                        }

                        return await command.ExecuteNonQueryAsync(); // Execute and return number of affected rows
                    }
                }
                catch (OleDbException ex)
                {
                    throw new Exception("Database error: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Operation failed: " + ex.Message, ex);
                }
            }
        }

        // Executes a scalar query (e.g., COUNT, MAX) asynchronously
        public async Task<T> ExecuteScalarAsync<T>(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync(); // Open connection asynchronously
                    using (var command = new OleDbCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters); // Add parameters if any
                        }

                        var result = await command.ExecuteScalarAsync(); // Execute and return scalar result
                        return result == DBNull.Value ? default : (T)result;
                    }
                }
                catch (OleDbException ex)
                {
                    throw new Exception("Database error: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Operation failed: " + ex.Message, ex);
                }
            }
        }

        // Executes a query and returns a data reader asynchronously
        public async Task<DbDataReader> ExecuteReaderAsync(string query, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            var connection = GetConnection();
            try
            {
                await connection.OpenAsync(); // Open connection asynchronously
                var command = new OleDbCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters); // Add parameters if any
                }

                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection); // Return reader and close connection when done
            }
            catch (OleDbException ex)
            {
                connection.Dispose(); // Dispose connection if error occurs
                throw new Exception("Database error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                connection.Dispose(); // Dispose connection if error occurs
                throw new Exception("Operation failed: " + ex.Message, ex);
            }
        }
    }
}
