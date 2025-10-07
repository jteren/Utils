using System.Diagnostics;
using System.Text;

namespace BulkUnzipper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            var files = openFileDialog1.FileNames;
            StringBuilder allFiles = new StringBuilder();

            foreach (string file in files)
            {
                allFiles.Append(file + ";");
            }

            txtPath.Text = allFiles.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // UnZipper.DoJob();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            txtPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {           
            var path = txtPath.Text;

            if (path.IndexOf(";", StringComparison.Ordinal) > -1)
            {
                path = path.Remove(txtPath.Text.LastIndexOf(";", StringComparison.InvariantCultureIgnoreCase), 1);
            }

            if (!string.IsNullOrWhiteSpace(txtPath.Text.Trim()))
            {
                // not found ; so assume folder
                if (path.IndexOf(";", StringComparison.InvariantCultureIgnoreCase) < 0 && path.IndexOf(".", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    UnZipper.DoJob(cbxTopFolder.Checked, lblStatus, directory:path);
                }
                // check single file
                else 
                {
                    var files=openFileDialog1.FileNames;

                    var allFiles = new List<FileInfo>();

                    foreach (var file in files)
                    {
                        allFiles.Add(new FileInfo(file));
                    }

                    UnZipper.DoJob(cbxTopFolder.Checked, lblStatus, allFiles:allFiles.ToArray());
                }
            }
        }
    }

    public static class UnZipper
    {
        public static void DoJob(bool useTopFolder, Label? statusLabel, string? directory = null, FileInfo[]? allFiles = null)
        {
            var files = new FileInfo[] { };
            
            if (statusLabel != null)
            {
                // If called from a background thread, use Invoke
                if (statusLabel.InvokeRequired)
                {
                    statusLabel.Invoke(() => statusLabel.Text = "Processing...");
                }
                else
                {
                    statusLabel.Text = "Processing...";
                }
            }

            if (directory != null)
            {
                var directoryInfo = new DirectoryInfo(directory);
                         
                files = directoryInfo.GetFiles("*.*")
                    .Where(f => f.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase)
                             || f.Extension.Equals(".7z", StringComparison.OrdinalIgnoreCase)
                             || f.Extension.Equals(".rar", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            else
            {
                files = allFiles;
            }

            if (files != null)
            {
                int counter = 0;
                foreach (var file in files)
                {
                    ++counter;
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                    var folderName = file.DirectoryName ?? "";

                    var fullPath = "";

                    if (useTopFolder)
                    {
                        fullPath =
                           Path.Combine(folderName, String.Empty); //  this is the new folder to hold extracted files
                    }
                    else
                    {
                        fullPath =
                           Path.Combine(folderName, fileName); //  this is the new folder to hold extracted files
                    }


                    Directory.CreateDirectory(fullPath);

                    var extension = file.Extension.ToLowerInvariant();
                    //var ext = Path.GetExtension(file.FullName).ToLowerInvariant();

                    if (statusLabel != null)
                    {
                        // If called from a background thread, use Invoke
                        if (statusLabel.InvokeRequired)
                        {
                            statusLabel.Invoke(() => statusLabel.Text = $"Processing... {counter} of {files.Length} ");
                        }
                        else
                        {
                            statusLabel.Invoke(() => statusLabel.Text = $"Processing... {counter} of {files.Length} ");
                        }
                    }


                    switch (extension)
                    {
                        case ".zip":
                            ProcessZip(file.FullName, fullPath);
                            break;
                        case ".7z":
                            Process7Z(file.FullName, fullPath);
                            break;
                        case ".rar":
                            ProcessRar(file.FullName, fullPath);
                            break;
                        default:
                            MessageBox.Show("Unsupported file type: " + extension);
                            break;
                    }
                }
            }
        }

        private static void ProcessZip(string fileFullName, string destinationPath)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(fileFullName, destinationPath);
        }

        private static void Process7Z(string fileFullName, string destinationPath)
        {
            ExtractFile(fileFullName, destinationPath);                       
        }

        private static void ExtractFile(string sourceArchive, string destination)
        {
            string zPath = "7zr.exe"; //add to proj and set CopyToOuputDir
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden, FileName = zPath, Arguments =
                        $"x \"{sourceArchive}\" -y -o\"{destination}\""
                };
                Process? x = Process.Start(pro);
                x?.WaitForExit();
            }
            catch (System.Exception)
            {
                //handle error
            }
        }

        // password for WinRAR is : koko
        private static void ProcessRar(string fileFullName, string destinationPath)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //Put the path of installed winrar.exe
            proc.StartInfo.FileName = @"C:\Program Files\_PORTABLE\WinRAR 7.01 Final x64 Portable\WinRAR.7.01.Port_Downloadly.ir.exe";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.EnableRaisingEvents = true;

            //PWD: Password if the file has any
            //SRC: The path of your rar file. e.g: c:\temp\abc.rar
            //DES: The path you want it to be extracted. e.g: d:\extracted

            //var PWD = ""; // if no password, leave it empty

            //ATTENTION: DESTINATION FOLDER MUST EXIST!

            //proc.StartInfo.Arguments = String.Format("x -p{0} {1} {2}", PWD, SRC, DES);
            proc.StartInfo.Arguments = $"x {fileFullName} {destinationPath}";

            proc.Start();
        }
    }
}
