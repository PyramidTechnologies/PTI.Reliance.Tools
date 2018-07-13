#region Header

// RELLogoParser.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 12:47 PM

#endregion

namespace PTIRelianceLib.Logo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using Protocol;

    /// <summary>
    /// Reads in an image and processes data into a packet list
    /// </summary>
    internal class RELLogoParser
    {
        /// <summary>
        /// Logo data type code
        /// </summary>
        private const byte LogoType = 0x03;

        /// <summary>
        /// External flash chip alignment
        /// 4096 bytes (4K)
        /// </summary>
        private const int Alignment = 0x1000;

        /// <summary>
        /// USB HID max packet size is controlled by firmware in Reliance
        /// </summary>
        private const int MaxPacketSize = 0x20;

        /// <summary>
        /// Parse file as a logo
        /// </summary>
        /// <param name="startAddress">Starting storage address</param>
        /// <param name="data">Logo data to parse</param>
        /// <returns>Packetized list or null on error</returns>
        public IList<byte[]> Parse(uint startAddress, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var repacked = new List<byte[]>();
            var flashCmd = new[] {(byte) RelianceCommands.DataWriteRequest, LogoType};

            // data portion of the packet. There are 4 bytes of packaging
            const int dataLen = MaxPacketSize - 4;

            // Start requesting flash permission at this address
            var addr = startAddress;

            // There is a chance we will not have an even count of FLASH_BLOCK_SIZE
            // byte in the stream. Count the bytes and check for this condition. If 
            // detected, we must random-fill the block to round out the data.

            // read into a block that will be crc'd
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position != stream.Length)
                {
                    var crcBlock = new byte[Alignment];
                    var readThisMuch = Alignment;

                    // Detect if there is enough left to read. Use the stream since
                    // it is shared with the reader.
                    if (stream.Length - stream.Position < Alignment)
                    {
                        readThisMuch = (int) (stream.Length - stream.Position);
                    }

                    // Random fill if needed
                    if (reader.Read(crcBlock, 0, readThisMuch) != Alignment)
                    {
                        var fillMe = new byte[Alignment - readThisMuch];
                        using (var rnd = new RNGCryptoServiceProvider())
                        {
                            rnd.GetBytes(fillMe);
                        }

                        Array.Copy(fillMe, 0, crcBlock, readThisMuch, fillMe.Length);
                    }


                    // Checksum that whole thing
                    var csum = Crc32.ComputeChecksum(crcBlock);

                    // Buffer for the flash permission request packet
                    // Hold however many bytes are in flash command 
                    // and the adress + checksum (4 bytes each)
                    using (var req = new MemoryStream(flashCmd.Length + 8))
                    {
                        req.Write(flashCmd, 0, flashCmd.Length);

                        // Create the flash permission request by injecting address and then checksum buffers
                        var tmp = addr.ToBytesBE();
                        req.Write(tmp, 0, tmp.Length);

                        tmp = csum.ToBytesBE();
                        req.Write(tmp, 0, tmp.Length);

                        // req packet is now complete.
                        req.Position = 0;
                        repacked.Add(req.GetBuffer());
                    }

                    // Now split the crcBlock into HID compatible packet lengths.
                    // The protocol specifies that we must provide the sequence number
                    // inside the packet. This must never exceed 255 sequences per block.
                    // Meaning, we cannot use less than 9 bytes in a flash payload packet.
                    byte seq = 0;
                    foreach (var segment in crcBlock.Split(dataLen))
                    {
                        // We're inserting 2 bytes in front of the segment payload
                        var payload = new byte[dataLen + 2];
                        payload[0] = (byte) RelianceCommands.FlashDo;
                        payload[1] = seq++;

                        Array.Copy(segment, 0, payload, 2, segment.Length);
                        repacked.Add(payload);
                    }

                    // Move address to next block
                    addr += Alignment;
                }
            }

            return repacked;
        }
    }
}