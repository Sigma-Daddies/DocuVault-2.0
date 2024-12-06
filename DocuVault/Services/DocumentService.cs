using System;
using System.IO;
using System.Collections.Generic;
using System.Data.OleDb;
using DocuVault.Models;
using DocuVault.Data;

namespace DocuVault.Services
{
    public class DocumentService
    {
        private readonly string _storagePath;
        private readonly AccessDB _accessDB;

        public DocumentService(string storagePath)
        {
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            _accessDB = new AccessDB();
        }

        // Retrieve documents uploaded by a specific user
        public List<Document> GetDocumentsByUser(int userId)
        {
            List<Document> documents = new List<Document>();

            _accessDB.ExecuteAsync(async connection =>
            {
                string query = "SELECT * FROM Document WHERE UserId = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = userId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
            }).Wait();

            return documents;
        }

        // Upload a document
        public void UploadDocument(int userId, string fileName, string filePath)
        {
            try
            {
                if (!Directory.Exists(_storagePath))
                {
                    Directory.CreateDirectory(_storagePath);
                }

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string destinationPath = Path.Combine(_storagePath, uniqueFileName);
                File.Copy(filePath, destinationPath);

                Console.WriteLine($"File uploaded successfully to {destinationPath}");

                _accessDB.ExecuteAsync(async connection =>
                {
                    string query = "INSERT INTO Document (UserId, DocumentName, FilePath, UploadedAt) VALUES (?, ?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = userId;
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = fileName;
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = destinationPath;
                        cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Document successfully inserted into the database.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to insert document into the database.");
                        }
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during document upload: {ex.Message}");
            }
        }

        // Download a document (updated to support four parameters)
        public void DownloadDocument(int userId, Document document, string destinationDirectory, string destinationFileName)
        {
            try
            {
                // Ensure the document's file path exists
                if (File.Exists(document.FilePath))
                {
                    // Construct the full destination path
                    string destinationPath = Path.Combine(destinationDirectory, destinationFileName);

                    // Ensure the destination directory exists
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Copy the file to the destination path
                    File.Copy(document.FilePath, destinationPath, overwrite: true);
                    Console.WriteLine($"Document downloaded to {destinationPath}");
                }
                else
                {
                    throw new FileNotFoundException($"Document not found at {document.FilePath}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during document download: {ex.Message}");
                throw;
            }
        }

        // Delete a document
        public void DeleteDocument(int documentId)
        {
            try
            {
                _accessDB.ExecuteAsync(async connection =>
                {
                    string query = "DELETE FROM Document WHERE DocumentId = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = documentId;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Document successfully deleted.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to delete document.");
                        }
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during document deletion: {ex.Message}");
            }
        }
    }
}
