#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 15-05-2018
// 3:06 PM
#endregion

using System.Collections.Generic;
using System.Linq;

namespace PTIRelianceLib
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        /// <summary>
        /// Opaque HID device handle
        /// </summary>
        internal struct HidDevice
        {
            /// <summary>
            /// Handle to device
            /// </summary>
            internal IntPtr Handle;

            /// <summary>
            /// Returns true if this is a valid handle
            /// </summary>
            public bool IsValid => Handle != IntPtr.Zero;

            /// <summary>
            /// Returns an empty (invalid) device handle
            /// </summary>
            /// <returns>Device handle</returns>
            public static HidDevice Invalid()
            {
                return new HidDevice
                {
                    Handle = IntPtr.Zero
                };
            }
        }

        /// <summary>
        /// Interal device info enumeration
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct PrivateHidDeviceInfo
        {
            public readonly string Path;
            public readonly ushort VendorId;
            public readonly ushort ProductId;
            public readonly string SerialNumber;
            public readonly ushort ReleaseNumber;
            public readonly string ManufacturerString;
            public readonly string ProductString;
            public readonly ushort UsagePage;
            public readonly ushort Usage;
            public readonly int InterfaceNumber;
            public readonly IntPtr Next;
        }

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
        }


        [DllImport("hidapi", EntryPoint = "hid_error")]
        private static extern IntPtr _HidError(IntPtr device);
        /// <summary>
        /// Get a string describing the last error which occurred.
        /// </summary>
        /// <param name="device">A device handle returned from HidOpenPath().</param>
        /// <returns>This function returns a string containing the last error
        /// which occurred or NULL if none has occurred.</returns>
        public static string HidError(HidDevice device)
        {
            return !device.IsValid ? "PTIRelianceLib: Invalid device handle" : Marshal.PtrToStringUni(_HidError(device.Handle));
        }

        /// <summary>
        /// Initialize the HIDAPI library.
        /// 
        /// This function initializes the HIDAPI library. Calling it is not
        /// strictly necessary, as it will be called automatically by
        /// HidEnumerate() and HidOpenPath() functions if it is
        /// needed.  This function should be called at the beginning of
        /// execution however, if there is a chance of HIDAPI handles
        /// being opened by different threads simultaneously.
        /// </summary>
        /// <returns>This function returns 0 on success and -1 on error</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_init")]
        internal static extern int HidInit();

        /// <summary>
        /// Finalize the HIDAPI library.
        /// 
        /// This function frees all of the static data associated with
        /// HIDAPI. It should be called at the end of execution to avoid
        /// memory leaks.
        /// </summary>
        /// <returns>This function returns 0 on success and -1 on error.</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_exit")]
        internal static extern int HidExit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_enumerate")]
        internal static extern IntPtr _HidEnumerate(ushort vid, ushort pid);
        /// <summary>
        /// Enumerate the HID Devices.
        /// 
        /// This function returns a linked list of all the HID devices
        /// attached to the system which match vid and pid.
        /// </summary>
        /// <param name="vid">The Vendor ID (VID) of the types of device
        /// to open. Set to 0 to match any vendor id.</param>
        /// <param name="pid">The Product ID (PID) of the types of
        /// device to open. Set to zero to match and product id.</param>
        /// <returns>This function returns list of HidDeviceInfo, each containing
        /// information about the HID devices attached to the system,
        /// or an empty list case of failure.</returns>
        internal static IEnumerable<HidDeviceInfo> HidEnumerate(ushort vid, ushort pid)
        {
            var enumerated = _HidEnumerate(vid, pid);
            if (enumerated == IntPtr.Zero)
            {
                return Enumerable.Empty<HidDeviceInfo>();
            }

            var result = new List<HidDeviceInfo>();

            var current = enumerated;
            while (current != IntPtr.Zero)
            {
                var devinfo = (PrivateHidDeviceInfo)Marshal.PtrToStructure(enumerated,
                    typeof(PrivateHidDeviceInfo));

                result.Add(new HidDeviceInfo
                {
                    Path = devinfo.Path,
                    VendorId = devinfo.VendorId,
                    ProductId = devinfo.ProductId,
                    SerialNumber = devinfo.SerialNumber,
                    ReleaseNumber = devinfo.ReleaseNumber,
                    ManufacturerString = devinfo.ManufacturerString,
                    ProductString = devinfo.ProductString,
                    UsagePage = devinfo.UsagePage,
                    Usage = devinfo.Usage,
                    InterfaceNumber = devinfo.InterfaceNumber
                });

                current = devinfo.Next;
            }

            _HidFreeEnumerate(enumerated);
            return result;
        }

        /// <summary>
        /// Free an enumeration Linked List
        /// 
        /// This function frees a linked list created by HidEnumerate().
        /// </summary>
        /// <param name="devices">devs Pointer to a list of HidDeviceInfo returned from
        /// HidEnumerate()</param>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_free_enumeration")]
        private static extern void _HidFreeEnumerate(IntPtr devices);


        [DllImport("hidapi", CharSet = CharSet.Ansi, EntryPoint = "hid_open_path")]
        private static extern IntPtr _HidOpenPath(string devicePath);
        /// <summary>
        /// Open a HID device by its path name.
        /// 
        /// The path name be determined by calling HidEnumerate(), or a
        /// platform-specific path name can be used (eg: /dev/hidraw0 on
        /// Linux).
        /// </summary>
        /// <param name="devicePath">The path name of the device to open</param>
        /// <returns>This function returns a pointer to a #hid_device object on
        /// success or NULL on failure.</returns>
        internal static HidDevice HidOpenPath(string devicePath)
        {
            return new HidDevice
            {
                Handle = _HidOpenPath(devicePath)
            };
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_close")]
        private static extern void _HidClose(IntPtr device);
        /// <summary>
        /// Close a HID device.
        /// </summary>
        /// <param name="device">A device handle returned from hid_open().</param>
        internal static void HidClose(HidDevice device)
        {
            _HidClose(device.Handle);            
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_read_timeout")]
        private static extern int _HidRead(IntPtr device, byte[] data, UIntPtr length, int timeout);
        /// <summary>
        /// Read an Input report from a HID device with timeout.
        /// 
        /// Input reports are returned
        /// to the host through the INTERRUPT IN endpoint. The first byte will
        /// contain the Report number if the device uses numbered reports.
        /// </summary>
        /// <param name="device">A device handle returned from HidOpenPath().</param>
        /// <param name="data">A buffer to put the read data into.</param>
        /// <param name="length">The number of bytes to read. For devices with
        /// multiple reports, make sure to read an extra byte for
        /// the report number.</param>
        /// <param name="timeout">timeout in milliseconds or -1 for blocking wait.</param>
        /// <returns>This function returns the actual number of bytes read and
        /// -1 on error. If no packet was available to be read within
        /// the timeout period, this function returns 0.</returns>
        internal static int HidRead(HidDevice device, byte[] data, UIntPtr length, int timeout)
        {
            return _HidRead(device.Handle, data, length, timeout);
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_write")]
        private static extern int _HidWrite(IntPtr device, byte[] data, UIntPtr length);
        /// <summary>
        /// Write an Output report to a HID device.
        /// 
        /// The first byte of @p data[] must contain the Report ID. For
        /// devices which only support a single report, this must be set
        /// to 0x0. The remaining bytes contain the report data. Since
        /// the Report ID is mandatory, calls to HidWrite() will always
        /// contain one more byte than the report contains. For example,
        /// if a hid report is 16 bytes long, 17 bytes must be passed to
        /// HidWrite(), the Report ID (or 0x0, for devices with a
        /// single report), followed by the report data (16 bytes). In
        /// this example, the length passed in would be 17.
        /// 
        /// HidWrite() will send the data on the first OUT endpoint, if
        /// one exists. If it does not, it will send the data through
        /// the Control Endpoint (Endpoint 0).
        /// 
        /// </summary>
        /// <param name="device"> A device handle returned from HidOpenPath()</param>
        /// <param name="data">The data to send, including the report number as
        /// the first byte.</param>
        /// <param name="length">The length in bytes of the data to send.</param>
        /// <returns>This function returns the actual number of bytes written and
        /// -1 on error.</returns>
        internal static int HidWrite(HidDevice device, byte[] data, UIntPtr length)
        {
            return _HidWrite(device.Handle, data, length);
        }
    }
}
