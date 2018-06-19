#region Header
// FixedArray.cs
// PTIRelianceLib
// Cory Todd
// 19-06-2018
// 7:50 AM
#endregion

namespace PTIRelianceLib
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// A read-only fixed length array
    /// </summary>
    /// <typeparam name="T">Data type to hold</typeparam>
    public struct FixedArray<T>
    {
        private readonly IList<T> _mData;

        /// <summary>
        /// Create a new fixed length array of this type
        /// </summary>
        /// <param name="size"></param>
        public FixedArray(int size)
        {
            Size = size;
            _mData = new List<T>(size);
        }

        /// <summary>
        /// Returns the fixed length of this array. This is the max size.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Returns the count of elements in the fixed array.
        /// </summary>
        public int Count => _mData.Count;

        /// <summary>
        /// Puts data into this fixed length array starting from the current tail
        /// of the fixed array. If the fixed array size limit has been reached,
        /// no more data will be added and this method will return.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(params T[] data)
        {
            foreach(var d in data)
            {
                if (_mData.Count >= Size)
                {
                    break;
                }
                _mData.Add(d);
            }
        }

        /// <summary>
        /// Returns a readonly array containg all data
        /// </summary>
        /// <returns></returns>
        public ImmutableArray<T> GetData() => _mData.ToImmutableArray();
    }
}