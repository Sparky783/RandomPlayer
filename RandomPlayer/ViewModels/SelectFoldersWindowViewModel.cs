using Microsoft.Win32;
using NReco.VideoInfo;
using RandomPlayer.Models;
using RandomPlayer.Models.Command;
using RandomPlayer.Models.Theme;
using RandomPlayer.Views.Controls;
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

namespace RandomPlayer.ViewModels
{
    public class SelectFoldersWindowViewModel : ViewModelNotifier
    {
        private List<string> _selectedFolders;


        public SelectFoldersWindowViewModel()
        {
            // Initialize commands for user.
            InitCommands();
        }

        #region Properties
        public List<string> SelectedFolders
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
        #endregion

        #region Commands
        public ICommand AddFolderCommand { get; private set; }
        public ICommand FinishCommand { get; private set; }

        /// <summary>
        /// Init all commands for WPF view.
        /// </summary>
        private void InitCommands()
        {
            AddFolderCommand = new RelayCommand(x => { AddFolder(); });
            FinishCommand = new RelayCommand(x => { Close(); });
        }
        #endregion

        #region Button methods
        private void AddFolder()
        {

        }

        private void Close()
        {

        }
        #endregion
    }
}
