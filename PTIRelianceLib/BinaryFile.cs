#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:09 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.IO;

    /// <summary>
    /// A convenience wrapper for arbitrary data. This is a reusable, read-only wrapper 
    /// around the original file that is used throughout this library. A <typeparamref name="BinaryFile"/>
    /// can be created using the static builder methods <see cref="BinaryFile.From(string)"/> or
    /// <see cref="BinaryFile.From(byte[])"/>. Once a <typeparamref name="BinaryFile"/> has been
    /// created, the contents cannot be changed.
    /// </summary>
    public sealed class BinaryFile
    {
        /// <summary>
        /// Backing data
        /// </summary>
        private readonly byte[] _mData;

        /// <summary>
        /// Construct a <typeparamref name="BinaryFile"/> from a physical file location at <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path to source file</param>
        /// <returns>BinaryFile contains data from <paramref name="path"/>. If the file cannot be read,
        /// the result will be empty.</returns>
        public static BinaryFile From(string path)
        {
            if (!File.Exists(path))
            {
                return new BinaryFile(new byte[0]);
            }

            // Make sure to open in read mode in case the file is located on RO directory
            using (var sr = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var data = new byte[sr.Length];
                sr.Read(data, 0, data.Length);
                return new BinaryFile(data);
            }
        }

        /// <summary>
        /// Construct a <typeparamref name="BinaryFile"/> from a buffer in memory.
        /// </summary>
        /// <param name="raw">Memory to copy into a new BinaryFile</param>
        /// <returns>BinaryFile</returns>
        public static BinaryFile From(byte[] raw)
        {
            return new BinaryFile(raw);
        }

        /// <summary>
        /// Construct a new BinaryFile
        /// </summary>
        /// <param name="data">Source data</param>
        private BinaryFile(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _mData = Copy(data);
            Length = data.Length;
            LongLength = data.LongLength;
        }

        /// <summary>Returns the length in bytes of this file</summary>
        /// <value>Integer length of data in this <typeparamref name="BinaryFile"/> in bytes</value>
        public int Length { get; }

        /// <summary>Returns the length in bytes of this file</summary>
        /// <value>Long length of data in this <typeparamref name="BinaryFile"/> in bytes</value>
        public long LongLength { get; }

        /// <summary>
        /// Returns a copy of this data
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetData()
        {
            return Copy(_mData);
        }

        /// <summary>
        /// Read-only indexer
        /// </summary>
        /// <param name="key">Index to read</param>
        /// <returns>byte value at index <paramref name="key"/></returns>
        /// <value>byte value of offset 0-base <paramref name="key"/> within data </value>
        public byte this[int key] => _mData[key];

        /// <summary>
        /// Returns true if this BinaryFile is empty
        /// </summary>
        /// <value>True if no data is in this <typeparamref name="BinaryFile"/></value>
        public bool Empty => Length == 0;

        /// <summary>
        /// Creates a new copy of <paramref name="buff"/>
        /// </summary>
        /// <param name="buff">Source data to copy</param>
        /// <returns>A copy of buff</returns>
        private static byte[] Copy(byte[] buff)
        {
            var dat = new byte[buff.Length];
            Buffer.BlockCopy(buff, 0, dat, 0, buff.Length);
            return dat;
        }
    }
}
