using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DocuVault
{
    public partial class ManagePage : Page
    {
        private List<File> fileList = new List<File>(); // List of files
        private UserService currentUser; // Current logged-in user

        // Constructor to initialize the page with the current user
        public ManagePage(UserService user)
        {
            InitializeComponent();
            currentUser = user;

            // No need to check for admin status, assuming the UI stays the same for all users

            // Load files for the user
            LoadFileList();
        }

        // Load a list of files associated with the current user (this can be from a database or other sources)
        private void LoadFileList()
        {
            // Example: Adding files to the list with an index
            fileList.Add(new File { Index = 1, FileName = "Document1.pdf", UploadDate = DateTime.Now });
            fileList.Add(new File { Index = 2, FileName = "Image2.png", UploadDate = DateTime.Now.AddMinutes(-30) });
            fileList.Add(new File { Index = 3, FileName = "Report3.docx", UploadDate = DateTime.Now.AddHours(-2) });

            // Set the data source of the ListView
            FileListView.ItemsSource = fileList;
        }

        // Event handler for uploading a file
        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle file upload logic here (e.g., open a file dialog and upload selected files)
            MessageBox.Show("File upload functionality is yet to be implemented.");
        }
    }

    // File class to represent files in the list
    public class File
    {
        public int Index { get; set; }  // Index for numbering the files
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
