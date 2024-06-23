using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPlayer.Models
{
    /// <summary>
    /// Tool to search files.
    /// </summary>
    public class FileSearcher
    {
        private string _selectedFolder;
        private FileType _selectedType;
        private string _searchText;
        private bool _useSubFolders;
        private List<FileInfo> _files;

        #region Constructeur
        public FileSearcher()
        {
            _files = new List<FileInfo>();

            SelectedFolder = "";
            UseSubFolders = false;
            SearchText = "";
            SelectedType = FileType.None;
        }

        public FileSearcher(string folder)
        {
            _files = new List<FileInfo>();

            SelectedFolder = folder;
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
        /// Get the selected folder
        /// </summary>
        public string SelectedFolder
        {
            get { return _selectedFolder; }

            set
            {
                _selectedFolder = value;
                Search();
            }
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
        /// Run the search process.
        /// </summary>
        public void Search()
        {
            _files = new List<FileInfo>();

            if (string.IsNullOrEmpty(_selectedFolder))
                return;

            if (!Directory.Exists(_selectedFolder))
                throw new InvalidOperationException("A folder path must be exist.");

            // Triger start search event
            StartSearchEvent?.Invoke(null, null);

            new Task(() => {
                DirectoryInfo dos = new DirectoryInfo(_selectedFolder);

                SearchOption option = UseSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                string pattern = "*";

                // Update search elements
                if (!string.IsNullOrEmpty(SearchText))
                {
                    string[] searchElement = SearchText.Split(' ');

                    if(searchElement.Length == 1)
                        pattern = "*" + SearchText + "*";
                    else
                    {
                        foreach(string element in searchElement)
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

                if (allFiles)
                    _files = dos.GetFiles(pattern, option).ToList();
                else
                    _files = dos.GetFiles(pattern, option).Where(file => allowedExtensions.Any(file.Extension.ToLower().EndsWith)).ToList();

                // Triger the end search event.
                FinishSearchEvent?.Invoke(null, null);
            }).Start();
        }
        #endregion
    }
}
