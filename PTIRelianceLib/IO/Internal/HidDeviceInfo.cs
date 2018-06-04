#region Header
// HidDeviceInfo.cs
// PTIRelianceLib
// Cory Todd
// 21-05-2018
// 11:49 AM
#endregion

namespace PTIRelianceLib.IO.Internal
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Public device enumeration
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HidDeviceInfo
    {
        /// <summary>
        /// Platform-specific device path
        /// </summary>
        public string Path;
        /// <summary>
        /// Device Vendor ID 
        /// </summary>
        public ushort VendorId;
        /// <summary>
        /// Device Product ID
        /// </summary>
        public ushort ProductId;
        /// <summary>
        /// Device serial number
        /// </summary>
        public string SerialNumber;
        /// <summary>
        /// Device Release Number in binary-coded decimal,
        /// also known as Device Version Number
        /// </summary>
        public ushort ReleaseNumber;
        /// <summary>
        /// Manufacturer String
        /// </summary>
        public string ManufacturerString;
        /// <summary>
        /// Product string
        /// </summary>
        public string ProductString;
        /// <summary>
        /// Usage Page for this Device/Interface (Windows/Mac only)
        /// </summary>
        public ushort UsagePage;
        /// <summary>
        /// Usage for this Device/Interface (Windows/Mac only)
        /// </summary>
        public ushort Usage;
        /// <summary>
        /// The USB interface which this logical device represents.
        /// Valid on both Linux implementations in all cases, and 
        /// valid on the Windows implementation only if the device 
        /// contains more than one interface
        /// </summary>
        public int InterfaceNumber;

        /// <summary>
        /// Returns device info in form:
        /// VID:PID|Mfg. String|Prod. String|Path
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{VendorId:X4}:{ProductId:X4}|{ManufacturerString}|{ProductString}|{Path}";
        }
    }
}