using System;
using System.Data.OleDb;

namespace DocuVault
{
    public class AccessDB
    {
        private readonly string _connectionString;

        public AccessDB()
        {
            // Initialize the connection string
            _connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Karl\Documents\GitHub\DocuVault-2.0\DocuVault.accdb";
        }

        public OleDbConnection GetConnection()
        {
            return new OleDbConnection(_connectionString);
        }

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
    }
}