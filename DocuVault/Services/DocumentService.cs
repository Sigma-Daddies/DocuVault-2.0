using System;
using System.IO;
using System.Collections.Generic;
using System.Data.OleDb;
using DocuVault.Models;
using DocuVault.Data;
using DocuVault.Services; // Add this to use AuditService
using System.Windows.Controls;
using System.Windows; // For MessageBox

namespace DocuVault.Services
{
    public class DocumentService
    {
        private readonly string _storagePath;
        private readonly AccessDB _accessDB;
        private readonly AuditService _auditService;

        public DocumentService(string storagePath)
        {
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            _accessDB = new AccessDB();
            _auditService = new AuditService(_accessDB); // Initialize the AuditService
        }

        // Retrieve documents uploaded by a specific user
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

        // Upload a document
        public async void UploadDocument(int userId, string fileName, string filePath)
        {
            try
            {
                // Ensure the storage directory exists
                if (!Directory.Exists(_storagePath))
                {
                    Directory.CreateDirectory(_storagePath);
                    MessageBox.Show("Storage path created: " + _storagePath);  // Debugging line
                }

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string destinationPath = Path.Combine(_storagePath, uniqueFileName);

                // Copy the file to the storage path
                File.Copy(filePath, destinationPath);
                MessageBox.Show("File uploaded successfully to: " + destinationPath); // Debugging line

                _accessDB.Execute(connection =>
                {
                    string query = "INSERT INTO Document (UserId, DocumentName, FilePath, UploadedAt) " +
                                   "VALUES (?, ?, ?, ?);";

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = userId;
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = fileName;
                        cmd.Parameters.Add("?", OleDbType.VarChar).Value = destinationPath;
                        cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                        try
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Document successfully inserted into the database."); // Debugging line
                            }
                            else
                            {
                                MessageBox.Show("Failed to insert document into the database."); // Debugging line
                            }
                        }
                        catch (Exception dbEx)
                        {
                            MessageBox.Show($"Database operation failed: {dbEx.Message}");  // Debugging line
                        }
                    }
                });

                // Log the audit entry for document upload
                await _auditService.LogAuditAsync(userId, $"User uploaded file: {fileName}");

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during document upload: " + ex.Message);
            }
        }

        // Download a document
        public void DownloadDocument(int userId, Document document, string destinationPath)
        {
            try
            {
                if (File.Exists(document.FilePath))
                {
                    string destinationFilePath = Path.Combine(destinationPath, document.DocumentName);

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    File.Copy(document.FilePath, destinationFilePath, overwrite: true);
                }
                else
                {
                    throw new FileNotFoundException($"Document not found at {document.FilePath}.");
                }

                // Log the audit entry for document download
                _auditService.LogAuditAsync(userId, $"User downloaded file: {document.DocumentName}").Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during document download: " + ex.Message);
                throw;
            }
        }

        // Delete a document
        public void DeleteDocument(int userId, int documentId)
        {
            string filePath = string.Empty;
            string documentName = string.Empty;

            try
            {
                // Retrieve document details (document name and file path) from the database
                _accessDB.Execute(connection =>
                {
                    string query = "SELECT DocumentName, FilePath FROM Document WHERE DocumentId = @DocumentId";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DocumentId", documentId);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                documentName = reader["DocumentName"].ToString();
                                filePath = reader["FilePath"].ToString();
                            }
                            else
                            {
                                Console.WriteLine("Document not found.");
                                return;
                            }
                        }
                    }
                });

                // Delete the file from storage
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show("File deleted successfully.");
                }
                else
                {
                    MessageBox.Show($"File not found at {filePath}.");
                }

                // Delete the record from the database
                _accessDB.Execute(connection =>
                {
                    string query = "DELETE FROM Document WHERE DocumentId = @DocumentId";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DocumentId", documentId);
                        cmd.ExecuteNonQuery();
                    }
                });

                // Log the audit entry for document deletion
                _auditService.LogAuditAsync(userId, $"User deleted file: {documentName}").Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during document deletion: " + ex.Message);
            }
        }
    }
}
