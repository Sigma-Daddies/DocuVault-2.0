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


    }
}
