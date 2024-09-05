using Microsoft.Win32;
using NReco.VideoInfo;
using RandomPlayer.Models;
using RandomPlayer.Models.Command;
using RandomPlayer.Models.Theme;
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
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace RandomPlayer.ViewModels
{
    public class SelectFoldersWindowViewModel : ViewModelNotifier
    {
        private ObservableCollection<string> _selectedFolders;


        public SelectFoldersWindowViewModel()
        {
            // Initialize commands for user.
            InitCommands();

            SelectedFolders = new ObservableCollection<string>(SaveTool.GetSelectedFolders());
        }

        #region Properties
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
        #endregion

        #region Commands
        public ICommand AddFolderCommand { get; private set; }
        public ICommand RemoveFolderCommand { get; private set; }
        public ICommand FinishCommand { get; private set; }

        /// <summary>
        /// Init all commands for WPF view.
        /// </summary>
        private void InitCommands()
        {
            AddFolderCommand = new RelayCommand(x => { AddFolder(); });
            RemoveFolderCommand = new RelayCommand(x => { RemoveFolder(x); });
            FinishCommand = new RelayCommand(x => { });
        }
        #endregion

        #region Button methods
        private void AddFolder()
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();

            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedFolders.Add(ofd.SelectedPath);
                SaveTool.SetSelectedFolders(SelectedFolders.ToList());
            }
        }

        private void RemoveFolder(object parameter)
        {
            if (parameter is string item)
            {
                SelectedFolders.Remove(item);
                SaveTool.SetSelectedFolders(SelectedFolders.ToList());
            }
        }
        #endregion
    }
}
