using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.Generic;
using DocumentManagementSystem.Services;
using DocuVault.Models;

namespace DocuVault
{
    public partial class ManagePage : Page
    {
        private int _userId;
        private string _email;
        private bool _isAdmin;
        private DocumentService _documentService;

        public ManagePage(int userId, string email, bool isAdmin)
        {
            InitializeComponent();
            _userId = userId;
            _email = email;
            _isAdmin = isAdmin;
            _documentService = new DocumentService(); // Initialize DocumentService
            LoadDocuments(); // Load documents on page load
        }

        // Upload document button click event
        private void UploadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Open file dialog to select a file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; // Filter for any type of file
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedFilePath);

                try
                {
                    // Call the UploadDocument method from DocumentService
                    _documentService.UploadDocument(_userId, fileName, selectedFilePath);
                    MessageBox.Show("Document uploaded successfully.");

                    // Reload the document list and update the DataGrid
                    LoadDocuments(); // This will update the DataGrid with the newly uploaded document
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while uploading the document: " + ex.Message);
                }
            }
        }


<<<<<<< Updated upstream
=======
                try
                {
                    // Call the DeleteDocument method from DocumentService
                    _documentService.DeleteDocument(_userId, selectedDocument.DocumentId);
                    MessageBox.Show("Document deleted successfully.");
                    LoadDocuments(); // Reload the document list
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the document: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a document to delete.");
            }
        }

        // Download document button click event
        private void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;

                // Ask the user to choose a download location
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = selectedDocument.DocumentName; // Default filename
                if (saveFileDialog.ShowDialog() == true)
                {
                    string destinationPath = saveFileDialog.FileName;

                    try
                    {
                        // Call the DownloadDocument method from DocumentService
                        _documentService.DownloadDocument(_userId, selectedDocument, Path.GetDirectoryName(destinationPath));
                        MessageBox.Show("Document downloaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while downloading the document: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a document to download.");
            }
        }
>>>>>>> Stashed changes
    }
}
