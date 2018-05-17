#region Header
// HidDeviceConfig
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 1:47 PM
#endregion

namespace PTIRelianceLib.IO.Internal
{
    internal struct HidDeviceConfig
    {
        public ushort VendorId { get; set; }

        public ushort ProductId { get; set; }

        public byte InReportId { get; set; }

        public byte OutReportId { get; set; }

        public int InReportLength { get; set; }

        public int OutReportLength { get; set; }
    }
}