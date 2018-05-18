#region Header
// RELFwStreamer.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:42 AM
#endregion

namespace PTIRelianceLib.Firmware.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Protocol;
    using Transport;

    /// <inheritdoc />
    /// <summary>
    /// Handles the low-level protocol for flash updating a Reliance thermal printer
    /// </summary>
    internal class RELFwStreamer : IDataStreamer
    {
        public RELFwStreamer(IProgressMonitor reporter, IPort<IPacket> port)
        {
            Reporter = reporter;
            Port = port;
        }

        public IProgressMonitor Reporter { get; set; }

        public IPort<IPacket> Port { get; set; }

        public ReturnCodes StreamFlashData(IList<IPacket> packetList)
        {
            var status = ReturnCodes.InvalidRequestPayload;

            if (packetList == null || packetList.Count == 0)
            {
                return status;
            }

            var increment = 1.00 / packetList.Count + double.Epsilon;
            var progress = 0.0;

            // Start a tracker with 5 up to five retries
            var tracker = new FlashTracker(5);
            var packetQ = new Queue<IPacket>(packetList);

            IPacket Write(IPacket data)
            {
                if (!data.IsPackaged)
                {
                    data.Package();
                }

                if (!Port.Write(data))
                {
                    return Port.PacketLanguage;
                }

                var resp = Port.Read(200);
                return resp.ExtractPayload();
            }


            var payload = packetQ.Dequeue();
            Reporter.ReportMessage("Starting flash update process");
            do
            {
                var resp = Write(payload);

                // Stats
                tracker.TotalBytesTx += payload.Count;
                ++tracker.TotalPacketsTx;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (resp.GetPacketType())
                {
                    case PacketTypes.PositiveAck:
                        tracker.Status = FlashState.Next;
                        progress += increment;
                        Reporter.ReportProgress(progress);
                        break;


                    case PacketTypes.Busy:
                    case PacketTypes.Malformed:
                    case PacketTypes.NegativeAck:
                    case PacketTypes.SequenceError:
                        Reporter.ReportMessage("Flash Error: {0}", resp.GetPacketType());
                        tracker.Status = FlashState.Retry;
                        ++tracker.RetryCount;
                        break;

                    case PacketTypes.Timeout:
                        tracker.Status = FlashState.Retry;
                        ++tracker.Timeouts;
                        ++tracker.RetryCount;
                        Reporter.ReportMessage("Flash Timeout {0}/{1}", tracker.Timeouts, tracker.RetryLimit);
                        break;

                    default:
                        Reporter.ReportMessage("Unknown Response: {0}", resp.ToString());
                        tracker.Status = FlashState.Retry;
                        ++tracker.RetryCount;
                        break;
                }

                if (tracker.Status == FlashState.Next)
                {
                    // If there are no more, then we have sent all the data
                    // and can move on to checksum processing.                    
                    if (!packetQ.Any())
                    {
                        tracker.Status = FlashState.Success;
                        status = ReturnCodes.Okay;
                    }
                    else
                    {
                        payload = packetQ.Dequeue();
                    }
                }
                else if (tracker.Status == FlashState.SkipThisBlock)
                {
                    // This could be a range that we should not send to the target
                    // Skip through payload data to find start of next block
                    while (true)
                    {
                        // No more data, we're done
                        if (!packetQ.Any())
                        {
                            tracker.Status = FlashState.Success;
                            status = ReturnCodes.Okay;
                        }

                        payload = packetQ.Dequeue();
                        // Invalid data, abort
                        if (payload.Count < 2)
                        {
                            // If this happens then that means the file parser is broken.
                            Reporter.ReportMessage(
                                "Error flashing because of invalid payload, contact PTI and report this message");
                            status = ReturnCodes.OperationAborted;
                            break;
                        }

                        // Test for start of data block
                        if (payload[1] == (byte) RelianceCommands.FlashRequest ||
                            payload[1] == (byte) RelianceCommands.DataWriteRequest)
                        {
                            // We found the next block, break out of this loop and continue with
                            // transmission
                            break;
                        }
                    }
                }

                if (tracker.RetryCount <= tracker.RetryLimit)
                {
                    continue;
                }

                Reporter.ReportFailure("Too many retries. Aborting flash operation");
                tracker.Status = FlashState.Giveup;
                status = ReturnCodes.OperationAborted;

            } while (tracker.Status != FlashState.Giveup && tracker.Status != FlashState.Success);

#if DEBUG
            Reporter.ReportMessage(tracker.ToString());
#endif
            return status == ReturnCodes.Okay ? CheckChecksum(Write) : status;
        }

        /// <summary>
        /// Check checksum of target device
        /// </summary>
        /// <param name="write">Device transmit function</param>
        /// <returns>Return code</returns>
        private ReturnCodes CheckChecksum(Func<IPacket, IPacket> write)
        {
            // Get expected checksum
            var cmd = Port.Package(0x85, 0x11);
            var raw = write(cmd);
            var expectedCsum = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(raw);

            // Check actual checksum
            cmd = Port.Package(0x95, 0x11);
            raw = write(cmd);
            var actualCsum = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(raw);

            return expectedCsum == actualCsum ? ReturnCodes.Okay : ReturnCodes.FlashChecksumMismatch;
        }
    }

    internal enum FlashState
    {
        Next, Retry, SkipThisBlock, Success, Giveup
    }

    internal struct FlashTracker
    {
        public FlashState Status;
        public int TotalPacketsTx;
        public int TotalBytesTx;

        public int Timeouts;
        public int RetryCount;
        public readonly int RetryLimit;
        public readonly DateTime StartedAt;

        /// <summary>
        /// Total time between start and stop
        /// </summary>
        public TimeSpan Delta;

        private DateTime _mStoppedAt;

        /// <summary>
        /// Create a new flash tracker with retry limit
        /// </summary>
        /// <param name="retries"></param>
		public FlashTracker(int retries)
        {
            RetryLimit = retries;
            RetryCount = 0;
            Timeouts = 0;
            Status = FlashState.Next;
            TotalPacketsTx = 0;
            TotalBytesTx = 0;

            Delta = TimeSpan.FromSeconds(0);
            StartedAt = DateTime.Now;
            _mStoppedAt = DateTime.MinValue;

        }

        /// <summary>
        /// Stops the runtime clock
        /// </summary>
        public void Stop()
        {
            _mStoppedAt = DateTime.Now;
            Delta = _mStoppedAt - StartedAt;
        }

        /// <summary>
        /// Pretty-print self to log file. If Stop()
        /// has not been called, this function will call it for you.
        /// </summary>
		public override string ToString()
        {
            // Ensure we're stopped
            if (_mStoppedAt.Equals(DateTime.MinValue))
            {
                Stop();
            }

            var kbps = TotalBytesTx / Delta.TotalSeconds / 1000.0;

            var sb = new StringBuilder();
            sb.AppendLine("Flash update performance");
            sb.AppendFormat("\tTimeouts: {0}\n", Timeouts);
            sb.AppendFormat("\tTotal Retry: {0}\n", RetryCount);
            sb.AppendFormat("\tTotal Bytes: {0}\n", TotalBytesTx);
            sb.AppendFormat("\tTotal Packets: {0}\n", TotalPacketsTx);
            sb.AppendFormat("\tTotal Time: {0}ms\n", Delta.TotalMilliseconds);
            sb.AppendFormat("\tThroughput: {0:F3}KBps\n", kbps);

            return sb.ToString();
        }
    }
}