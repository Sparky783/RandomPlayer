using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPlayer.Models
{
    public class SelectedFile
    {
        #region Static object
        public static SelectedFile Empty
        {
            get
            {
                string content = "Aucun dossier n'est sélèctionné.";
                return new SelectedFile(content, content);
            }
        }
        #endregion

        private bool _isEmpty;
        private string _directoryName;

        public SelectedFile(FileInfo file)
        {
            File = file;
            _isEmpty = false;
        }

        public SelectedFile(string fileName, string DirectoryName)
        {
            File = new FileInfo(fileName);
            _directoryName = DirectoryName;
            _isEmpty = true;
        }

        #region Properties
        public FileInfo File { get; set; }

        public string FileName
        {
            get
            {
                return File.Name;
            }
        }

        public string DirectoryName
        {
            get
            {
                return _isEmpty ? _directoryName : File.DirectoryName;
            }
        }
        #endregion
    }
}
