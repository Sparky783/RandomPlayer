using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomFile.Models
{
    /// <summary>
    /// Outils de gestions du registre.
    /// </summary>
    public class RegistryTools
    {
        /// <summary>
        /// Récupère le chemin de l'executable du programme recherché.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public string GetPathForExe(string fileName)
        {
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            RegistryKey localMachine = Registry.LocalMachine;
            RegistryKey fileKey = localMachine.OpenSubKey(string.Format(@"{0}\{1}", registryKey, fileName));
            object result = null;

            if (fileKey != null)
            {
                result = fileKey.GetValue(string.Empty);
                fileKey.Close();
            }

            return (string)result;
        }
    }
}
