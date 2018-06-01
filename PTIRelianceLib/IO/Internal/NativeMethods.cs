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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using IO.Internal;

    internal class NativeMethods : INativeMethods
    {
        /// <summary>
        /// Interal device info enumeration
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct PrivateHidDeviceInfo
        {
            public readonly string Path;
            public readonly ushort VendorId;
            public readonly ushort ProductId;
            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly string SerialNumber;
            public readonly ushort ReleaseNumber;
            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly string ManufacturerString;
            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly string ProductString;
            public readonly ushort UsagePage;
            public readonly ushort Usage;
            public readonly int InterfaceNumber;
            public readonly IntPtr Next;
        }

        [DllImport("hidapi", EntryPoint = "hid_error")]
        private static extern IntPtr _HidError(IntPtr device);
        /// <inheritdoc />
        public string Error(HidDevice device)
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
        internal static extern int _HidInit();

        /// <inheritdoc />
        public int Init()
        {
            return _HidInit();
        }

        /// <summary>
        /// Finalize the HIDAPI library.
        /// 
        /// This function frees all of the static data associated with
        /// HIDAPI. It should be called at the end of execution to avoid
        /// memory leaks.
        /// </summary>
        /// <returns>This function returns 0 on success and -1 on error.</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_exit")]
        private static extern int _HidExit();

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_enumerate")]
        internal static extern IntPtr _HidEnumerate(ushort vid, ushort pid);
        public IEnumerable<HidDeviceInfo> Enumerate(ushort vid, ushort pid)
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

        /// <inheritdoc />
        public HidDevice OpenPath(string devicePath)
        {
            var handle = _HidOpenPath(devicePath);
            return new HidDevice(handle);
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_close")]
        internal static extern void _HidClose(IntPtr device);

        /// <inheritdoc />
        public void Close(HidDevice device)
        {
            try
            {
                _HidClose(device.Handle);
            }
            catch (SEHException ex)
            {
                throw new PTIException("Failed to close HID handle: {0}", ex.Message);
            }
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_read_timeout")]
        private static extern int _HidRead(IntPtr device, byte[] data, UIntPtr length, int timeout);

        /// <inheritdoc />
        public int Read(HidDevice device, byte[] data, UIntPtr length, int timeout)
        {
            return _HidRead(device.Handle, data, length, timeout);
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_write")]
        private static extern int _HidWrite(IntPtr device, byte[] data, UIntPtr length);

        /// <inheritdoc />
        public int Write(HidDevice device, byte[] data, UIntPtr length)
        {
            return _HidWrite(device.Handle, data, length);
        }

        public void Dispose()
        {
            _HidExit();
        }
    }
}
