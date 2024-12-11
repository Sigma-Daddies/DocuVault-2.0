using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DocuVault.Services;
using DocuVault.Models;
using DocuVault.Data;
using System.Configuration; // For reading from App.config

namespace DocuVault
{
    public partial class ManagePage : Page
    {
        private int _userId;
        private string _email;
        private bool _isAdmin;
        private DocumentService _documentService;
        private AuditService _auditService;
        private AccessDB _accessDB;

        private ObservableCollection<Document> _documents;

        public ManagePage(int userId, string email, bool isAdmin)
        {
            InitializeComponent();
            _userId = userId;
            _email = email;
            _isAdmin = isAdmin;

            _accessDB = new AccessDB();
            _auditService = new AuditService(_accessDB);

            // Initialize DocumentService with storage path
            string storagePath = ConfigurationManager.AppSettings["StoragePath"] ?? @"C:\Documents\Data\";
            _documentService = new DocumentService(storagePath);

            LoadDocuments(); // Load documents for the user
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select a file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*" // Filter for any type of file
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedFilePath);

                MessageBox.Show("Selected file: " + selectedFilePath);  // Debugging line

                try
                {
                    // Call the synchronous UploadDocument method from DocumentService
                    _documentService.UploadDocument(_userId, fileName, selectedFilePath);
                    MessageBox.Show("Document uploaded successfully.");

                    // Reload the document list
                    LoadDocuments();
                    Documents_DataGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while uploading the document: {ex.Message}");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;
                try
                {
                    // Call the DeleteDocument method from DocumentService
                    _documentService.DeleteDocument(_userId, selectedDocument.DocumentId);
                    MessageBox.Show("Document deleted successfully.");

                    // Reload the document list
                    LoadDocuments();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the document: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a document to delete.");
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;

                // Ask the user to choose a download location
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = selectedDocument.DocumentName // Default filename
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    string destinationPath = saveFileDialog.FileName;

                    try
                    {
                        // Call the synchronous DownloadDocument method from DocumentService
                        _documentService.DownloadDocument(_userId, selectedDocument, Path.GetDirectoryName(destinationPath));
                        MessageBox.Show("Document downloaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while downloading the document: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a document to download.");
            }
        }

        // Load the documents into the DataGrid
        private void LoadDocuments()
        {
            try
            {
                _documents = new ObservableCollection<Document>(_documentService.GetDocumentsByUser(_userId));
                Documents_DataGrid.ItemsSource = _documents;
                Documents_DataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading documents: {ex.Message}");
            }
        }

        private void Documents_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the event when a document is selected in the DataGrid (optional)
        }
    }
}
