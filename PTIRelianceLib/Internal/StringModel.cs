#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:11 AM
#endregion

namespace PTIRelianceLib
{
    using PTIRelianceLib.Transport;

    /// <summary>
    /// Useful for handling parseable data streams that are simple strings and not complex
    /// data structures.
    /// </summary>
    internal class StringModel : IParseable
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new StringModel with Data set to empty string
        /// </summary>
        public StringModel() : this(string.Empty) { }

        /// <summary>
        /// Creates a new StringModel with the specified data
        /// </summary>
        /// <param name="data"></param>
        public StringModel(string data)
        {
            Data = data;
            IsValid = true;
        }

        /// <summary>
        /// Extracted string data (printable ASCII only)
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Returns true if the raw source data had a correct packaging.
        /// </summary>
        public bool IsValid { get; protected set; }

        public override string ToString()
        {
            return Data;
        }

        public string ToJson()
        {
            return Data;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Stub class to differentiate hex string and ascii strings. The
    /// implementation actually resides in the parser strategy.
    /// </summary>
    class HexStringModel : StringModel
    {
        public HexStringModel() : this(string.Empty) { }

        public HexStringModel(string s) : base(s) { }
    }


    /// <summary>
    /// Implementation containing constant string "Invalid" that satifies HexStringModel contract
    /// </summary>
    class InvalidHexStringModel : HexStringModel
    { }

    /// <inheritdoc />
    /// <summary>
    /// Implementation containing constant string "Invalid" that satifies StringModel contract
    /// </summary>
    internal class InvalidStringModel : StringModel
    {
        public InvalidStringModel()
        {
            Data = "Invalid";
            IsValid = false;
        }
    }

    internal class RawASCIIParser : IParseAs<StringModel>
    {
        /// <inheritdoc />
        /// <summary>
        /// Parse response portion of packet as raw ASCII.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public StringModel Parse(IPacket packet)
        {

            if (packet == null)
            {
                return new InvalidStringModel();
            }

            if (packet.IsPackaged)
            {
                packet = packet.ExtractPayload();
            }

            var data = packet.GetBytes();
            var str = data.GetPrintableString();

            return new StringModel(str);

        }
    }

    internal class HexStringParser : IParseAs<HexStringModel>
    {
        /// <summary>
        /// Parse response portion of packet as a hex string.
        /// [0x12, 0x45, 0xC0] => "1245C0"
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public HexStringModel Parse(IPacket packet)
        {

            if (packet == null)
            {
                return new InvalidHexStringModel();
            }

            if (packet.IsPackaged)
            {
                packet = packet.ExtractPayload();
            }

            var data = packet.GetBytes();
            return new HexStringModel(data.ByteArrayToHexString(delimeter: ""));

        }
    }
}
