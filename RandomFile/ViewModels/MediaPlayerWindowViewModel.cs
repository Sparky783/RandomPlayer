using RandomFile.Models;
using RandomFile.Models.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RandomFile.ViewModels
{
    public class MediaPlayerWindowViewModel : ViewModelNotifier
    {
        #region Commands
        /*public ICommand PlayCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }
        public ICommand NextCommand { get; private set; }
        public ICommand ScreenshotCommand { get; private set; }
        public ICommand RandomCommand { get; private set; }
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
        }*/
        #endregion
    }
}
