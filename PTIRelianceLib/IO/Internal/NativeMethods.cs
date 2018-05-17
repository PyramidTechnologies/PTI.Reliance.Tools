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
            /// Pointer to the next device
            /// </summary>
            public IntPtr Next;
        };

        [DllImport("hidapi", EntryPoint = "hid_error")]
        public static extern IntPtr HidError(IntPtr device);

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_init")]
        internal static extern int HidInit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_exit")]
        internal static extern int HidExit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_enumerate")]
        internal static extern IntPtr HidEnumerate(ushort vid, ushort pid);

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_free_enumeration")]
        internal static extern void HidFreeEnumerate(IntPtr devices);

        [DllImport("hidapi", CharSet = CharSet.Ansi, EntryPoint = "hid_open")]
        internal static extern IntPtr HidOpen(ushort vid, ushort pid, string serialNumber);

        [DllImport("hidapi", CharSet = CharSet.Ansi, EntryPoint = "hid_open_path")]
        internal static extern IntPtr HidOpen(string devicePath);

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_close")]
        internal static extern void HidClose(IntPtr device);

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_read")]
        public static extern int HidRead(IntPtr device, byte[] data, UIntPtr length);

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_write")]
        public static extern int HidWrite(IntPtr device, byte[] data, UIntPtr length);
    }
}
