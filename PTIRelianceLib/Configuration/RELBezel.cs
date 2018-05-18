#region Header
// RELBezelModes.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:22 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Transport;

    /// <summary>
    /// This model represent the bezel configuration options
    /// and provides serialization and deserialization utilities.
    /// This is designed for the 1.20 Set Bezel Operation command set 
    /// (NOT set bezel override)
    /// </summary>
    internal class RELBezel : IParseable
    {

        /// <summary>
        /// When should the bezel configuration be applied
        /// </summary>
        public RELBezelModes BezelMode { get; set; }

        /// <summary>
        /// Controls brightness of bezel LEDs as a percentage.
        /// Valid range is 0-100
        /// </summary>
        public byte DutyCycle { get; set; }

        /// <summary>
        /// Number of milliseconds to be on then off
        /// </summary>
        public uint FlashInterval { get; set; }

        /// <summary>
        /// Returns the default values as they are encoded in the firwmare
        /// </summary>
        /// <returns></returns>
        public static IList<RELBezel> GetDefaults()
        {
            return new List<RELBezel>()
            {
                new RELBezel()
                {
                    BezelMode = RELBezelModes.PrinterIdle,
                    DutyCycle = 100,
                    FlashInterval = 0,
                },
                new RELBezel()
                {
                    BezelMode = RELBezelModes.TicketPrinting,
                    DutyCycle = 100,
                    FlashInterval = 0,
                },
                new RELBezel()
                {
                    BezelMode = RELBezelModes.TicketPresented,
                    DutyCycle = 100,
                    FlashInterval = 500,
                },
                new RELBezel()
                {
                    BezelMode = RELBezelModes.TicketEjecting,
                    DutyCycle = 100,
                    FlashInterval = 200,
                },
            };
        }

        public byte[] Serialize()
        {
            var buff = new List<byte> {(byte) BezelMode, DutyCycle};
            buff.AddRange(FlashInterval.ToBytesBE());
            return buff.ToArray();
        }
    }

    /// <summary>
    /// Parse that can convert an IPacket into a RelianceBezel model
    /// </summary>
    internal class RELBezelParser : BaseModelParser<RELBezel>
    {
        /// <summary>
        /// Parse response portion of packet into a RelianceBezel
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override RELBezel Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            var payload = packet.GetBytes();
            if (payload.Length != 5)
            {
                return null;
            }

            try
            {
                // In 1.19 and older, only a subset of this command was supported.
                // Just in case someone decides to misuse this, don't explode
                var result = new RELBezel();

                using (var stream = new MemoryStream(payload))
                using (var reader = new BinaryReader(stream))
                {
                    var duty = reader.ReadByte();
                    result.BezelMode = RELBezelModes.PrinterIdle;           // Command is unfortunately context sensitive so caller must set this
                    result.DutyCycle = (byte)(Math.Min((int)duty, 100));    // 0-100, byte is unsigned so just enforce max
                    result.FlashInterval = reader.ReadUInt32();
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Bezel operation states. The bezel configuration
    /// will only be applied in when the printer is 
    /// in one of these states.
    /// </summary>
    internal enum RELBezelModes
    {
        /// <summary>
        /// Printer is idle, no paper movement and not ticket at presenter
        /// </summary>
        [EnumMember(Value = "Printer Idle")]
        PrinterIdle = 0,
        /// <summary>
        /// Ticket is being moved through system
        /// </summary>
        [EnumMember(Value = "Ticket Printing")]
        TicketPrinting = 1,
        /// <summary>
        /// Ticket has been moved and is awaiting removal by customer
        /// </summary>
        [EnumMember(Value = "Ticket Presented")]
        TicketPresented = 2,
        /// <summary>
        /// Ticket removal detected or triggered by timeout
        /// </summary>
        [EnumMember(Value = "Ticket Ejecting")]
        TicketEjecting = 3,
    }
}