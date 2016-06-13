using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SevenZip;

namespace CleanStorageFiles
{
    public partial class Main : Form
    {
        //List<string> rarExtensions= new List<string>();

        public Main()
        {
            SevenZipCompressor.SetLibraryPath(Path.GetDirectoryName(Application.ExecutablePath) + "\\7z.dll");
            InitializeComponent();
            //rarExtensions.Add(".rar");
            //for (int i = 0; i <= 99; i++)
            //{
            //    rarExtensions.Add(".r" + i.ToString("00"));
            //}
        }

        private int DeletedFileCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if(AllFolders.Checked){
                CleanAllFolders();
            } else {
                CleanFolder(directory.Text);
            }
        }

        private void CleanAllFolders()
        {
            List<String> folders = new List<String>();
            folders.Add(@"C:\Storage");
            folders.Add(@"D:\Storage");
            folders.Add(@"E:\Storage");
            folders.Add(@"F:\Storage");
            folders.Add(@"G:\Storage");
            folders.Add(@"k:\Storage");

            foreach (String folder in folders)
            {
                CleanFolder(folder);
            }
        }

        private void CleanFolder(String folder)
        {
            //Start checking all folders for .rar files
            long bytes = 0;

            bytes = ParseDirectoryForRARs(folder, Recurse.Checked);

            if (DontDelete.Checked)
            {
                LogBox.Text += "Data that can be deleted: " + (bytes / 1024) / 1024 + "mb\n";
            }
            else
            {
                LogBox.Text += "Data deleted: " + (bytes / 1024) / 1024 + "mb\n";
            }
        }



        private long ParseDirectoryForRARs(string dir, bool recurse)
        {
            if (String.IsNullOrEmpty(dir))
            {
                return 0;
            }
            //Recurse all directories
            long DataDeleted = 0;
            try
            {
                if (recurse)
                {
                    string[] directories = Directory.GetDirectories(dir);
                    foreach (string childdir in directories)
                    {
                        //Visit all subfolders if we have checked that option
                        DataDeleted += ParseDirectoryForRARs(childdir, true);
                    }
                 }

                //Find the filename via the first .rar file

                DirectoryInfo di = new DirectoryInfo(dir);
                FileInfo[] rarFiles = di.GetFiles("*.rar");
                FileInfo[] zipFiles = di.GetFiles("*.zip");
                List<FileInfo> archiveFiles = new List<FileInfo>();
                archiveFiles.AddRange(rarFiles);
                archiveFiles.AddRange(zipFiles);

                foreach (FileInfo fi in archiveFiles)
                    DataDeleted += DeleteFiles(fi);

            } catch (Exception ex){
                ErrorLogBox.Text += "Couldn't open dir: " + dir + " due to: " + ex.Message;
            }

            return DataDeleted;

        }

        //deletes extracted movie files with the same file name as fi
        private long DeleteFiles(FileInfo fi)
        {
            Application.DoEvents();

            long dataDeleted = 0;
            try
            {               
                SevenZipExtractor archive = new SevenZipExtractor(fi.FullName);
                int FailedFileCount = 0;
                foreach (SevenZip.ArchiveFileInfo archiveFileData in archive.ArchiveFileData)
                {
                    if (FailedFileCount > 10)
                    {
                        return dataDeleted;
                    }
                    String fileName = fi.DirectoryName + "\\" + archiveFileData.FileName;
                    if (fileName.Contains("nfo") || fileName.Contains("diz"))
                        continue;
                    if (File.Exists(fileName))
                    {
                        FailedFileCount = 0;
                        try
                        {
                            //Get the file size and attempt to delete the file
                            long length = new FileInfo(fileName).Length;
                            if (DontDelete.Checked)
                            {
                                LogBox.Text += "File found: \n" + fileName + "\n";
                            }
                            else
                            {
                                File.Delete(fileName);
                                //Only do this if we actually deleted the file
                                LogBox.Text += "Deleted file: \n" + fileName + "\n";
                            }
                            dataDeleted += length;
                            DeletedFileCount++;
                            DeletedFileCountBox.Text = DeletedFileCount.ToString();

                        }
                        catch (Exception e)
                        {
                            LogBox.Text += "Failed to delete: " + fileName + " due to: " + e.Message + "\n";
                        }
                    }
                    else
                    {
                        FailedFileCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogBox.Text += "Failed to open archive: " + fi.FullName + " due to error: " + ex.Message + "\n";
            }

            LogBox.SelectionStart = LogBox.Text.Length;
            LogBox.ScrollToCaret();
            return dataDeleted;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {   
            //Grab the folder the user wants to clean
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                directory.Text = folderBrowserDialog.SelectedPath;
            }

        }
    }
}
