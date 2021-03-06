﻿#region Header
// RELConfig.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:21 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal sealed class RELConfig
    {
        /// <summary>
        /// Get or Set configuration version number ( 0 lt x lt FF)
        /// </summary>
        [DefaultValue(1)]
        public byte Version { get; set; }

        /// <summary>
        /// Get or Set configration revision character (0 lt x lt FF)
        /// </summary>
        [DefaultValue('A')]
        public char Revision { get; set; }

        /// <summary>
        /// Get or Set baud rate
        /// </summary>
        [DefaultValue(19200)]
        public int BaudRate { get; set; }

        /// <summary>
        /// Get or Set serial databits
        /// </summary>
        [DefaultValue(8)]
        public int Databits { get; set; }

        /// <summary>
        /// Get or Set serial parity
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(SerialParity.None)]
        public SerialParity Parity { get; set; }

        /// <summary>
        /// Get or Set serial stopbits
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(SerialStopbits.One)]
        public SerialStopbits Stopbits { get; set; }

        /// <summary>
        /// Get or Set serial flow control (Handshake)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(SerialHandshake.None)]
        public SerialHandshake Handshake { get; set; }

        /// <summary>
        /// Get or Set print quality
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(ReliancePrintQuality.Normal)]
        public ReliancePrintQuality Quality { get; set; }

        /// <summary>
        /// Get or Set retraction feature enabled
        /// </summary>
        [DefaultValue(false)]
        public bool RetractEnabled { get; set; }

        /// <summary>
        /// Get or Set ejector mode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(RelianceEjectorMode.PresenterMode)]
        public RelianceEjectorMode Ejector { get; set; }

        /// <summary>
        /// Get or Set ticket timeout period
        /// </summary>
        [DefaultValue(5)]
        public int TicketTimeout { get; set; }

        /// <summary>
        /// Get or Set ticket timeout action
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(TicketTimeoutAction.Nothing)]
        public TicketTimeoutAction TicketTimeoutAction { get; set; }

        /// <summary>
        /// Get or Set new ticket action
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NewTicketAction.EjectTicket)]
        public NewTicketAction NewTicketAction { get; set; }

        /// <summary>
        /// Get or Set present length in mm
        /// </summary>
        [DefaultValue(48)]
        public int PresentLength { get; set; }

        /// <summary>
        /// Get or Set CRLF enabled feature
        /// </summary>
        [DefaultValue(false)]
        public bool CRLFEnabled { get; set; }

        /// <summary>
        /// Get or Set print density percentage
        /// </summary>
        [DefaultValue(100)]
        public int PrintDensity { get; set; }

        /// <summary>
        /// Get or Set autocut enabled feature
        /// </summary>
        [DefaultValue(false)]
        public bool AutocutEnabled { get; set; }

        /// <summary>
        /// Get or Set autocut timeout period
        /// </summary>
        [DefaultValue(1)]
        public int AutocutTimeout { get; set; }

        /// <summary>
        /// Get or Set paper width in mm
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(PaperSizes.Roll80Mm)]
        public PaperSizes PaperWidth { get; set; }

        /// <summary>
        /// Gets or Sets the scaling mode for fonts
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(RelianceScalarMode.Ideal)]
        public RelianceScalarMode FontScalingMode { get; set; }

        /// <summary>
        /// Get or Set font codepage id
        /// </summary>
        [DefaultValue(808)]
        public int DefaultCodepage { get; set; }

        /// <summary>
        /// Gets or Sets first codepage
        /// </summary>
        [DefaultValue(808)]
        public int Codepage1 { get; set; }

        /// <summary>
        /// Gets or Sets second codepage
        /// </summary>
        [DefaultValue(1252)]
        public int Codepage2 { get; set; }

        /// <summary>
        /// Gets or Sets third codepage
        /// </summary>
        [DefaultValue(0)]
        public int Codepage3 { get; set; }

        /// <summary>
        /// Gets or Sets fourth codepage
        /// </summary>
        [DefaultValue(0)]
        public int Codepage4 { get; set; }

        /// <summary>
        /// Get or Set font CPI size
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(RelianceFontSizes.A11_B15)]
        public RelianceFontSizes FontSize { get; set; }

        /// <summary>
        /// Get or Set font which ('A' or 'B')
        /// </summary>
        [DefaultValue('A')]
        public char FontWhich { get; set; }

        /// <summary>
        /// Get or Set virtual serial over USB enabled feature
        /// </summary>
        [DefaultValue(false)]
        public bool IsCDCEnabled { get; set; }

        /// <summary>
        /// Get or Set startup ticket enabled feature
        /// </summary>
        [DefaultValue(true)]
        public bool IsStartupTicketEnabled { get; set; }

        /// <summary>
        /// Get or Set motor current in mA
        /// </summary>
        [DefaultValue(400)]
        public int MotorCurrent { get; set; }

        /// <summary>
        /// Get or Set paper slack feature enabled
        /// </summary>
        [DefaultValue(false)]
        public bool IsPaperSlackEnabeld { get; set; }

        /// <summary>
        /// Get or Set unique usb serial number feture enabled
        /// Serialization format requires this name
        /// </summary>
        [DefaultValue(false)]
        public bool IsUniqueUSBSNEnabled { get; set; }

        /// <summary>
        /// Get or Set Xon control codes
        /// </summary>
        [DefaultValue(17)]
        public byte XonCode { get; set; }

        /// <summary>
        /// Get or Set Xoff control codes
        /// </summary>
        [DefaultValue(19)]
        public byte XoffCode { get; set; }

        /// <summary>
        /// Get or Set white glove flag. If set, this disables
        /// the files from being used as a configuration.
        /// </summary>
        [DefaultValue(false)]
        public bool IsWhiteglove { get; set; }

        /// <summary>
        /// Get or Set bezel duty cycle in idle state
        /// </summary>
        [DefaultValue(100)]
        public byte BezelIdleDutyCycle { get; set; }

        /// <summary>
        /// Get or Set flash interval in idle state
        /// </summary>
        [DefaultValue(0)]
        public uint BezelIdleInterval { get; set; }

        /// <summary>
        /// Get or Set bezel duty cycle in printing state
        /// </summary>
        [DefaultValue(100)]
        public byte BezelPrintingDutyCycle { get; set; }

        /// <summary>
        /// Get or Set flash interval in printing state
        /// </summary>
        [DefaultValue(0)]
        public uint BezelPrintingInterval { get; set; }

        /// <summary>
        /// Get or Set bezel duty cycle in presented state
        /// </summary>
        [DefaultValue(100)]
        public byte BezelPresentedDutyCycle { get; set; }

        /// <summary>
        /// Get or Set flash interval in presented state
        /// </summary>
        [DefaultValue(500)]
        public uint BezelPresentedInterval { get; set; }

        /// <summary>
        /// Get or Set bezel duty cycle in ejecting state
        /// </summary>
        [DefaultValue(100)]
        public byte BezelEjectingDutyCycle { get; set; }

        /// <summary>
        /// Get or Set flash interval in ejecting state
        /// </summary>
        [DefaultValue(200)]
        public uint BezelEjectingInterval { get; set; }
        
        /// <summary>
        /// Get or Set option for handling an invisible ticket. If
        /// the printer's sensors cannot see a ticket at presentation time and
        /// this value is set to false, the ticket is assumed taken. If this
        /// value is set to true, the ticket is assumed present
        /// </summary>
        [DefaultValue(false)]
        public bool InvisibleTicketIsPresent { get; set; }
        
        /// <summary>
        /// Get or Set count of lines between lines of text.
        /// Lines start and end from baseline to top line.
        /// </summary>
        [DefaultValue(35)]
        public byte DefaultLineSpacing { get; set; }
        
        /// <summary>
        /// Get or Set count 2D barcode quiet zone size factor
        /// Smaller equates to less whitespace above and below
        /// the barcode.
        /// </summary>
        [DefaultValue(4)]
        public byte QuietZoneSpacing { get; set; }
        
        /// <summary>
        /// Allows variable length print pages without having to
        /// specify true page size in Windows print API
        /// </summary>
        [DefaultValue(false)]
        public bool WhitespaceTruncationEnabled { get; set; }
        
        /// <summary>
        /// Get or Set whether or not the device is in Protected Mode.
        /// Blocks communication that could cause unexpected behavior.
        /// </summary>
        [DefaultValue(false)]
        public bool ProtectedMode { get; set; }

        /// <summary>
        /// Serializes this object to stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public void Save(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jsonWriter, this);
                jsonWriter.Flush();
            }            
        }

        /// <summary>
        /// Deserialized the provided UTF-8 input file. If there are any issues,
        /// <c>null</c> will be returned.
        /// </summary>
        /// <param name="file">Binary file containing configuration data</param>
        /// <returns>RELConfig or <c>null</c> on error</returns>
        /// <exception cref="PTIException">Raised if file cannot bre properly read or parsed</exception>
        public static RELConfig Load(BinaryFile file)
        {
            try
            {
                using (var stream = new MemoryStream(file.GetData()))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return JsonSerializer.Create(
                        new JsonSerializerSettings
                        {
                            DefaultValueHandling = DefaultValueHandling.Populate
                        }).Deserialize(reader, typeof(RELConfig)) as RELConfig;                        
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new PTIException("Failed to read config: {0}", ex.InnerException.Message);
                }
                throw new PTIException("Failed to read config: {0}", ex.Message);
            }
        }
    }
}
