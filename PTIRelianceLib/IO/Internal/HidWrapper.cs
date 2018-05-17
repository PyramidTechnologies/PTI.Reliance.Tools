#region Header
// HidWrapper.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:30 AM
#endregion

namespace PTIRelianceLib.IO.Internal
{
    using System;
    using System.Runtime.InteropServices;

    internal class HidWrapper : IDisposable
    {       
        private IntPtr Handle { get; set; }
        private readonly HidDeviceConfig _deviceConfig;

        /// <summary>
        /// Creates a new HID port wrapper
        /// </summary>
        /// <param name="config">USB HID configuration</param>
        public HidWrapper(HidDeviceConfig config)
        {
            _deviceConfig = config;
            Open();
        }

        /// <summary>
        /// Returns last error message
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Returns true if USB handle is open
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Opens USB port for communication
        /// </summary>
        /// <returns>True on succes, else False</returns>
        public bool Open()
        {
            if (IsOpen)
            {
                Close();
            }

            Handle = GetHidHandle();
            IsOpen = Handle != IntPtr.Zero;

            return IsOpen;
        }

        /// <summary>
        /// Close and release USB handle
        /// </summary>
        public void Close()
        {
            if (Handle == IntPtr.Zero)
            {
                return;
            }

            NativeMethods.HidClose(Handle);
        }

        /// <summary>
        /// Searchs USB HID devices for the specified vid/pid pair. Returns the raw
        /// handle to the device if found. Otherwise IntPtr.Zero is returned
        /// </summary>
        /// <returns>Device handle or IntPtr.Zero if no match found</returns>
        private IntPtr GetHidHandle()
        {
            var enumerated = NativeMethods.HidEnumerate(_deviceConfig.VendorId, _deviceConfig.ProductId);
            if (enumerated == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var current = enumerated;
            while (current != IntPtr.Zero)
            {
                var devinfo = (NativeMethods.HidDeviceInfo)Marshal.PtrToStructure(enumerated,
                    typeof(NativeMethods.HidDeviceInfo));

                var handle = OpenByPath(devinfo.Path);

                if (handle != IntPtr.Zero)
                {
                    return handle;
                }

                current = devinfo.Next;
            }

            NativeMethods.HidFreeEnumerate(enumerated);

            return IntPtr.Zero;
        }

        /// <summary>
        /// Writes data to USB port
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <returns></returns>
        public int WriteData(byte[] data)
        {
            if (Handle == IntPtr.Zero)
            {
                return -1;
            }

            var report = HidReport.MakeOutputReport(_deviceConfig, data);  
            var result = NativeMethods.HidWrite(Handle, report.Data, report.Size);

            CheckError();

            return result;
        }

        public byte[] ReadData()
        {
            if (Handle == IntPtr.Zero)
            {
                return new byte[0];
            }

            var inreport = HidReport.MakeInputReport(_deviceConfig);
            var read = NativeMethods.HidRead(Handle, inreport.Data, inreport.Size);

            var result = new byte[0];
            if (read > 0)
            {
                result = inreport.GetPayload();
            }

            CheckError();
            return result;
        }

        /// <summary>
        /// Opens HID port by device path
        /// </summary>
        /// <param name="devicePath">System device path</param>
        /// <returns>Handle to device on succes, else IntPtr.Zero</returns>
        private static IntPtr OpenByPath(string devicePath)
        {
            return string.IsNullOrEmpty(devicePath) ? IntPtr.Zero : NativeMethods.HidOpen(devicePath);
        }

        /// <summary>
        /// Captures last error message on this device
        /// </summary>
        private void CheckError()
        {
            LastError = Marshal.PtrToStringUni(NativeMethods.HidError(Handle));
        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return string.Format("HID [{0:X4},{1:X4}] ({2})", 
                _deviceConfig.VendorId, _deviceConfig.ProductId, IsOpen);
        }
    }
}