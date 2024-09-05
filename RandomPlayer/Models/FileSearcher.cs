using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RandomPlayer.Models
{
    /// <summary>
    /// Tool to search files.
    /// </summary>
    public class FileSearcher
    {
        private List<string> _sourceFolders;
        private FileType _selectedType;
        private string _searchText;
        private bool _useSubFolders;
        private List<FileInfo> _files;

        #region Constructeur
        public FileSearcher()
        {
            _files = new List<FileInfo>();
            _sourceFolders = new List<string>();

            UseSubFolders = false;
            SearchText = "";
            SelectedType = FileType.None;
        }

        public FileSearcher(string folder)
        {
            _files = new List<FileInfo>();
            _sourceFolders = new List<string>();

            UseSubFolders = false;
            SearchText = "";
            SelectedType = FileType.None;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the file list
        /// </summary>
        public List<FileInfo> FileList
        {
            get { return _files; }
        }

        /// <summary>
        /// Folder sources where the search must be made.
        /// </summary>
        public List<string> SourceFolders
        {
            get { return _sourceFolders; }

            set
            {
                _sourceFolders = value;
                Search();
            }
        }

        /// <summary>
        /// Folder sources where the search must be made.
        /// </summary>
        public bool IsUniqueSource
        {
            get { return _sourceFolders.Count == 1; }
        }

        /// <summary>
        /// Get or define if the subfolders must be browsed.
        /// </summary>
        public bool UseSubFolders
        {
            get { return _useSubFolders; }

            set
            {
                _useSubFolders = value;
                Search();
            }
        }

        /// <summary>
        /// Type of file to find.
        /// </summary>
        public FileType SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                Search();
            }
        }

        /// <summary>
        /// Text must be find in files names.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                Search();
            }
        }

        /// <summary>
        /// Get the number of files found.
        /// </summary>
        public int Count
        {
            get { return _files.Count; }
        }
        #endregion

        #region Events
        public event EventHandler StartSearchEvent;
        public event EventHandler FinishSearchEvent;
        #endregion

        #region Methods
        /// <summary>
        /// Set the source folder for an unique source
        /// </summary>
        /// <param name="folder"></param>
        public void SetSourceFolder(string folder)
        {
            _sourceFolders.Clear();
            _sourceFolders.Add(folder);
        }

        /// <summary>
        /// Set the source folder for multiple sources
        /// </summary>
        /// <param name="folder"></param>
        public void AddSourceFolder(string folder)
        {
            _sourceFolders.Add(folder);
        }

        /// <summary>
        /// Run the search process.
        /// </summary>
        public void Search()
        {
            if (_sourceFolders.Count <= 0)
                return;

            // Prepare search settings
            SearchOption option = UseSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string pattern = "*";

            // Update search elements
            if (!string.IsNullOrEmpty(SearchText))
            {
                string[] searchElement = SearchText.Split(' ');

                if (searchElement.Length == 1)
                    pattern = "*" + SearchText + "*";
                else
                {
                    foreach (string element in searchElement)
                        pattern += "*" + element;

                    pattern += "*";
                }
            }

            string[] allowedExtensions = new string[] { };
            bool allFiles = false;

            switch (_selectedType)
            {
                case FileType.Picture:
                    allowedExtensions = FileExtentions.Pictures;
                    break;

                case FileType.Movie:
                    allowedExtensions = FileExtentions.Movies;
                    break;

                case FileType.Song:
                    allowedExtensions = FileExtentions.Movies;
                    break;

                default: // FileType.None
                    allFiles = true;
                    break;
            }

            List<Task<List<FileInfo>>> tasks = new List<Task<List<FileInfo>>>();

            // Make a new task for each source folder where a search must be made 
            foreach (string folder in _sourceFolders)
            {
                if (!Directory.Exists(folder))
                {
                    return;
                    //throw new InvalidOperationException("A folder path must be exist.");
                }

                tasks.Add(Task.Run(() => {
                    DirectoryInfo dos = new DirectoryInfo(folder);
                    List<FileInfo> fileFound = new List<FileInfo>();

                    if (allFiles)
                        fileFound = dos.GetFiles(pattern, option).ToList();
                    else
                        fileFound = dos.GetFiles(pattern, option).Where(file => allowedExtensions.Any(file.Extension.ToLower().EndsWith)).ToList();

                    return fileFound;
                }));
            }

            new Task(async () => {
                // Triger start search event
                StartSearchEvent?.Invoke(null, null);

                // Attend que toutes les tâches soient terminées
                List<FileInfo>[] results = await Task.WhenAll(tasks);

                _files = new List<FileInfo>();

                foreach (List<FileInfo> fileList in results)
                    _files.AddRange(fileList);

                // Triger the end search event.
                FinishSearchEvent?.Invoke(null, null);
            }).Start();
        }
        #endregion
    }
}
