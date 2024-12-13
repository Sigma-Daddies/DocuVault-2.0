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
using DocuVault.Utils;

namespace DocuVault
{
    public partial class ManagePage : Page
    {
        private int _userId;
        private string _email;
        private bool _isAdmin;
        private readonly ToastNotifier _toastNotifier;
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

            _toastNotifier = new ToastNotifier(ToasterPanel);
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
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

                try
                {
                    // Call the synchronous UploadDocument method from DocumentService
                    _documentService.UploadDocument(_userId, fileName, selectedFilePath);
                    await _toastNotifier.ShowToastConfirm("Document uploaded successfully.");

                    // Reload the document list
                    LoadDocuments();
                    Documents_DataGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    await _toastNotifier.ShowToastWarning($"An error occurred while uploading the document: {ex.Message}");
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;
                try
                {
                    // Call the DeleteDocument method from DocumentService
                    _documentService.DeleteDocument(_userId, selectedDocument.DocumentId);
                    await _toastNotifier.ShowToastConfirm("Document deleted successfully.");

                    // Reload the document list
                    LoadDocuments();
                }
                catch (Exception ex)
                {
                    await _toastNotifier.ShowToastWarning($"An error occurred while deleting the document: {ex.Message}");
                }
            }
            else
            {
                await _toastNotifier.ShowToastWarning("Please select a document to delete.");
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
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
                        await _toastNotifier.ShowToastConfirm("Document downloaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        await _toastNotifier.ShowToastWarning($"An error occurred while downloading the document: {ex.Message}");
                    }
                }
            }
            else
            {
                await _toastNotifier.ShowToastWarning("Please select a document to download.");
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
