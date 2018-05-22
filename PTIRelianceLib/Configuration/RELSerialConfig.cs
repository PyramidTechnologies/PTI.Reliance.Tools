#region Header
// RELSerialParams.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:23 AM
#endregion


namespace PTIRelianceLib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Transport;

    /// <inheritdoc cref="IParseable" />
    /// <summary>
    /// Reliance RS-232 serial configuration (DB-9 only)
    /// </summary>
    internal class RELSerialConfig : IParseable
    {
        /// <summary>
        /// Get or Set baud rate
        /// </summary>
        internal int BaudRate { get; set; }

        /// <summary>
        /// Get or Set databits
        /// </summary>
        internal byte Databits { get; set; }

        /// <summary>
        /// Get or Set parity
        /// </summary>
        internal SerialParity Parity { get; set; }

        /// <summary>
        /// Get or Set stopbits
        /// </summary>
        internal SerialStopbits Stopbits { get; set; }

        /// <summary>
        /// Get or Set handshake mode
        /// </summary>
        internal SerialHandshake Handshake { get; set; }


        /// <summary>
        /// Override returns object summary of configuration as
        /// ...
        /// Parameter: value, ...
        /// ...
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"Baud: {BaudRate}, Databits: {Databits}, Parity: {Parity.GetEnumName()}, Stopbits: {Stopbits.GetEnumName()}, Handshake: {Handshake.GetEnumName()}";

        }

        public byte[] Serialize()
        {
            var result = new List<byte>();
            var baud = BaudRate.ToBytesBE();
            result.AddRange(baud);
            result.Add(Databits);
            result.Add((byte)Parity);
            result.Add((byte)Stopbits);
            result.Add((byte)Handshake);
            return result.ToArray();
        }
    }

    internal class RELSerialConfigParser : BaseModelParser<RELSerialConfig>
    {

        public override RELSerialConfig Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            var data = packet.GetBytes();
            if (data.Length != 8)
            {
                return null;
            }


            var builder = new SerialConfigBuilder();


            // baud is 1st 4 bytes
            var temp = new byte[4];
            Array.Copy(data, temp, 4);
            var baud = temp.ToUintBE();

            builder.SetBaudRate((int)baud);
            builder.SetDataBits(data[4]);
            builder.SetParity((SerialParity)data[5]);
            builder.SetStopBits((SerialStopbits)data[6]);
            builder.SetFlowControl((SerialHandshake)data[7]);

            return builder.Build();
        }
    }

    /// <summary>
    /// SerialConfigBuilder constructs an instance of SerialConfig.
    /// </summary>
    internal class SerialConfigBuilder
    {

        #region Default Serial Parameters
        private const int DefaultBaudRate = 19200;
        private const byte DefaultDataBits = 8;
        private const SerialParity DefaultParity = SerialParity.None;
        private const SerialStopbits DefaultStopBits = SerialStopbits.One;
        private const SerialHandshake DefaultFlowControl = SerialHandshake.None;
        #endregion

        #region Properties
        internal int BaudRate { get; set; }
        internal byte Databits { get; set; }
        internal SerialParity Parity { get; set; }
        internal SerialStopbits StopBits { get; set; }
        internal SerialHandshake FlowControl { get; set; }
        #endregion

        /// <summary>
        /// Returns the default serial configuration for Reliance thermal printer
        /// 19200, 8N1 no flow control
        /// </summary>
        /// <returns>SerialConfig</returns>
        public static RELSerialConfig Default => new RELSerialConfig()
        {
            BaudRate = DefaultBaudRate,
            Databits = DefaultDataBits,
            Parity = DefaultParity,
            Stopbits = DefaultStopBits,
            Handshake = DefaultFlowControl
        };

        /// <summary>
        /// Create a new builder instance
        /// </summary>
        public SerialConfigBuilder()
        {
            BaudRate = DefaultBaudRate;
            Databits = DefaultDataBits;
            Parity = DefaultParity;
            StopBits = DefaultStopBits;
            FlowControl = DefaultFlowControl;
        }

        /// <summary>
        /// Sets baud rate to use for configuration
        /// </summary>
        /// <param name="baud">int baud rate</param>
        /// <returns>SerialConfigBuilder</returns>
        public SerialConfigBuilder SetBaudRate(int baud)
        {
            BaudRate = baud;
            return this;
        }

        /// <summary>
        /// Sets the number of databuts to use for configuration
        /// </summary>
        /// <param name="databits">int</param>
        /// <returns>SerialConfigBuilder</returns>
        public SerialConfigBuilder SetDataBits(byte databits)
        {
            Databits = databits;
            return this;
        }

        /// <summary>
        /// Sets the parity to use for configuration
        /// </summary>
        /// <param name="parity">SerialParity</param>
        /// <returns>SerialConfigBuilder</returns>
        public SerialConfigBuilder SetParity(SerialParity parity)
        {
            Parity = parity;
            return this;
        }

        /// <summary>
        /// Sets the stopbits for configuration
        /// </summary>
        /// <param name="stopbits">SerialStopBits</param>
        /// <returns>SerialConfigBuilder</returns>
        public SerialConfigBuilder SetStopBits(SerialStopbits stopbits)
        {
            StopBits = stopbits;
            return this;
        }

        /// <summary>
        /// Sets the flow control for configuration
        /// </summary>
        /// <param name="flow">SerialFlowControl</param>
        /// <returns>SerialConfigBuilder</returns>
        public SerialConfigBuilder SetFlowControl(SerialHandshake flow)
        {
            FlowControl = flow;
            return this;
        }

        /// <summary>
        /// Constructs a new serial configuration using current builder state. This
        /// may be called multiple times for multiple configurations. 
        /// </summary>
        /// <returns>SerialConfig</returns>
        public RELSerialConfig Build()
        {
            return new RELSerialConfig()
            {
                BaudRate = BaudRate,
                Databits = Databits,
                Parity = Parity,
                Stopbits = StopBits,
                Handshake = FlowControl
            };
        }
    }

    /// <summary>
    /// Supported serial parity modes on Reliance
    /// </summary>
    internal enum SerialParity
    {
        /// <summary>
        /// No parity
        /// </summary>
        [EnumMember(Value = "None")]
        None,
        /// <summary>
        /// Odd parity
        /// </summary>
        [EnumMember(Value = "Odd")]
        Odd,
        /// <summary>
        /// Event parity
        /// </summary>
        [EnumMember(Value = "Even")]
        Even,
        //[EnumMember(Value = "Mark")]
        //Mark,
        //[EnumMember(Value = "Space")]
        //Space,
    }

    /// <summary>
    /// Supported serial stop bits
    /// </summary>
    internal enum SerialStopbits
    {
        /// <summary>
        /// 1, 1 stop bit, ah ah ah
        /// </summary>
        [EnumMember(Value = "1")]
        One,
        //[EnumMember(Value = "1.5")]
        //OnePointFive,
        //[EnumMember(Value = "2")]
        //Two,
    };

    /// <summary>
    /// Supported handshake modes
    /// </summary>
    internal enum SerialHandshake
    {
        /// <summary>
        /// No handshake
        /// </summary>
        [EnumMember(Value = "None")]
        None,
        /// <summary>
        /// RTS/CTS pin control
        /// </summary>
        [EnumMember(Value = "RTS/CTS")]
        RtsCts,
        /// <summary>
        /// DTS/DSR pin control
        /// </summary>
        [EnumMember(Value = "DTR/DSR")]
        DtrDsr,
        /// <summary>
        /// Xon/Xoff software control
        /// </summary>
        [EnumMember(Value = "Xon/Xoff")]
        XonXoff,
    };

    /// <summary>
    /// XonXoff configuration
    /// </summary>
    /// <remarks>Requires firmware 1.17+</remarks>
    internal class XonXoffConfig : IParseable
    {
        /// <summary>
        /// Default Xon byte is DC1
        /// </summary>
        public const byte DefaultXon = 17;

        /// <summary>
        /// Default Xoff byte is DC3
        /// </summary>
        public const byte DefaultXoff = 19;

        /// <summary>
        /// Gets or sets Xon value
        /// </summary>
        public byte Xon { get; set; }

        /// <summary>
        /// Gets of sets Xoff value
        /// </summary>
        public byte Xoff { get; set; }

        /// <summary>
        /// Returns default XonXoff configuration
        /// </summary>
        /// <returns></returns>
        public static XonXoffConfig Default()
        {
            return new XonXoffConfig()
            {
                Xon = DefaultXon,
                Xoff = DefaultXoff,
            };
        }

        public byte[] Serialize()
        {
            var buff = new List<byte> {Xon, Xoff};
            return buff.ToArray();
        }
    }

    internal class XonXoffConfigParser : BaseModelParser<XonXoffConfig>
    {
        public override XonXoffConfig Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            if (packet.Count < 2)
            {
                return null;
            }

            return new XonXoffConfig
            {
                Xon = packet[0],
                Xoff = packet[1]
            };
        }
    }
}