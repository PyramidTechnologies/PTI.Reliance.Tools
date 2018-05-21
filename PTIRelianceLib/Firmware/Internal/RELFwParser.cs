#region Header
// RelianceFwParser.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:11 AM
#endregion

namespace PTIRelianceLib.Firmware.Internal
{
    using System;
    using System.IO;

    internal class RELFwParser
    {
        /// <summary>
        /// MAGIC_NUM + CRC32 + Model|Algorithm + IV + Starting Address = 48              
        /// </summary>
        public const ulong PayloadStart = 48;

        public RELFwHeader Header { get; }

        private byte[] RawData { get; }

        public RELFwParser(byte[] data)
        {
            Header = GetHeaderData(data);
            RawData = data;
        }

        /// <summary>
        /// Deobfuscates data per the model specification. If the data is invalid
        /// then an empty buffer will be returned.
        /// </summary>
        /// <returns>Deobfuscated data</returns>
        public byte[] Deobfuscate()
        {
            if (!Header.IsValid)
            {
                return new byte[0];
            }

            switch (Header.Model)
            {
                case 0x501: // Securit file
                case 0x500: // Base file
                    return RelianceV1Deobfuscator();

                default: return new byte[0];
            }
        }

        /// <summary>
        /// Implements the deobfuscation used for v1 Reliance firmware. If this is not
        /// a proper firmware, an empty buffer will instead be returned.
        /// </summary>
        /// <returns>Deobfuscated data</returns>
        private byte[] RelianceV1Deobfuscator()
        {
            // Skip to payload
            var payloadLen = (ulong)RawData.LongLength - PayloadStart;

            // Ensure we have a sane length
            if (payloadLen <= 0)
            {
                return new byte[0];
            }

            // Verify checksum integrity. This is the CRC32 of 
            // the entire file EXCEPT for the 8-bit magic number 
            // and the 32-bit CRC value (i.e. skip the first 12 bytes)
            var result = new byte[RawData.Length - 8 - 4];
            Array.Copy(RawData, 12, result, 0, result.Length);

            var actualCrc32 = Crc32.ComputeChecksum(result);
            if (actualCrc32 != Header.ExpectedCsum)
            {
                return new byte[0];
            }

            // Checksum is okay, load up the payload to return as result
            result = new byte[payloadLen];
            Array.Copy(RawData, (int)PayloadStart, result, 0, result.Length);

            return result;
        }

        internal static RELFwHeader GetHeaderData(byte[] data)
        {
            var result = new RELFwHeader();

            // This cannot be a valid file if there is less than this many bytes
            if (data == null || (ulong)data.LongLength < PayloadStart)
            {
                return result;
            }

            try
            {
                using (var stream = new MemoryStream(data))
                using (var reader = new BinaryReader(stream))
                {

                    // Test for magic number in file. Should be "PTIXPTIX"
                    result.SetMagicNumber(reader.ReadBytes(8)); // 8
                    if (!result.MagicNum.GetPrintableString().Equals("PTIXPTIX"))
                    {
                        return result;
                    }

                    // Read in all the file metadata
                    result.ExpectedCsum = reader.ReadUInt32();  // 4
                    result.Model = reader.ReadUInt32();         // 4
                    result.Algorithm = reader.ReadUInt32();     // 4
                    result.SetIdMat(reader.ReadBytes(16));      // 16
                    result.StartAddr = reader.ReadUInt32();     // 4
                    result.OriginalSize = reader.ReadUInt64();  // 8

                    result.RawFileSize = (uint)data.Length;

                    // There should be exactly this many bytes in the payload
                    result.IsValid =
                        (ulong)data.LongLength - PayloadStart == result.OriginalSize;
                }
            }
            catch (Exception e)
            {
                throw new PTIException("Error reading firmware header: {0}", e.Message);
            }

            return result;
        }
    }
}