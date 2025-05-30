using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPlayer.Models
{
    internal class SaveTool
    {
        public static void SetSelectedFolders(List<string> folders)
        {
            Properties.Settings.Default.SelectedFolders = StringifyList(folders);
            Properties.Settings.Default.Save();
        }

        public static List<string> GetSelectedFolders()
        {
            return UnstringifyList(Properties.Settings.Default.SelectedFolders);
        }

        private static string StringifyList(List<string> list)
        {
            return string.Join(";", list);
        }

        private static List<string> UnstringifyList(string stringList)
        {
            if(string.IsNullOrEmpty(stringList))
                return new List<string>();

            return new List<string>(stringList.Split(';'));
        }
    }
}
