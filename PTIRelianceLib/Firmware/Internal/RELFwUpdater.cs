#region Header
// RelianceFwFlasher.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:04 AM
#endregion

namespace PTIRelianceLib.Firmware.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using Protocol;
    using Transport;

    /// <summary>
    /// Executes high-level flash update algorithm
    /// </summary>
    internal class RELFwUpdater : IFlashUpdater
    {

        private readonly BinaryFile _mFileToFlash;

        private readonly IPort<IPacket> _mPort;

        /// <summary>
        /// Constructs a new flasher
        /// </summary>
        public RELFwUpdater(IPort<IPacket> port, BinaryFile fileToFlash)
        {
            _mPort = port;
            _mFileToFlash = fileToFlash;
            RunBefore = new List<Func<ReturnCodes>>();
            RunAfter = new List<Func<ReturnCodes>>();
        }
        
        public IProgressMonitor Reporter { get; set; }

        public FileTypes FileType { get; set; }

        /// <summary>
        /// Sets the raw command to run before flash updating. If any
        /// of these command fail, the flash update process will not
        /// occur.
        /// </summary>
        public IList<Func<ReturnCodes>> RunBefore { get; set; }

        /// <summary>
        /// Sets the command to run after flash updating.
        /// If any commands fail, the process will return immediately.
        /// </summary>
        public IList<Func<ReturnCodes>> RunAfter { get; set; }

        /// <inheritdoc />
        public virtual ReturnCodes ExecuteUpdate()
        {
            ReturnCodes result;


            if (FileType != FileTypes.Base)
            {
                return ReportIncompatible();
            }

            if (_mFileToFlash.Empty)
            {
                return ReturnCodes.FlashFileInvalid;
            }

            // Run any setup commands
            foreach (var fn in RunBefore)
            {
                result = fn.Invoke();
                if (result != ReturnCodes.Okay)
                {
                    return result;
                }
            }

            var packets = ProcessFirmware(_mFileToFlash.GetData());

            // Transmit to target
            var streamer = new RELFwStreamer(Reporter, _mPort);
            result = streamer.StreamFlashData(packets);


            // Run any cleanup commands
            foreach (var fn in RunAfter)
            {
                result = fn.Invoke();
                if (result != ReturnCodes.Okay)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Report to client that the firmware is not compatible with their unit
        /// </summary>
        /// <returns>ReturnCode.FlashRequestDenied always</returns>
        protected ReturnCodes ReportIncompatible()
        {
            Reporter.ReportFailure("The selected firmware is not compatible with your device. " +
                                   "Please contact support@pyramidacceptors.com and request the latest firmware for Reliance.");
            return ReturnCodes.FlashPermissionDenied;
        }

        private IList<IPacket> ProcessFirmware(byte[] data)
        {
            var parser = new RELFwParser(data);
            var deobfuscated = parser.Deobfuscate();
            var header = RELFwParser.GetHeaderData(_mFileToFlash.GetData());
            var memoryMap = new RelianceMap(header.StartAddr);

            // data portion of the packet. There are 4 bytes of packaging
            const int dataLen = 28;
            const int blockSize = 0x800;
            var flashCmd = new[] {(byte) RelianceCommands.FlashRequest};
            var result = new List<IPacket>();

            // Inject ID matrix
            var idMat = _mPort.Package(header.IdMatrix);
            idMat.Prepend(0x45);
            result.Add(idMat);

            try
            {
                // Start requesting flash permission at this address
                var addr = memoryMap.FirstAddress;

                // There is a chance we will not have an even count of FLASH_BLOCK_SIZE
                // byte in the stream. Count the bytes and check for this condition. If 
                // detected, we must random-fill the block so round out the data. This 
                // is because encrypting small segments can expose portions of our AES surface.

                // read into a block that will be crc'd
                using (var stream = new MemoryStream(deobfuscated))
                using (var reader = new BinaryReader(stream))
                {
                    while (stream.Position != stream.Length)
                    {

                        var crcBlock = new byte[blockSize];
                        var readThisMuch = blockSize;

                        // Detect if there is enough left to read. Use the stream since
                        // it is shared with the reader.
                        if (stream.Length - stream.Position < blockSize)
                        {
                            readThisMuch = (int) (stream.Length - stream.Position);
                        }

                        // Random fill if needed
                        if (reader.Read(crcBlock, 0, readThisMuch) != blockSize)
                        {
                            var fillMe = new byte[blockSize - readThisMuch];
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
                            
                            result.Add(_mPort.Package(req.GetBuffer()));
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
                            
                            result.Add(_mPort.Package(payload));
                        }

                        // Move address to next block
                        addr += blockSize;
                    }
                }
            }
            catch (Exception e)
            {
                throw new PTIException("Failed to repack firmware: {0}", e.Message);
            }

            return result;
        }

        internal struct RelianceMap : IMemoryMap
        {
            public RelianceMap(uint first)
            {
                FirstAddress = first;
            }

            public uint FirstAddress { get; set; }

            public bool IsIllegalAddress(uint address)
            {
                return address < FirstAddress;
            }

            public bool IsRangeChecksummed(uint address)
            {
                return true;
            }

            public bool IsLastAddress(uint address)
            {
                return false;
            }
        }
    }
}