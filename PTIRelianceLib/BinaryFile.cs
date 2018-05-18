﻿#region Header
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
    /// Convenience wrapper for BinaryFile data. This is a reusable read-only wrapper 
    /// around the original file data and it lives in memory.
    /// Do not use for massive files.
    /// </summary>
    public class BinaryFile
    {
        private readonly byte[] _mData;

        /// <summary>
        /// Construct a firmware object from a physical file
        /// </summary>
        /// <param name="path">Path to source file</param>
        /// <returns>BinaryFile contains data from file. If the file cannot be read, the result
        /// will be empty.</returns>
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
        /// Construct a firmware from a buffer in memory
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static BinaryFile From(byte[] raw)
        {
            return new BinaryFile(raw);
        }

        /// <summary>
        /// Construct a new Firmware
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

        /// <summary>
        /// Returns the length in bytes of this file
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Returnst he length in bytes of this file
        /// </summary>
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
        /// <param name="key"></param>
        /// <returns></returns>
        public byte this[int key] => _mData[key];

        /// <summary>
        /// Returns true if this binary file is empty
        /// </summary>
        public bool Empty => Length == 0;

        /// <summary>
        /// Creates a new copy of the source data
        /// </summary>
        /// <param name="buff">Source data</param>
        /// <returns>New, identical copy</returns>
        private static byte[] Copy(byte[] buff)
        {
            var dat = new byte[buff.Length];
            Buffer.BlockCopy(buff, 0, dat, 0, buff.Length);
            return dat;
        }
    }
}
