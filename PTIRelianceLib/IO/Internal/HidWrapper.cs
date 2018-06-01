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
    using System.Diagnostics;

    [DebuggerDisplay("IsOpen = {IsOpen}, LastError = {LastError}")]
    internal class HidWrapper : IDisposable
    {       
        private HidDevice Device { get; set; }
        private readonly HidDeviceConfig _deviceConfig;

        /// <summary>
        /// Creates a new HID port wrapper
        /// </summary>
        /// <param name="config">USB HID configuration</param>
        public HidWrapper(HidDeviceConfig config)
        {
            // Setup HIDAPI structures
            if (config.NativeHid.Init() != 0)
            {
                CheckError();
            }

            Device = HidDevice.Invalid();
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
        public bool IsOpen => Device.IsValid;

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

            Device = GetHidHandle();

            CheckError();

            return IsOpen;
        }

        /// <summary>
        /// Close and release USB handle
        /// </summary>
        public void Close()
        {            
            Device = HidDevice.Invalid();
        }

        /// <summary>
        /// Searchs USB HID devices for the specified vid/pid pair. Returns the raw
        /// handle to the device if found. Otherwise IntPtr.Zero is returned
        /// </summary>
        /// <returns>Device handle or IntPtr.Zero if no match found</returns>
        private HidDevice GetHidHandle()
        {
            foreach (var devinfo in _deviceConfig.NativeHid.Enumerate(_deviceConfig.VendorId, _deviceConfig.ProductId))
            {
                var handle = _deviceConfig.NativeHid.OpenPath(devinfo.Path);

                if (handle.IsValid)
                {
                    return handle;
                }
            }

            return HidDevice.Invalid();
        }

        /// <summary>
        /// Writes data to USB port
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <returns></returns>
        public int WriteData(byte[] data)
        {
            if (!Device.IsValid)
            {
                return -1;
            }

            var report = HidReport.MakeOutputReport(_deviceConfig, data);  
            var result = _deviceConfig.NativeHid.Write(Device, report.Data, report.Size);

            CheckError();

            return result;
        }

        /// <summary>
        /// Reads and returns available data. -1 is a blocking read.
        /// </summary>
        /// <param name="timeoutMs">Time in milliseconds to wait before giving up read</param>
        /// <returns>Read data or empty if no data read</returns>
        public byte[] ReadData(int timeoutMs)
        {
            if (!Device.IsValid)
            {
                return new byte[0];
            }

            var inreport = HidReport.MakeInputReport(_deviceConfig);
            var read = _deviceConfig.NativeHid.Read(Device, inreport.Data, inreport.Size, timeoutMs);

            var result = new byte[0];
            if (read > 0)
            {
                result = inreport.GetPayload();
            }

            CheckError();

            return result;
        }
        /// <summary>
        /// Captures last error message on this device
        /// </summary>
        private void CheckError()
        {
            LastError = _deviceConfig.NativeHid.Error(Device);            
        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return string.Format("HID [{0:X4},{1:X4}] (Open? {2})", 
                _deviceConfig.VendorId, _deviceConfig.ProductId, IsOpen);
        }
    }
}