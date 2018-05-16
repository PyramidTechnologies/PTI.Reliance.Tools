#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 15-05-2018
// 3:06 PM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HidDeviceInfo
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
            /// Pointer to the next device
            /// </summary>
            public IntPtr Next;
        };

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_init")]
        public static extern int HidInit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_exit")]
        public static extern int HidExit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_enumerate")]
        public static extern IntPtr HidEnumerate(ushort vid, ushort pid);
    }
}
