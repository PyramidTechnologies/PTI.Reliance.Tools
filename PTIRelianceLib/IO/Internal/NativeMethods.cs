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
    using System.Text;
    using System.Threading;
    using IO.Internal;
    using Logging;

    /// <summary>
    /// NativeMethods is a globally and instance synchronized interface to the native HID library
    /// </summary>
    internal sealed class NativeMethods : INativeMethods
    {
        private static readonly ILog Log = LogProvider.For<NativeMethods>();
        private static bool _hidInitialized;
        private static readonly object GlobalLock = new object();
        private readonly object _mInstanceLock = new object();

        /// <summary>
        /// Internal device info enumeration
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct PrivateHidDeviceInfo
        {
            [MarshalAs(UnmanagedType.LPStr)]
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

        [DllImport("hidapi", EntryPoint = "hid_error", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _HidError(IntPtr device);
        /// <inheritdoc />
        public string Error(HidDevice device)
        {
            lock (GlobalLock)
            {
                return !device.IsValid
                    ? "PTIRelianceLib: Invalid device handle"
                    : Marshal.PtrToStringUni(_HidError(device.Handle));
            }
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
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_init", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int _HidInit();

        /// <inheritdoc />
        public int Init()
        {
            try
            {
                lock (GlobalLock)
                {
                    if (_hidInitialized)
                    {
                        return 0;
                    }

                    _hidInitialized = true;
                    return _HidInit();
                }
            }
            catch (DllNotFoundException)
            {
                throw new PTIException(
                    "Missing hidapi library or VC++ 140. This dll requires access to a copy of hidapi for your system.\n" +
                    "1) Verify that you have installed the Microsoft Redistributable C++ 2015 runtime.\n" +
                    "2) If you installed this package from Nuget, this is a bug on our end. File an bug report on Github: " +
                    " https://github.com/PyramidTechnologies/PTI.Reliance.Tools/issues \n\n" +
                    "If you manually added this dll, please copy the appropriate hidapi library from the runtimes folder" +
                    "from this source's repo to a directory on your path: " +
                    "https://github.com/PyramidTechnologies/PTI.Reliance.Tools/tree/master/PTIRelianceLib/runtimes \n\n");
            }
        }

        /// <summary>
        /// Finalize the HIDAPI library.
        ///
        /// This function frees all of the static data associated with
        /// HIDAPI. It should be called at the end of execution to avoid
        /// memory leaks.
        /// </summary>
        /// <returns>This function returns 0 on success and -1 on error.</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidExit();

        /// <summary>
        /// Clears state and shuts down HID stack
        /// </summary>
        /// <returns>0 on success, -1 on error</returns>
        private static int HidExit()
        {
            lock (GlobalLock)
            {
                return _HidExit();
            }
        }

        /// <summary>
        /// Enumerate the HID Devices.
        ///
        /// This function returns a linked list of all the HID devices
        /// attached to the system which match vendor_id and product_id.
        /// If vendor_id is set to 0 then any vendor matches.
        /// If product_id is set to 0 then any product matches.
        /// If vendor_id and product_id are both set to 0, then
        /// all HID devices will be returned.
        ///
        /// This function returns a pointer to a linked list of type
        /// struct #hid_device, containing information about the HID devices
        /// attached to the system, or NULL in the case of failure. Free
        /// this linked list by calling hid_free_enumeration().
        /// </summary>
        /// <param name="vid">The Vendor ID (VID) of the types of device</param>
        /// <param name="pid">The Product ID (PID) of the types of device to open.</param>
        /// <returns></returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_enumerate", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr _HidEnumerate(ushort vid, ushort pid);

        /// <inheritdoc />
        public IEnumerable<HidDeviceInfo> Enumerate(ushort vid, ushort pid)
        {
            lock (GlobalLock)
            {
                if (Init() != 0)
                {
                    Log.Error("HID initialization failed");
                    return Enumerable.Empty<HidDeviceInfo>();
                }

                var enumerated = _HidEnumerate(vid, pid);
                if (enumerated == IntPtr.Zero)
                {
                    Log.Warn("No HID devices found during enumeration");

                    if (!Library.Options.HidFlushStructuresOnEnumError)
                    {
                        return Enumerable.Empty<HidDeviceInfo>();
                    }

                    Log.Info("flushing HID structures...");
                    if (HidExit() != 0)
                    {
                        Log.Error("Failed to flush HID structures");
                    }

                    if (Library.Options.HidCleanupDelayMs > 0)
                    {
                        Thread.Sleep(Library.Options.HidCleanupDelayMs);
                    }

                    Log.Info("HID structure flush completed");

                    return Enumerable.Empty<HidDeviceInfo>();
                }

                var result = new List<HidDeviceInfo>();

                var current = enumerated;
                while (current != IntPtr.Zero)
                {
                    // Use correct pointer type for our system
                    var ptr = Environment.Is64BitProcess
                        ? new IntPtr(current.ToInt64())
                        : new IntPtr(current.ToInt32());

                    var devinfo = (PrivateHidDeviceInfo) Marshal.PtrToStructure(ptr,
                        typeof(PrivateHidDeviceInfo));

                    // Copy to fully managed (e.g. no pointers) data type
                    var managed = new HidDeviceInfo
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
                    };
                    result.Add(managed);
                    Log.Trace("Found device: {0}", managed);

                    current = devinfo.Next;
                }
                Log.Trace("HID enumeration allocated for {0}-bit process", Environment.Is64BitProcess ? 64 : 32);

                _HidFreeEnumerate(enumerated);
                Log.Trace("HID enumeration free");

                return result;
            }
        }

        /// <summary>
        /// Free an enumeration Linked List
        ///
        /// This function frees a linked list created by HidEnumerate().
        /// </summary>
        /// <param name="devices">devices Pointer to a list of HidDeviceInfo returned from
        /// HidEnumerate()</param>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_free_enumeration", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _HidFreeEnumerate(IntPtr devices);

        /// <summary>
        /// Open a HID device by its path name.
        /// The path name be determined by calling hid_enumerate(), or a
        /// platform-specific path name can be used(eg: /dev/hidraw0 onLinux).
        /// </summary>
        /// <param name="devicePath">The path name of the device to open</param>
        /// <returns>returns a pointer to a #hid_device object on
        /// success or NULL on failure.</returns>
        [DllImport("hidapi", CharSet = CharSet.Ansi, EntryPoint = "hid_open_path", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _HidOpenPath(string devicePath);

        /// <inheritdoc />
        public HidDevice OpenPath(string devicePath)
        {
            lock (GlobalLock)
            {
                var handle = _HidOpenPath(devicePath);
                return new HidDevice(handle);
            }
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_close", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void _HidClose(IntPtr device);

        /// <inheritdoc />
        public void Close(HidDevice device)
        {
            lock (GlobalLock)
            {
                try
                {
                    _HidClose(device.Handle);
                }
                catch (SEHException ex)
                {
                    Log.Error(ex, "Failed to close HID port");
                    throw new PTIException("Failed to close HID handle: {0}", ex.Message);
                }
            }
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_read_timeout", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidRead(IntPtr device, byte[] data, UIntPtr length, int timeout);

        /// <inheritdoc />
        public int Read(HidDevice device, byte[] data, UIntPtr length, int timeout)
        {
            lock (_mInstanceLock)
            {
                return _HidRead(device.Handle, data, length, timeout);
            }
        }

        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_write", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidWrite(IntPtr device, byte[] data, UIntPtr length);

        /// <inheritdoc />
        public int Write(HidDevice device, byte[] data, UIntPtr length)
        {
            lock (_mInstanceLock)
            {
                return _HidWrite(device.Handle, data, length);
            }
        }

        /// <summary>
        /// Get The Manufacturer String from a HID device.
        /// </summary>
        /// <param name="device">A device handle returned from hid_open()</param>
        /// <param name="str">A wide string buffer to put the data into</param>
        /// <param name="length">The length of the buffer in multiples of wchar_t</param>
        /// <returns>This function returns 0 on success and -1 on error</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_get_manufacturer_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidGetManufacturerString(IntPtr device, StringBuilder str, uint length);

        /// <inheritdoc />
        public string GetManufacturerString(HidDevice device)
        {
            return ReadUnicodeString(device, _HidGetManufacturerString);
        }

        /// <summary>
        /// Get The Product String from a HID device.
        /// </summary>
        /// <param name="device">A device handle returned from hid_open().</param>
        /// <param name="str">A wide string buffer to put the data into.</param>
        /// <param name="size">The length of the buffer in multiples of wchar_t.</param>
        /// <returns>This function returns 0 on success and -1 on error.</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_get_product_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidGetProductString(IntPtr device, StringBuilder str, uint size);

        /// <inheritdoc />
        public string GetProductString(HidDevice device)
        {
            return ReadUnicodeString(device, _HidGetProductString);
        }

        /// <summary>
        /// Get The Serial Number String from a HID device.
        /// </summary>
        /// <param name="device">A device handle returned from hid_open().</param>
        /// <param name="str">A wide string buffer to put the data into.</param>
        /// <param name="size">The length of the buffer in multiples of wchar_t.</param>
        /// <returns>This function returns 0 on success and -1 on error.</returns>
        [DllImport("hidapi", CharSet = CharSet.Unicode, EntryPoint = "hid_get_serial_number_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _HidGetSerialNumber(IntPtr device, StringBuilder str, uint size);

        /// <inheritdoc />
        public string GetSerialNumber(HidDevice device)
        {
            return ReadUnicodeString(device, _HidGetSerialNumber);
        }

        /// <summary>
        /// Executes fn to read a unicode string from an HID device. The string will
        /// be parsed and returned as a regular, managed string. If there is an error, the returned value
        /// will be null.
        /// </summary>
        /// <param name="device">Device handle</param>
        /// <param name="fn">Function to execute</param>
        /// <returns>string, may be null</returns>
        private static string ReadUnicodeString(HidDevice device, Func<IntPtr, StringBuilder, uint, int> fn)
        {
            var sb = new StringBuilder();
            return fn.Invoke(device.Handle, sb, 255) != 0 ? null : sb.ToString();
        }
    }
}
