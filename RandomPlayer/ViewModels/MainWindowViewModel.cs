using Microsoft.Win32;
using NReco.VideoInfo;
using RandomPlayer.Models;
using RandomPlayer.Models.Command;
using RandomPlayer.Models.Theme;
using RandomPlayer.Views;
using RandomPlayer.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private RandomManager<FileInfo> _randomManager;
        private FileSearcher _fileSearcher;
        private ApplicationManager _applicationManager;
        private ThemeManager _themeManager;

        // Load common details controls
        private UserControl pictureDetailsControl = new PictureDetailsControl();
        private UserControl movieDetailsControl = new MovieDetailsControl();
        private UserControl musicDetailsControl = new MusicDetailsControl();
        private UserControl fileDetailsControl = new FileDetailsControl();

        public ObservableCollection<FileTypeOption> FileTypeOptions { get; set; }


        public MainWindowViewModel()
        {
            // Initialize objects
            _applicationManager = new ApplicationManager();
            _themeManager = new ThemeManager();
            _randomManager = new RandomManager<FileInfo>();
            _selectedFolders = new ObservableCollection<string>();

            FileTypeOptions = new ObservableCollection<FileTypeOption>
            {
                new FileTypeOption { Key = "all", Display = "Tous" },
                new FileTypeOption { Key = "movies", Display = "Vidéos" },
                new FileTypeOption { Key = "pictures", Display = "Photos" },
                new FileTypeOption { Key = "songs", Display = "Musiques" }
            };

            // Initialize events for the file research.
            _fileSearcher = new FileSearcher();
            _fileSearcher.StartSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = true;
            };
            _fileSearcher.FinishSearchEvent += (object sender, EventArgs e) => {
                EnableProgressBar = false;
                _randomManager.List = _fileSearcher.FileList;
                FilesLabel = _fileSearcher.Count + " fichiers";
            };

            // Load saved selected folders
            List<string> savedFolders = SaveTool.GetSelectedFolders();

            if (savedFolders.Count > 0)
            {
                SelectedFolders = new ObservableCollection<string>(savedFolders);
                _fileSearcher.SourceFolders = savedFolders;
            }

            // Load saved theme
            _themeManager.ChangeTheme(ThemeManager.ConvertIntToThemeType(Properties.Settings.Default.ThemeType));

            // Load saved file type
            if(!string.IsNullOrEmpty(Properties.Settings.Default.SelectedType))
                SelectedFileType = Properties.Settings.Default.SelectedType;

            // Initialize commands for user.
            InitCommands();

            // Set default options
            CurrentFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            AutoLaunchOption = true;
            SearchSubfolderOption = true;
            PrevButtonEnable = false;
            EnableProgressBar = false;
            FilesLabel = "0 fichiers";

        }

        #region Properties
        /// <summary>
        /// Theme to display
        /// </summary>
        public ThemeType SelectedTheme
        {
            get
            {
                return ThemeManager.ConvertIntToThemeType(Properties.Settings.Default.ThemeType);
            }

            set
            {
                // Save user preference
                Properties.Settings.Default.ThemeType = ThemeManager.ConvertThemeTypeToInt(value);
                Properties.Settings.Default.Save();

                // Set theme to display it
                _themeManager.ChangeTheme(value);
                OnPropertyChanged("SelectedTheme");
            }
        }

        /// <summary>
        /// List of folder used to search files
        /// </summary>
        private ObservableCollection<string> _selectedFolders;
        public ObservableCollection<string> SelectedFolders
        {
            get
            {
                return _selectedFolders;
            }

            set
            {
                _selectedFolders = value;
                OnPropertyChanged("SelectedFolders");
            }
        }

        /// <summary>
        /// Current file opened/displayed
        /// </summary>
        private FileInfo _currentFile;
        public FileInfo CurrentFile
        {
            get
            {
                return _currentFile;
            }

            set
            {
                _currentFile = value;
                OnPropertyChanged("CurrentFile");
            }
        }

        /// <summary>
        /// Type of file to searche
        /// </summary>
        public string SelectedFileType
        {
            get
            {
                switch (_fileSearcher.SelectedType)
                {
                    case FileType.None:
                        return "all";

                    case FileType.Movie:
                        return "movies";

                    case FileType.Picture:
                        return "pictures";

                    case FileType.Song:
                        return "songs";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                 switch (value)
                {
                    case "all":
                        _fileSearcher.SelectedType = FileType.None;
                        ApplciationList = _applicationManager.DefaultApplicationList;
                        break;

                    case "movies":
                        _fileSearcher.SelectedType = FileType.Movie;
                        ApplciationList = _applicationManager.MovieApplicationList;
                        break;

                    case "pictures":
                        _fileSearcher.SelectedType = FileType.Picture;
                        ApplciationList = _applicationManager.PictureApplicationList;
                        break;

                    case "songs":
                        _fileSearcher.SelectedType = FileType.Song;
                        ApplciationList = _applicationManager.MusicApplicationList;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Properties.Settings.Default.SelectedType = value;
                Properties.Settings.Default.Save();

                SelectedApplication = ApplciationList[0];
                OnPropertyChanged("SelectedFileType");
            }
        }

        /// <summary>
        /// List of application can be used to open the current file
        /// </summary>
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

        /// <summary>
        /// Selected application can be used to open the current file
        /// </summary>
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
                return _fileSearcher.UseSubFolders;
            }

            set
            {
                _fileSearcher.UseSubFolders = value;
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
                    _fileSearcher.SearchText = value;
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
        public string FilesLabel
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
                OnPropertyChanged("FilesLabel");
            }
        }

        public UserControl detailsControl;
        public UserControl DetailsControl
        {
            get
            {
                return detailsControl;
            }

            set
            {
                detailsControl = value;
                OnPropertyChanged("DetailsControl");
            }
        }
        
        public Metadata fileMetadata;
        public Metadata FileMetadata
        {
            get
            {
                return fileMetadata;
            }

            set
            {
                fileMetadata = value;
                OnPropertyChanged("FileMetadata");
            }
        }

        public string fileSize;
        public string FileSize
        {
            get
            {
                return fileSize;
            }

            set
            {
                fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }
        #endregion

        #region Commands
        public ICommand SubfolderChangedCommand { get; private set; }
        public ICommand RandomCommand { get; private set; }
        public ICommand LaunchCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }
        public ICommand AddFolderCommand { get; private set; }
        public ICommand RemoveFolderCommand { get; private set; }

        /// <summary>
        /// Init all commands for WPF view.
        /// </summary>
        private void InitCommands()
        {
            SubfolderChangedCommand = new RelayCommand(x => { SubfolderChanged(); });
            RandomCommand = new RelayCommand(x => { Random(); });
            LaunchCommand = new RelayCommand(x => { Launch(); });
            RefreshCommand = new RelayCommand(x => { Refresh(); });
            OpenFolderCommand = new RelayCommand(x => { OpenFolder(); });
            RenameCommand = new RelayCommand(x => { Rename(); });
            DeleteCommand = new RelayCommand(x => { Delete(); });
            PreviousCommand = new RelayCommand(x => { Previous(); });
            QuitCommand = new RelayCommand(x => { Quit(); });
            AddFolderCommand = new RelayCommand(x => { AddFolder(); });
            RemoveFolderCommand = new RelayCommand(x => { RemoveFolder(x); });
        }
        #endregion

        #region Button methods
        /// <summary>
        /// Add a folder to the search list
        /// </summary>
        private void AddFolder()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedFolders.Add(fbd.SelectedPath);

                SaveTool.SetSelectedFolders(SelectedFolders.ToList());
                _fileSearcher.SourceFolders = SelectedFolders.ToList();
            }
        }

        /// <summary>
        /// Remove folder from the search list
        /// </summary>
        /// <param name="parameter"></param>
        private void RemoveFolder(object parameter)
        {
            if (parameter is string item)
            {
                SelectedFolders.Remove(item);

                SaveTool.SetSelectedFolders(SelectedFolders.ToList());
                _fileSearcher.SourceFolders = SelectedFolders.ToList();
            }
        }

        /// <summary>
        /// Open a folder dialog to choose the working directory.
        /// </summary>
        public void SubfolderChanged()
        {
            _randomManager.Refresh();
        }

        /// <summary>
        /// Find a new file from a random function, and try to lunch if it's asked.
        /// </summary>
        public void Random()
        {
            if (CheckSelectedFolders())
            {
                if (_randomManager.Count > 0)
                {
                    ClearDetails();
                    CurrentFile = _randomManager.Next();

                    LaunchButtonEnable = true;
                    PrevButtonEnable = true;

                    // Load details
                    Task.Run(() => { Details(); });

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
                CurrentFile = new FileInfo("Aucun dossier n'est sélèctionné.");
            }
        }

        /// <summary>
        /// Lunch the file if there is one.
        /// </summary>
        public void Launch()
        {
            if (!CheckSelectedFolders() || CurrentFile == null)
                return;

            try
            {
                if(SelectedApplication != null && !string.IsNullOrEmpty(SelectedApplication.Executable))
                {
                    string progamPath = RegistryTools.GetPathForExe(SelectedApplication.Executable);
                    Process.Start(progamPath, "\"" + CurrentFile.FullName + "\"");
                }
                else
                {
                    Process.Start(CurrentFile.FullName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Une erreur est survenue lors du lancement du fichier : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Refresh the list of file
        /// </summary>
        private void Refresh()
        {
            _fileSearcher.Search();
        }

        /// <summary>
        /// Open the folder of the current file.
        /// </summary>
        public void OpenFolder()
        {
            if (!CheckSelectedFolders() || CurrentFile == null)
                return;

            try
            {
                Process.Start(CurrentFile.DirectoryName);
            }
            catch (Exception e)
            {
                MessageBox.Show("Une erreur est survenue lors du l'ouverture du dossier : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Rename the current file.
        /// </summary>
        public void Rename()
        {
            if (!CheckSelectedFolders() || CurrentFile == null)
                return;

            RenameDialogWindow rdw = new RenameDialogWindow();
            rdw.fileName.Text = CurrentFile.Name.Replace(CurrentFile.Extension, "");
            Nullable<bool> result = rdw.ShowDialog();

            if(result == true)
            {
                string newName = rdw.fileName.Text + CurrentFile.Extension;
                CurrentFile.MoveTo(Path.Combine(CurrentFile.DirectoryName, newName));
            }

            rdw.Close();
        }

        /// <summary>
        /// Remove the current file.
        /// </summary>
        public void Delete()
        {
            if (!CheckSelectedFolders() || CurrentFile == null)
                return;

            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimer ce fichier ?", "Supprimer le fichier", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;
            
            try
            {
                File.Delete(CurrentFile.FullName);
                _randomManager.Refresh();
                CurrentFile = new FileInfo("Aucun dossier n'est sélèctionné.");
                LaunchButtonEnable = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Une erreur est survenue : " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Launch the previous file
        /// </summary>
        public void Previous()
        {
            FileInfo file = _randomManager.Previous();
            if (file != null)
            {
                CurrentFile = file;

                LaunchButtonEnable = true;

                if(_randomManager.HistoryCount <= 1)
                    PrevButtonEnable = false;

                ClearDetails();
                Task.Run(() => { Details(); });

                if (AutoLaunchOption)
                    Launch();
            }
        }

        /// <summary>
        /// Close the application
        /// </summary>
        public void Quit()
        {
            App.Current.Shutdown();
        }
        #endregion

        /// <summary>
        /// Get details of the file and display them.
        /// </summary>
        private void Details()
        {
            // Get standard information
            FileSize = FormatFileSize(CurrentFile.Length);

            // Gest file type
            string ext = CurrentFile.Extension.ToLower();

            if (FileExtentions.Pictures.Contains(ext))
            {
                MediaDetails();
                DetailsControl = pictureDetailsControl;
            }
            else if (FileExtentions.Movies.Contains(ext))
            {
                MediaDetails();
                DetailsControl = movieDetailsControl;
            }
            else if (FileExtentions.Musics.Contains(ext))
            {
                MediaDetails();
                DetailsControl = musicDetailsControl;
            }
            else
            {
                DetailsControl = fileDetailsControl;
            }
        }

        /// <summary>
        /// Get details for media file
        /// </summary>
        private void MediaDetails()
        {
            if (!CheckSelectedFolders() || CurrentFile == null)
                return;
            
            // Get media information
            FFProbe ffProbe = new FFProbe();
            FileMetadata = new Metadata(ffProbe.GetMediaInfo(CurrentFile.FullName));
        }

        /// <summary>
        /// Clear details information
        /// </summary>
        private void ClearDetails()
        {
            FileSize = "";
            FileMetadata = null;
        }

        /// <summary>
        /// Convert a file length in byte into string text (Ex: 50 Mo).
        /// </summary>
        /// <param name="fileLength"></param>
        /// <returns></returns>
        private string FormatFileSize(long fileLength)
        {
            string[] sizes = { "o", "Ko", "Mo", "Go", "To", "Po", "Eo" };
            double len = (double)fileLength;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private bool CheckSelectedFolders()
        {
            if (_fileSearcher.SourceFolders.Count <= 0)
                return false;

            foreach (string folder in _fileSearcher.SourceFolders)
            {
                if (!Directory.Exists(folder))
                    return false;
            }

            return true;
        }
    }
}
