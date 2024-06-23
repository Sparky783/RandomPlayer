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
    /// Allow to random list of objects<typeparamref name="T"/> and browse between them without get the same two times
    /// </summary>
    public class RandomManager<T>
    {
        private List<T> _elementsList;    // Object list
        private int _elementIndex;        // Current position in the list
        private List<T> _elementsHistory; // List of objet previously get

        #region Constructeur
        public RandomManager()
        {
            _elementsList = new List<T>();
            _elementIndex = 0;
            _elementsHistory = new List<T>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Set the elements list.
        /// </summary>
        public List<T> List
        {
            set
            {
                _elementsList = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get the current element.
        /// </summary>
        /// <returns>Current element</returns>
        public T Current
        {
            get
            {
                if (_elementsList.Count == 0)
                    throw new InvalidOperationException("The element list is empty.");

                return _elementsList[_elementIndex];
            }
        }

        /// <summary>
        /// Get the number of elements.
        /// </summary>
        /// <returns>Number of elemnts in the list</returns>
        public int Count
        {
            get { return _elementsList.Count; }
        }

        /// <summary>
        /// Get the number of objects already gets.
        /// </summary>
        /// <returns>Number of elemnts in the history list</returns>
        public int HistoryCount
        {
            get { return _elementsHistory.Count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the next element
        /// </summary>
        /// <returns>Next element</returns>
        public T Next()
        {
            if (_elementsList.Count == 0)
                throw new InvalidOperationException("The element list is empty.");

            _elementIndex++;

            if (_elementIndex >= _elementsList.Count)
            {
                Shuffle();
                _elementIndex = 0;
            }

            T element = _elementsList[_elementIndex];
            _elementsHistory.Add(element);

            return element;
        }

        /// <summary>
        /// Get the previous element.
        /// </summary>
        /// <returns>Previous element</returns>
        public T Previous()
        {
            if (_elementsHistory.Count == 0)
                throw new InvalidOperationException("The history list is empty.");

            T element = _elementsHistory.Last();
            _elementsHistory.Remove(element);

            return element;
        }

        /// <summary>
        /// Shufffle all elements and reset index position.
        /// </summary>
        public void Refresh()
        {
            _elementIndex = 0;
            _elementsHistory.Clear();
            Shuffle();
        }

        /// <summary>
        /// Shuffle all elements.
        /// </summary>
        private void Shuffle()
        {
            if (_elementsList.Count == 0)
                return;

            Random random = new Random();
            int nbElements = _elementsList.Count;

            for (int index = 0; index < nbElements; index++)
            {
                int randomIndex = random.Next(nbElements);
                T value = _elementsList[randomIndex];
                _elementsList[randomIndex] = _elementsList[index];
                _elementsList[index] = value;
            }
        }
        #endregion
    }
}
