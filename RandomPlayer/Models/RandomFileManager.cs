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
    /// Outil de sélection aléatoire des fichiers d'un dossier.
    /// </summary>
    public class RandomPlayerManager
    {
        private string _selectedFolder;
        private FileType _selectedType;
        private string _searchText;
        private bool _useSubFolders;
        private List<FileInfo> _files;     // Liste des fichiers du dossier.
        private int _fileIndex;            // Position du fichier sélectionné dans les fichier du dossier.
        private List<FileInfo> _filesHistory; // Liste des fichiers ouvert précédement.

        #region Constructeur
        public RandomPlayerManager()
        {
            _files = new List<FileInfo>();
            _fileIndex = 0;
            _filesHistory = new List<FileInfo>();

            SelectedFolder = "";
            UseSubFolders = false;
            SearchText = "";
            SelectedType = FileType.None;
        }

        public RandomPlayerManager(string folder)
        {
            _files = new List<FileInfo>();
            _fileIndex = 0;
            _filesHistory = new List<FileInfo>();

            SelectedFolder = folder;
            UseSubFolders = false;
            SearchText = "";
            SelectedType = FileType.None;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Retourne le dossier en cours
        /// </summary>
        public string SelectedFolder
        {
            get { return _selectedFolder; }

            set
            {
                _selectedFolder = value;
                Refresh();
            }
        }

        /// <summary>
        /// Set ou retourne si les sous-dossier doivent être utilisés.
        /// </summary>
        public bool UseSubFolders
        {
            get { return _useSubFolders; }

            set
            {
                _useSubFolders = value;
                Refresh();
            }
        }

        /// <summary>
        /// Type de fichier sélectionné.
        /// </summary>
        public FileType SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                Refresh();
            }
        }

        /// <summary>
        /// Selctionne les fichiers correspondant à la chaine de caractères de recherche.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                Refresh();
            }
        }

        /// <summary>
        /// Retourne le fichier en cours.
        /// </summary>
        public FileInfo CurrentFile
        {
            get
            {
                if (_filesHistory.Count > 0)
                    return _filesHistory.Last();

                return null;
            }
        }

        /// <summary>
        /// Retroune le nombre de fichier correspondant au critères.
        /// </summary>
        public int FileCount
        {
            get { return _files.Count; }
        }

        /// <summary>
        /// Retroune le nombre de fichier parcourru. Le fichier en cours étant exclus.
        /// </summary>
        public int RemainingPreviousFiles
        {
            get { return _filesHistory.Count; }
        }
        #endregion

        #region Events
        public event EventHandler StartSearchEvent;
        public event EventHandler FinishSearchEvent;
        #endregion

        #region Methods
        /// <summary>
        /// Retourne le fichier suivant.
        /// </summary>
        /// <returns></returns>
        public FileInfo NextFile()
        {
            if (_files.Count > 0)
            {
                if (_fileIndex >= _files.Count)
                {
                    Shuffle();
                    _fileIndex = 0;
                }

                FileInfo file = _files[_fileIndex];
                _filesHistory.Add(file);
                _fileIndex ++;

                return file;
            }

            return null;
        }

        /// <summary>
        /// Retourne le fichier précédent.
        /// </summary>
        /// <returns></returns>
        public FileInfo PreviousFile()
        {
            if (_filesHistory.Count > 1)
            {
                _filesHistory.Remove(_filesHistory.Last());
                return _filesHistory.Last();
            }

            return null;
        }

        /// <summary>
        /// Récupère tous les fichiers correspondant aux critères de recherche de l'utilisateur et les mélanges.
        /// </summary>
        public void Refresh()
        {
            _files = new List<FileInfo>();

            if (string.IsNullOrEmpty(_selectedFolder))
                return;

            if (!Directory.Exists(_selectedFolder))
                return;

            // Déclanche l'événement de début de recherche.
            StartSearchEvent?.Invoke(null, null);

            new Task(() => {
                DirectoryInfo dos = new DirectoryInfo(_selectedFolder);

                SearchOption option = UseSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                string pattern = "*";

                if (!string.IsNullOrEmpty(SearchText))
                    pattern = "*" + SearchText + "*";

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

                _fileIndex = 0; // Réinitialise la position dans la liste de fichiers.
                Shuffle();

                // Déclanche l'événement de fin de recherche.
                FinishSearchEvent?.Invoke(null, null);
            }).Start();
        }

        /// <summary>
        /// Mélange le dossier en cours.
        /// </summary>
        public void Shuffle()
        {
            if (!string.IsNullOrEmpty(_selectedFolder) && _files.Count > 0)
            {
                Random rng = new Random();
                int tot = _files.Count;

                for (int i = 0; i < tot; i++)
                {
                    int k = rng.Next(tot);
                    FileInfo value = _files[k];
                    _files[k] = _files[i];
                    _files[i] = value;
                }
            }
        }
        #endregion
    }
}
