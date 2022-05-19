using Microsoft.Win32;
using NReco.VideoInfo;
using RandomPlayer.Models;
using RandomPlayer.Models.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = RandomPlayer.Models.Application;

namespace RandomPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelNotifier
    {
        private const string WORKING_FOLDER = "RandomPlayerContent";

        private RandomPlayerManager _randomFileManager;
        private ApplicationManager _applicationManager;

        public MainWindowViewModel()
        {
            _randomFileManager = new RandomPlayerManager();
            _randomFileManager.StartSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = true;
            };
            _randomFileManager.FinishSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = false;
                Files = _randomFileManager.FileCount + " fichiers";
            };
            
            // Défini le dossier de travail par défaut
            if(string.IsNullOrEmpty(Properties.Settings.Default.DefaultFolder))
            {
                Properties.Settings.Default.DefaultFolder = AppDomain.CurrentDomain.BaseDirectory;
                Properties.Settings.Default.Save();
            }
            _randomFileManager.SelectedFolder = Properties.Settings.Default.DefaultFolder;

            _applicationManager = new ApplicationManager();

            // Default option
            SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            AutoLaunchOption = true;
            SearchSubfolderOption = true;
            PrevButtonEnable = false;
            EnableProgressBar = false;
            Files = "0 fichiers";

            // Création du dossier de travail si celui-ci n'existe pas
            if (!Directory.Exists(_randomFileManager.SelectedFolder))
                Directory.CreateDirectory(_randomFileManager.SelectedFolder);

            InitCommands();
        }

        #region Properties
        public string SelectedFolder
        {
            get
            {
                return _randomFileManager.SelectedFolder;
            }

            set
            {
                if(Directory.Exists(value))
                {
                    Properties.Settings.Default.DefaultFolder = value;
                    _randomFileManager.SelectedFolder = Properties.Settings.Default.DefaultFolder;

                    OnPropertyChanged("SelectedFolder");
                }
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
                if (value != searchText)
                {
                    searchText = value;
                    _randomFileManager.SearchText = value;
                    OnPropertyChanged("SearchText");
                }
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

        public Metadata fileInfo;
        public Metadata FileInfo
        {
            get
            {
                return fileInfo;
            }

            set
            {
                fileInfo = value;
                OnPropertyChanged("FileInfo");
            }
        }
        #endregion

        #region Commands
        public ICommand SelectFolderCommand { get; private set; }
        public ICommand SubfolderChangedCommand { get; private set; }
        public ICommand RandomCommand { get; private set; }
        public ICommand LaunchCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand OpenFolderCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }

        private void InitCommands()
        {
            SelectFolderCommand = new RelayCommand(x => { SelectFolder(); });
            SubfolderChangedCommand = new RelayCommand(x => { SubfolderChanged(); });
            RandomCommand = new RelayCommand(x => { Random(); });
            LaunchCommand = new RelayCommand(x => { Launch(); });
            RefreshCommand = new RelayCommand(x => { Refresh(); });
            OpenFolderCommand = new RelayCommand(x => { OpenFolder(); });
            DetailsCommand = new RelayCommand(x => { Details(); });
            RenameCommand = new RelayCommand(x => { Rename(); });
            DeleteCommand = new RelayCommand(x => { Delete(); });
            PreviousCommand = new RelayCommand(x => { Previous(); });
        }
        #endregion

        #region Button methods
        /// <summary>
        /// Open a folder dialog to choose the working directory.
        /// </summary>
        public void SelectFolder()
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();

            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SelectedFolder = ofd.SelectedPath;
        }

        /// <summary>
        /// Open a folder dialog to choose the working directory.
        /// </summary>
        public void SubfolderChanged()
        {
            _randomFileManager.Refresh();
        }

        /// <summary>
        /// Find a new file from a random function, and try to lunch if it's asked.
        /// </summary>
        public void Random()
        {
            if (!string.IsNullOrEmpty(WORKING_FOLDER))
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
                    MessageBox.Show("Aucun fichier correspondant aux critères n'a été trouvé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            }
        }

        /// <summary>
        /// Lunch the file if there is one.
        /// </summary>
        public void Launch()
        {
            if (!string.IsNullOrEmpty(WORKING_FOLDER) && _randomFileManager.CurrentFile != null)
            {
                // Load details
                Task.Run(() => { Details(); });

                try
                {
                    if(SelectedApplication != null && !string.IsNullOrEmpty(SelectedApplication.Executable))
                    {
                        string progamPath = RegistryTools.GetPathForExe(SelectedApplication.Executable);
                        Process.Start(progamPath, "\"" + _randomFileManager.CurrentFile.FullName + "\"");
                    }
                    else
                    {
                        Process.Start(_randomFileManager.CurrentFile.FullName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Une erreur est survenue lors du lancement du fichier : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Refresh the list of file
        /// </summary>
        private void Refresh()
        {
            _randomFileManager.Refresh();
        }

        /// <summary>
        /// Open the folder of the current file.
        /// </summary>
        public void OpenFolder()
        {
            if (!string.IsNullOrEmpty(WORKING_FOLDER) && _randomFileManager.CurrentFile != null)
            {
                try
                {
                    Process.Start(_randomFileManager.CurrentFile.DirectoryName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Une erreur est survenue lors du l'ouverture du dossier : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Get details of the file and display them.
        /// </summary>
        public void Details()
        {
            if (!string.IsNullOrEmpty(WORKING_FOLDER) && _randomFileManager.CurrentFile != null)
            {
                FFProbe ffProbe = new FFProbe();
                FileInfo = new Metadata(ffProbe.GetMediaInfo(_randomFileManager.CurrentFile.FullName));
            }
        }

        /// <summary>
        /// Rename the current file.
        /// </summary>
        public void Rename()
        {
            if (!string.IsNullOrEmpty(WORKING_FOLDER) && SelectedFile != null)
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
            if (!string.IsNullOrEmpty(WORKING_FOLDER) && _randomFileManager.CurrentFile != null)
            {
                MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimer ce fichier ?", "Supprimer le fichier", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if(result == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(_randomFileManager.CurrentFile.FullName);
                        _randomFileManager.Refresh();
                        SelectedFile = new FileInfo("Aucun dossier n'est sélèctionné.");
                        LaunchButtonEnable = false;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Une erreur est survenue : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
        #endregion
    }
}
