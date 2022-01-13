using Microsoft.Win32;
using NReco.VideoInfo;
using RandomFile.Models;
using RandomFile.Models.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Application = RandomFile.Models.Application;

namespace RandomFile.ViewModels
{
    public class MainWindowViewModel : ViewModelNotifier
    {
        private RandomFileManager _randomFileManager;
        private ApplicationManager _applicationManager;

        public MainWindowViewModel()
        {
            _randomFileManager = new RandomFileManager();
            _randomFileManager.StartSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = true;
            };
            _randomFileManager.FinishSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = false;
                Files = _randomFileManager.FileCount + " fichiers";
            };

            _applicationManager = new ApplicationManager();

            // Default option
            SearchFolder = AppDomain.CurrentDomain.BaseDirectory; // Dossier d'ouverture de l'applicaiton
            SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            AutoLaunchOption = true;
            SearchSubfolderOption = true;
            PrevButtonEnable = false;
            EnableProgressBar = false;
            Files = "0 fichiers";

            InitCommands();
        }

        #region Properties
        public string SearchFolder
        {
            get
            {
                return _randomFileManager.SelectedFolder;
            }

            set
            {
                _randomFileManager.SelectedFolder = value;
                OnPropertyChanged("SearchFolder");
            }
        }

        private FileInfo _selectedFile;
        public FileInfo SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public ComboBoxItem SelectedFileType
        {
            get
            {
                switch (_randomFileManager.SelectedType)
                {
                    case FileType.None:
                        return new ComboBoxItem() { Name = "all" };

                    case FileType.Movie:
                        return new ComboBoxItem() { Name = "movies" };

                    case FileType.Picture:
                        return new ComboBoxItem() { Name = "pictures" };

                    case FileType.Song:
                        return new ComboBoxItem() { Name = "songs" };

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                switch (value.Name)
                {
                    case "all":
                        _randomFileManager.SelectedType = FileType.None;
                        ApplciationList = _applicationManager.DefaultApplicationList;
                        break;

                    case "movies":
                        _randomFileManager.SelectedType = FileType.Movie;
                        ApplciationList = _applicationManager.MovieApplicationList;
                        break;

                    case "pictures":
                        _randomFileManager.SelectedType = FileType.Picture;
                        ApplciationList = _applicationManager.PictureApplicationList;
                        break;

                    case "songs":
                        _randomFileManager.SelectedType = FileType.Song;
                        ApplciationList = _applicationManager.MusicApplicationList;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                SelectedApplication = ApplciationList[0];

                OnPropertyChanged("SelectedFileType");
            }
        }

        private List<Application> applicationList;
        public List<Application> ApplciationList
        {
            get
            {
                return applicationList;
            }

            set
            {
                applicationList = value;
                OnPropertyChanged("ApplciationList");
            }
        }

        private Application selectedApplication;
        public Application SelectedApplication
        {
            get
            {
                return selectedApplication;
            }

            set
            {
                selectedApplication = value;
                OnPropertyChanged("SelectedApplication");
            }
        }

        private bool launchButtonEnable;
        public bool LaunchButtonEnable
        {
            get
            {
                return launchButtonEnable;
            }

            set
            {
                launchButtonEnable = value;
                OnPropertyChanged("LaunchButtonEnable");
            }
        }

        private bool prevButtonEnable;
        public bool PrevButtonEnable
        {
            get
            {
                return prevButtonEnable;
            }

            set
            {
                prevButtonEnable = value;
                OnPropertyChanged("PrevButtonEnable");
            }
        }

        private bool autoLaunchOption;
        public bool AutoLaunchOption
        {
            get
            {
                return autoLaunchOption;
            }

            set
            {
                autoLaunchOption = value;
                OnPropertyChanged("AutoLaunchOption");
            }
        }

        public bool SearchSubfolderOption
        {
            get
            {
                return _randomFileManager.UseSubFolders;
            }

            set
            {
                _randomFileManager.UseSubFolders = value;
                OnPropertyChanged("SearchSubfolderOption");
            }
        }

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }

            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private bool enableProgressBar;
        public bool EnableProgressBar
        {
            get
            {
                return enableProgressBar;
            }

            set
            {
                enableProgressBar = value;
                OnPropertyChanged("EnableProgressBar");
            }
        }

        public string files;
        public string Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }
        #endregion

        #region Commands
        public ICommand SelectFolderCommand { get; private set; }
        public ICommand RandomCommand { get; private set; }
        public ICommand LaunchCommand { get; private set; }
        public ICommand OpenFolderCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }

        private void InitCommands()
        {
            SelectFolderCommand = new RelayCommand(x => { SelectFolder(); });
            RandomCommand = new RelayCommand(x => { Random(); });
            LaunchCommand = new RelayCommand(x => { Launch(); });
            OpenFolderCommand = new RelayCommand(x => { OpenFolder(); });
            DetailsCommand = new RelayCommand(x => { Details(); });
            RenameCommand = new RelayCommand(x => { Rename(); });
            DeleteCommand = new RelayCommand(x => { Delete(); });
            PreviousCommand = new RelayCommand(x => { Previous(); });
        }
        #endregion

        #region Button methods
        // When the user click on the button "Select Folder".
        public void SelectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = SearchFolder;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SearchFolder = fbd.SelectedPath;
                OnPropertyChanged("SelectedFile");
            }
        }

        // Find a new file from a random function, and try to lunch if it's asked.
        public void Random()
        {
            if (!string.IsNullOrEmpty(SearchFolder))
            {
                if (_randomFileManager.FileCount > 0)
                {
                    SelectedFile = _randomFileManager.NextFile();

                    LaunchButtonEnable = true;
                    PrevButtonEnable = true;

                    if (AutoLaunchOption)
                        Launch();
                }
                else
                {
                    MessageBox.Show("Aucun fichier correspondant aux critères n'a été trouvé.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            }
        }

        // Lunch a file if there is one.
        public void Launch()
        {
            if (!string.IsNullOrEmpty(SearchFolder) && SelectedFile != null)
            {
                try
                {
                    if(SelectedApplication != null && !string.IsNullOrEmpty(SelectedApplication.Executable))
                    {
                        string progamPath = RegistryTools.GetPathForExe(SelectedApplication.Executable);
                        Process.Start(progamPath, "\"" + SelectedFile.FullName + "\"");
                    }
                    else
                    {
                        Process.Start(SelectedFile.FullName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Une erreur est survenue lors du lancement du fichier : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Open the folder of the current file.
        public void OpenFolder()
        {
            if (!string.IsNullOrEmpty(SearchFolder) && SelectedFile != null)
            {
                try
                {
                    Process.Start(SelectedFile.DirectoryName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Une erreur est survenue lors du l'ouverture du dossier : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Open a window with file details0
        public void Details()
        {
            if (!string.IsNullOrEmpty(SearchFolder) && SelectedFile != null)
            {
                FFProbe ffProbe = new FFProbe();
                MediaInfo videoInfo = ffProbe.GetMediaInfo(SelectedFile.FullName);

                string message = "";
                message += string.Format("Name: {0}", SelectedFile.Name) + "\n";
                message += string.Format("File format: {0}", videoInfo.FormatName) + "\n";
                message += string.Format("Duration: {0}", videoInfo.Duration) + "\n\n";

                foreach (var tag in videoInfo.FormatTags)
                    message += string.Format("\t{0}: {1}", tag.Key, tag.Value) + "\n";

                foreach (var stream in videoInfo.Streams)
                {
                    message += string.Format("Stream {0} ({1})", stream.CodecName, stream.CodecType) + "\n";

                    if (stream.CodecType == "video")
                    {
                        message += string.Format("\tFrame size: {0}x{1}", stream.Width, stream.Height) + "\n";
                        message += string.Format("\tFrame rate: {0:0.##}", stream.FrameRate) + "\n";
                    }

                    foreach (var tag in stream.Tags)
                        message += string.Format("\t{0}: {1}", tag.Key, tag.Value) + "\n";

                    message += "\n";
                }

                MessageBox.Show(message, "Informations", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        // Rename the current file.
        public void Rename()
        {
            if (!string.IsNullOrEmpty(SearchFolder) && SelectedFile != null)
            {
                RenameDialogWindow rdw = new RenameDialogWindow();
                rdw.fileName.Text = SelectedFile.Name.Replace(SelectedFile.Extension, "");
                Nullable<bool> result = rdw.ShowDialog();

                if(result == true)
                {
                    string newName = rdw.fileName.Text + SelectedFile.Extension;
                    SelectedFile.MoveTo(Path.Combine(SelectedFile.DirectoryName, newName));
                }

                rdw.Close();
            }
        }

        // Remove the current file.
        public void Delete()
        {
            if (!string.IsNullOrEmpty(SearchFolder) && SelectedFile != null)
            {
                DialogResult result = MessageBox.Show("Voulez-vous vraiment supprimer ce fichier ?", "Supprimer le fichier", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if(result == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(SelectedFile.FullName);
                        _randomFileManager.Refresh();
                        SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
                        LaunchButtonEnable = false;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Une erreur est survenue : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Ouvre le fichier précédent
        public void Previous()
        {
            FileInfo file = _randomFileManager.PreviousFile();
            if (file != null)
            {
                SelectedFile = file;

                LaunchButtonEnable = true;

                if(_randomFileManager.RemainingPreviousFiles <= 1)
                    PrevButtonEnable = false;

                if (AutoLaunchOption)
                    Launch();
            }
        }

        // Ouvre le fichier précédent
        public void SearchTextChanged()
        {
            _randomFileManager.SearchText = SearchText;
        }
        #endregion
    }
}
