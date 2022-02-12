using Ermes.FileManager.Core;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ermes.FileManager.UI
{
    public partial class MainForm : Form
    {
        FileProvider _fileProvider;
        public MainForm()
        {
            InitializeComponent();
            _fileProvider = new FileProvider();
        }

        #region Events

        private void btnOpenDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = folderBrowserDialog1.SelectedPath;
                LoadFilesAndDirectories();
            }
        }
        private void btnCreateDirectory_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDirectory.Text) && !string.IsNullOrEmpty(txtName.Text))
            {
                _fileProvider.CreateDirectory(_fileProvider.Combine(txtDirectory.Text, txtName.Text));
                ClearFields();
                LoadFilesAndDirectories();
            }
            else
                HandledMessageShown(3);
        }
        private void btnCreateFile_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtPlainText.Text) && !string.IsNullOrEmpty(txtDirectory.Text) && !string.IsNullOrEmpty(txtName.Text))
            {
                CreateFile();
                ClearFields();
                LoadFilesAndDirectories();
            }
            else
                HandledMessageShown(2);

        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lsvFileManager.SelectedItems != null && lsvFileManager.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this record ?", "File Manager", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (_fileProvider.IsDirectory(lsvFileManager.SelectedItems[0].Name))
                        _fileProvider.DeleteDirectory(lsvFileManager.SelectedItems[0].Name);
                    else
                        _fileProvider.DeleteFile(lsvFileManager.SelectedItems[0].Name);

                    LoadFilesAndDirectories();
                }
            }
            else
                HandledMessageShown(1);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDirectory.Text))
            {
                var parentDirectory = _fileProvider.GetParentDirectory(txtDirectory.Text);
                if (!string.IsNullOrEmpty(parentDirectory) && _fileProvider.DirectoryExists(parentDirectory))
                {
                    txtDirectory.Text = parentDirectory;
                    LoadFilesAndDirectories();
                }
            }
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (lsvFileManager.SelectedItems != null && lsvFileManager.SelectedItems.Count > 0)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtDirectory.Text = folderBrowserDialog1.SelectedPath;
                    var destFileName = GetDestinationFile(txtDirectory.Text, lsvFileManager.SelectedItems[0].Name);

                    if (_fileProvider.IsDirectory(lsvFileManager.SelectedItems[0].Name))
                        _fileProvider.DirectoryCopy(lsvFileManager.SelectedItems[0].Name, destFileName, false);
                    else
                        _fileProvider.FileCopy(lsvFileManager.SelectedItems[0].Name, destFileName);
                    LoadFilesAndDirectories();
                }
            }
            else
                HandledMessageShown(1);
        }
        private void btnMove_Click(object sender, EventArgs e)
        {
            if (lsvFileManager.SelectedItems != null && lsvFileManager.SelectedItems.Count > 0)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtDirectory.Text = folderBrowserDialog1.SelectedPath;
                    var destFileName = GetDestinationFile(txtDirectory.Text, lsvFileManager.SelectedItems[0].Name);

                    if (_fileProvider.IsDirectory(lsvFileManager.SelectedItems[0].Name))
                        _fileProvider.DirectoryMove(lsvFileManager.SelectedItems[0].Name, destFileName);
                    else
                        _fileProvider.FileMove(lsvFileManager.SelectedItems[0].Name, destFileName);

                    LoadFilesAndDirectories();
                }
            }
            else
                HandledMessageShown(1);
        }
        private void lsvFileManager_DoubleClick(object sender, EventArgs e)
        {
            if (lsvFileManager.SelectedItems != null && lsvFileManager.SelectedItems.Count > 0)
            {
                ClearFields();
                if (_fileProvider.IsDirectory(lsvFileManager.SelectedItems[0].Name))
                {
                    txtDirectory.Text = lsvFileManager.SelectedItems[0].Name;
                    LoadFilesAndDirectories();
                }
                else if (_fileProvider.GetFileExtension(lsvFileManager.SelectedItems[0].Name) == ".txt")
                {
                    txtName.Text = _fileProvider.GetFileName(lsvFileManager.SelectedItems[0].Name);
                    txtPlainText.Text = _fileProvider.ReadFile(lsvFileManager.SelectedItems[0].Name, Encoding.UTF8);
                }
            }
            else
                HandledMessageShown(1);
        }
        private void btnCreateBreadcrumbs_Click(object sender, EventArgs e)
        {
            var breadCrumbForm = new BreadcrumbGeneratorForm();
            breadCrumbForm.ShowDialog();
        }

        #endregion

        #region Methods

        private void LoadFilesAndDirectories()
        {
            if (!string.IsNullOrEmpty(txtDirectory.Text))
            {
                lsvFileManager.Items.Clear();
                ClearFields();
                if (_fileProvider.DirectoryExists(txtDirectory.Text))
                {
                    foreach (var directory in _fileProvider.EnumeratedDirectories(txtDirectory.Text))
                        lsvFileManager.Items.Add(directory, _fileProvider.GetFileName(directory), 0);
                    foreach (var file in _fileProvider.EnumerateFiles(txtDirectory.Text, "*.*"))
                        lsvFileManager.Items.Add(file, _fileProvider.GetFileName(file), 1);
                }
            }
        }
        private void ClearFields()
        {
            txtName.Text = "";
            txtPlainText.Text = "";
        }
        private void CreateFile()
        {
            var fileExtension = _fileProvider.GetFileExtension(txtName.Text);
            if (string.IsNullOrEmpty(fileExtension))
                txtName.Text += ".txt";
            _fileProvider.CreateFileFromText(_fileProvider.Combine(txtDirectory.Text, txtName.Text), txtPlainText.Text, Encoding.UTF8);
        }
        private string GetDestinationFile(string directoryPath, string filePath)
        {
            string fileName = _fileProvider.GetFileName(filePath);
            string destFileName = _fileProvider.Combine(directoryPath, fileName);

            return destFileName;
        }
        private void HandledMessageShown(int messageId)
        {
            string message;
            switch (messageId)
            {
                case 1:
                    message = "You must select a file or directory to continue";
                    break;
                case 2:
                    message = "You must enter a name, content and directory to continue";
                    break;
                case 3:
                    message = "You must enter a name and directory to continue";
                    break;
                default:
                    message = "Message not handled";
                    break;
            }

            MessageBox.Show(message);
        }

        #endregion
    }
}