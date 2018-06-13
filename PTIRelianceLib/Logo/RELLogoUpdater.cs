#region Header
// RELLogoUpdater.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 12:47 PM
#endregion

namespace PTIRelianceLib.Logo
{
    using System;
    using System.Linq;
    using Flash;
    using Transport;

    internal class RELLogoUpdater : IFlashUpdater
    {
        private readonly RELLogoHeader _mLogoHeader;

        private readonly IPort<IPacket> _mPort;

        /// <summary>
        /// Constructs a new flasher
        /// </summary>
        /// <param name="port">Port to write to</param>
        /// <param name="_mLogoHeader">Data to write</param>
        /// <exception cref="ArgumentNullException">Thrown if port or fileToFlash are null</exception>
        public RELLogoUpdater(IPort<IPacket> port, RELLogoHeader header)
        {
            _mPort = port ?? throw new ArgumentNullException(nameof(port));
            _mLogoHeader = header ?? throw new ArgumentNullException(nameof(header));
        }

        /// <summary>
        /// Gets or Sets reporter
        /// </summary>
        public IProgressMonitor Reporter { get; set; }

        /// <inheritdoc />
        public ReturnCodes ExecuteUpdate()
        {
            var logoParser = new RELLogoParser();
            var parsed = logoParser.Parse(_mLogoHeader.StartAddr, _mLogoHeader.LogoData);
            if (parsed == null)
            {
                return ReturnCodes.FlashFileInvalid;
            }

            // Repack data into packets
            var packets = parsed.Select(p => _mPort.Package(p)).ToList();

            var updater = new RELStreamer(Reporter, _mPort);
            return updater.StreamFlashData(packets);
        }
    }
}