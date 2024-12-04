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

        // Method to upload a document
        public void UploadDocument(int userId, string fileName, string filePath)
        {
            try
            {
                // Ensure the storage directory exists
                if (!Directory.Exists(StoragePath))
                {
                    Directory.CreateDirectory(StoragePath);
                }

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string destinationPath = Path.Combine(StoragePath, uniqueFileName);

                // Copy the file to the storage path
                File.Copy(filePath, destinationPath);

                Console.WriteLine($"File uploaded successfully to {destinationPath}");

                _accessDB.Execute(connection =>
                {
                    string query = "INSERT INTO Document (UserId, DocumentName, FilePath, UploadedAt) " +
                                   "VALUES (?, ?, ?, ?);";  // Using positional parameters to avoid confusion with named parameters

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        // Explicitly set correct types for each parameter using Add method

                        cmd.Parameters.Add("?", OleDbType.Integer).Value = userId; // UserId is integer
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = fileName; // DocumentName is text (string)
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = destinationPath; // FilePath is text (string)
                        cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now; // UploadedAt is Date/Time

                        try
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Document successfully inserted into the database.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to insert document into the database.");
                            }
                        }
                        catch (Exception dbEx)
                        {
                            Console.WriteLine($"Database operation failed: {dbEx.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during document upload: " + ex.Message);
            }
        }



    }
}
