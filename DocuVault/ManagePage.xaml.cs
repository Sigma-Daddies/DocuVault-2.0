using System;
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

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; // Filter for any type of file
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedFilePath);

                try
                {
                    _documentService.UploadDocument(_userId, fileName, selectedFilePath);
                    MessageBox.Show("Document uploaded successfully.");
                    LoadDocuments();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while uploading the document: " + ex.Message);
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
                    _documentService.DeleteDocument(selectedDocument.DocumentId);
                    MessageBox.Show("Document deleted successfully.");
                    LoadDocuments();
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

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Documents_DataGrid.SelectedItem != null)
            {
                Document selectedDocument = (Document)Documents_DataGrid.SelectedItem;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = selectedDocument.DocumentName;
                if (saveFileDialog.ShowDialog() == true)
                {
                    string destinationPath = saveFileDialog.FileName;

                    try
                    {
                        string destinationDirectory = Path.GetDirectoryName(destinationPath);
                        string destinationFileName = Path.GetFileName(destinationPath);

                        _documentService.DownloadDocument(_userId, selectedDocument, destinationDirectory, destinationFileName);
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
            Documents_DataGrid.ItemsSource = documents;
        }
    }
}
