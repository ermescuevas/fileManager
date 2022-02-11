using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ermes.FileManager.Core
{
    public class FileProvider
    {
        #region Utilities

        private void DeleteDirectoryRecursive(string path)
        {
            SetAttributesNormal(path);
            Directory.Delete(path, true);
            const int maxLoopToWait = 10;
            var currentIteration = 0;
            while (Directory.Exists(path))
            {
                currentIteration += 1;
                if (currentIteration > maxLoopToWait)
                    return;
                Thread.Sleep(100);
            }
        }
        private void SetAttributesNormal(string path)
        {
            var dir = new DirectoryInfo(path);
            dir.Attributes = FileAttributes.Normal;
            foreach (var subDir in dir.GetDirectories())
                SetAttributesNormal(subDir.FullName);
            foreach (var file in dir.GetFiles())
                file.Attributes = FileAttributes.Normal;
        }

        #endregion

        #region Methods

        public string Combine(params string[] paths)
        {
            var path = Path.Combine(paths);
            return path;
        }
        public void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }
        public void CreateFileFromPath(string path)
        {
            var fileInfo = new FileInfo(path);
            CreateDirectory(fileInfo.DirectoryName);
            using (File.CreateText(path)) { }
        }
        public void CreateFileFromText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }
        public void DeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            DeleteDirectoryRecursive(path);
        }
        public void DeleteFile(string filePath)
        {
            if (!FileExists(filePath))
                return;

            File.Delete(filePath);
        }
        public void DirectoryCopy(string sourceDir, string destinationDir, bool topDirectoryOnly = true)
        {
            if (!DirectoryExists(sourceDir))
            return;
            var directorySource = new DirectoryInfo(sourceDir);
            var directories = directorySource.GetDirectories();
            CreateDirectory(destinationDir);
            foreach (var file in directorySource.GetFiles())
            {
                string targetFilePath = Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (!topDirectoryOnly)
            {
                foreach (var subDirectory in directories)
                {
                    string newDestDirectory = Combine(destinationDir, subDirectory.Name);
                    DirectoryCopy(subDirectory.FullName, newDestDirectory, false);
                }
            }
        }
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public void DirectoryMove(string sourceDirName, string destDirName)
        {
            if (!DirectoryExists(sourceDirName))
                return;
            if (DirectoryExists(destDirName))
                DeleteDirectory(destDirName);

            Directory.Move(sourceDirName, destDirName);
        }
        public IEnumerable<string> EnumeratedDirectories(string directoryPath)
        {
            try
            {
                return Directory.EnumerateDirectories(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }
        public IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
        {
            try
            {
                return Directory.EnumerateFiles(directoryPath, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
        public void FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            DeleteFile(destFileName);
            File.Copy(sourceFileName, destFileName, overwrite);
        }
        public void FileMove(string sourceFileName, string destFileName)
        {
            DeleteFile(destFileName);
            File.Move(sourceFileName, destFileName);
        }
        public bool IsDirectory(string path)
        {
            return DirectoryExists(path);
        }
        public string ReadFile(string path, Encoding encoding)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var streamReader = new StreamReader(fileStream, encoding))
                {
                    return streamReader.ReadToEnd();
                }

            }
        }
        public string GetParentDirectory(string directoryPath)
        {
            return Directory.GetParent(directoryPath).FullName;
        }
        public string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        #endregion
    }
}
