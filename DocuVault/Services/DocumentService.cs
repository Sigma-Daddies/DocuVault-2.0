using System;
using System.IO;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Linq;
using DocuVault.Models;
using DocuVault;

namespace DocumentManagementSystem.Services
{
    public class DocumentService
    {
        private const string StoragePath = @"C:\Documents\Data\";
        private readonly AccessDB _accessDB;

        public DocumentService()
        {
            _accessDB = new AccessDB();
        }

        // Method to retrieve documents uploaded by a specific user
        public List<Document> GetDocumentsByUser(int userId)
        {
            List<Document> documents = new List<Document>();

            _accessDB.Execute(connection =>
            {
                string query = "SELECT * FROM Document WHERE UserId = @UserId";
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            documents.Add(new Document
                            {
                                DocumentId = (int)reader["DocumentId"],
                                UserId = (int)reader["UserId"],
                                DocumentName = reader["DocumentName"].ToString(),
                                FilePath = reader["FilePath"].ToString(),
                                UploadedAt = (DateTime)reader["UploadedAt"]
                            });
                        }
                    }
                }
            });

            return documents;
        }
    }
}
