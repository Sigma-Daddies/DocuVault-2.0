﻿using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.Generic;
using DocuVault.Services;
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

            // Pass the storage path to the DocumentService constructor
            _documentService = new DocumentService(@"C:\Users\punza\Desktop\newdocu\DocuVault-2.0-main\Documents");
            LoadDocuments(); // Load documents on page load
        }

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

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;
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

        private void LoadDocuments()
        {
            List<Document> documents = _documentService.GetDocumentsByUser(_userId);
            Documents_DataGrid.ItemsSource = documents; // Set the ItemsSource of the DataGrid
            Documents_DataGrid.Items.Refresh(); // Refresh DataGrid after loading
        }

        private void Documents_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the event when a document is selected in the DataGrid (optional)
        }
    }
}
