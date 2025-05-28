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

        #region Constructeur
        public RandomManager()
        {
            _elementsList = new List<T>();
            _elementIndex = 0;
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
        /// Say if there is a previous element.
        /// </summary>
        /// <returns>Number of elemnts in the list</returns>
        public bool HasPrevious
        {
            get { return _elementIndex > 0; }
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

            return _elementsList[_elementIndex];
        }

        /// <summary>
        /// Get the previous element.
        /// </summary>
        /// <returns>Previous element</returns>
        public T Previous()
        {
            if (_elementIndex == 0)
                throw new InvalidOperationException("The history list is empty.");

            _elementIndex--;

            if (_elementIndex < 0)
            {
                Shuffle();
                _elementIndex = _elementsList.Count -1;
            }

            return _elementsList[_elementIndex];
        }

        /// <summary>
        /// Shufffle all elements and reset index position.
        /// </summary>
        public void Refresh()
        {
            _elementIndex = 0;
            Shuffle();
        }

        /// <summary>
        /// Remove the current element from the list.
        /// </summary>
        public void DeleteCurrent()
        {
            _elementsList.RemoveAt(_elementIndex);
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
