using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomFile;
using RandomFile.Models;

namespace UnitTests
{
    [TestClass]
    public class RandomFileManagerTests
    {
        /// <summary>
        /// Spécifie le dossier de test.
        /// </summary>
        private static string TEST_FOLDER = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFolder");

        /// <summary>
        /// Test le constructeur simple.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            RandomFileManager manager = new RandomFileManager();

            Assert.AreEqual("", manager.SelectedFolder);
            Assert.AreEqual(false, manager.UseSubFolders);
            Assert.AreEqual("", manager.SearchText);
            Assert.AreEqual(FileType.None, manager.SelectedType);
            Assert.AreEqual(0, manager.FileCount);

            Assert.IsNull(manager.CurrentFile);
        }

        /// <summary>
        /// Test le constructeur avec paramètre.
        /// </summary>
        [TestMethod]
        public void TestConstructorWithParameters()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);

            Assert.AreEqual(TEST_FOLDER, manager.SelectedFolder);
            Assert.IsTrue(manager.FileCount > 0);
        }

        /// <summary>
        /// Vérifie que les fichiers sont dans le désordre.
        /// </summary>
        [TestMethod]
        public void TestSuffledFiles()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            FileInfo file = manager.NextFile();

            for (int i = 1; i < manager.FileCount; i ++)
            {
                FileInfo newfile = manager.NextFile();
                int valFileA = int.Parse(Path.GetFileNameWithoutExtension(file.Name));
                int valFileB = int.Parse(Path.GetFileNameWithoutExtension(newfile.Name));

                if (valFileA + 1 != valFileB)
                {
                    Assert.IsTrue(true);
                    return;
                }

                file = newfile;
            }
        }

        /// <summary>
        /// Vérifie que le fichier suivant ne corresponde pas a un fichier précedent. (Ouverture unique)
        /// </summary>
        [TestMethod]
        public void TestUniqueNextFile()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            List<FileInfo> files = new List<FileInfo>();

            for (int i = 0; i < manager.FileCount; i++)
            {
                FileInfo file = manager.NextFile();

                if (files.Contains(file))
                    Assert.Fail();
                else
                    files.Add(file);
            }
        }

        /// <summary>
        /// Vérifie que le fichier précédent est bien le bon.
        /// </summary>
        [TestMethod]
        public void TestPreviousFile()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);

            FileInfo file = null;
            Random rnd = new Random();
            int val = rnd.Next(1, manager.FileCount);
            
            for (int i = 0; i < val; i++)
                file = manager.NextFile();

            manager.NextFile();

            Assert.IsTrue(manager.RemainingPreviousFiles > 0);
            Assert.AreEqual(file, manager.PreviousFile());
        }

        /// <summary>
        /// Vérifie qu'il n'est pas possible de récupérer le fichier précédent s'il n'y en a pas. (Au début de série)
        /// </summary>
        [TestMethod]
        public void TestFirstPreviousFile()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            
            Assert.IsNull(manager.PreviousFile());
            Assert.AreEqual(0, manager.RemainingPreviousFiles);
        }

        /// <summary>
        /// Vérifie que deux ouverture d'un même dossier fournis deux ordre différent
        /// </summary>
        [TestMethod]
        public void TestSuffleFunction()
        {
            RandomFileManager managerA = new RandomFileManager(TEST_FOLDER);
            RandomFileManager managerB = new RandomFileManager(TEST_FOLDER);

            Assert.AreEqual(managerA.FileCount, managerB.FileCount);

            string listA = "";
            string listB = "";

            for (int i = 0; i < managerA.FileCount; i++)
            {
                listA += managerA.NextFile().Name + "/";
                listB += managerB.NextFile().Name + "/";
            }

            Assert.AreEqual(listA, listB);
        }

        /// <summary>
        /// Vérifie qu'il est possible de travailler que sur les fichiers du dossier courrant et non sur les fichiers des sous-dossiers.
        /// </summary>
        [TestMethod]
        public void TestCurrentFolderOnly()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            List<string> expectedList = new List<string> { "1.txt", "2.txt", "3.txt", "4.avi", "5.mp4", "6.wmv", "7.flv", "8.jpg", "9.png", "10.bmp" };

            for (int i = 0; i < manager.FileCount; i++)
            {
                if (!expectedList.Contains(manager.NextFile().Name))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Vérifie qu'il est possible de travailler que sur les fichiers du dossier courrant et des sous-dossiers.
        /// </summary>
        [TestMethod]
        public void TestCurrentFolderAndSubFolder()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            manager.UseSubFolders = true;

            List<string> expectedList = new List<string> {
                "1.txt", "2.txt", "3.txt", "4.avi", "5.mp4", "6.wmv", "7.flv", "8.jpg", "9.png", "10.bmp",
                "11.txt", "12.txt", "13.txt", "14.avi", "15.mp4", "16.wmv", "17.flv", "18.jpg", "19.png", "20.bmp"
            };

            for (int i = 0; i < manager.FileCount; i++)
            {
                if (!expectedList.Contains(manager.NextFile().Name))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Vérifie qu'il est possible de faire un mélange uniquement sur un fichier contenant une certaine chaine de caractère.
        /// </summary>
        [TestMethod]
        public void TestSpecificText()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            manager.SearchText = "2";
            
            Assert.AreEqual("2.txt", manager.NextFile().Name);
        }

        /// <summary>
        /// Vérifie qu'il est possible de faire un mélange uniquement sur un type préci (Image, Vidéo, Son, etc ...).
        /// </summary>
        [TestMethod]
        public void TestSpecificType()
        {
            RandomFileManager manager = new RandomFileManager(TEST_FOLDER);
            manager.SelectedType = FileType.Movie;

            List<string> expectedList = new List<string> {
                "4.avi", "5.mp4", "6.wmv", "7.flv",
                "14.avi", "15.mp4", "16.wmv", "17.flv",
            };

            for (int i = 0; i < manager.FileCount; i++)
            {
                if (!expectedList.Contains(manager.NextFile().Name))
                    Assert.Fail();
            }
        }
    }
}
