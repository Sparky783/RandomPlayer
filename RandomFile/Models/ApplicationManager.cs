using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomFile.Models
{
    /// <summary>
    /// Gestionnaire des programme installé sur la machine de l'utilisateur.
    /// </summary>
    public class ApplicationManager
    {
        #region Constantes
        // Application par défaut.
        private static Application _defaultApplication = new Application() { Name = "Par défaut", Executable = null };

        // Liste des progammes de lecture de vidéo potentiellement disponible.
        private static List<Application> _defaultMovieApplicationList = new List<Application>
        {
            new Application(){ Name = "Windows Media Player", Executable = "wmplayer.exe" },
            new Application(){ Name = "VLC", Executable = "vlc.exe" }
        };

        // Liste des progammes de visionnage d'image potentiellement disponible.
        private static List<Application> _defaultPictureApplicationList = new List<Application>
        {
            new Application(){ Name = "Paint", Executable = "pbrush.exe" }
        };

        // Liste des progammes de musique potentiellement disponible.
        private static List<Application> _defaultMusicApplicationList = new List<Application>
        {
        };
        #endregion

        #region Properties
        public List<Application> DefaultApplicationList
        {
            get { return new List<Application> { _defaultApplication }; }
        }

        private List<Application> _movieApplicationList;
        public List<Application> MovieApplicationList
        {
            get { return _movieApplicationList; }
            private set { _movieApplicationList = value; }
        }

        private List<Application> _pictureApplicationList;
        public List<Application> PictureApplicationList
        {
            get { return _pictureApplicationList; }
            private set { _pictureApplicationList = value; }
        }

        private List<Application> _musicApplicationList;
        public List<Application> MusicApplicationList
        {
            get { return _musicApplicationList; }
            private set { _musicApplicationList = value; }
        }
        #endregion

        #region Constructors
        public ApplicationManager()
        {
            MovieApplicationList = new List<Application> { _defaultApplication };
            PictureApplicationList = new List<Application> { _defaultApplication };
            MusicApplicationList = new List<Application> { _defaultApplication };

            MakeProgramLists();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée les liste des programmes disponible suivant la machine de l'utilisateur.
        /// </summary>
        private void MakeProgramLists()
        {
            foreach(Application app in _defaultMovieApplicationList)
            {
                if (RegistryTools.GetPathForExe(app.Executable) != null)
                    MovieApplicationList.Add(app);
            }

            foreach (Application app in _defaultPictureApplicationList)
            {
                if (RegistryTools.GetPathForExe(app.Executable) != null)
                    PictureApplicationList.Add(app);
            }

            foreach (Application app in _defaultMusicApplicationList)
            {
                if (RegistryTools.GetPathForExe(app.Executable) != null)
                    MusicApplicationList.Add(app);
            }
        }
        #endregion
    }
}
