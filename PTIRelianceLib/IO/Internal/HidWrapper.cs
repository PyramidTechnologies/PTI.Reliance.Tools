#region Header
// HidWrapper.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:30 AM
#endregion


namespace PTIRelianceLib.IO.Internal
{
    using Logging;
    using System;
    using System.Diagnostics;
    using System.Threading;

    [DebuggerDisplay("IsOpen = {IsOpen}, LastError = {LastError}")]
    internal class HidWrapper : IDisposable
    {
        private static readonly ILog Log = LogProvider.For<NativeMethods>();

        private HidDevice Device { get; set; }
        private readonly HidDeviceConfig _mDeviceConfig;

        /// <summary>
        /// Creates a new HID port wrapper
        /// </summary>
        /// <param name="config">USB HID configuration</param>
        public HidWrapper(HidDeviceConfig config)
        {
            _mDeviceConfig = config;

            // Setup HIDAPI structures
            if (config.NativeHid.Init() != 0)
            {
                CheckError();
            }

            Device = HidDevice.Invalid();

            Open();

            Log.Trace("New HidWrapper created");            
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

            Log.Trace("HidWrapper opened okay: {0}", IsOpen);            

            return IsOpen;
        }

        /// <summary>
        /// Close and release USB handle
        /// </summary>
        public void Close()
        {
            _mDeviceConfig.NativeHid.Close(Device);
            Log.Trace("HidWrapper closed ({0} ms delay here)", Library.Options.HidCleanupDelayMs);

            // Make sure handle is closed in case enumeration call is made immedately
            // after device is closed.
            Thread.Sleep(Library.Options.HidCleanupDelayMs);
        }

        /// <summary>
        /// Searchs USB HID devices for the specified vid/pid pair. Returns the raw
        /// handle to the device if found. Otherwise IntPtr.Zero is returned
        /// </summary>
        /// <returns>Device handle or IntPtr.Zero if no match found</returns>
        private HidDevice GetHidHandle()
        {
            var devices = _mDeviceConfig.NativeHid.Enumerate(_mDeviceConfig.VendorId, _mDeviceConfig.ProductId);
            foreach (var devinfo in devices)
            {
                var handle = _mDeviceConfig.NativeHid.OpenPath(devinfo.Path);

                if (handle.IsValid)
                {
                    return handle;
                }
                
                Log.Trace("Enumeration returned invalid handle");                
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

            var report = HidReport.MakeOutputReport(_mDeviceConfig, data);  
            var result = _mDeviceConfig.NativeHid.Write(Device, report.Data, report.Size);

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

            var inreport = HidReport.MakeInputReport(_mDeviceConfig);
            var read = _mDeviceConfig.NativeHid.Read(Device, inreport.Data, inreport.Size, timeoutMs);

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
            LastError = _mDeviceConfig.NativeHid.Error(Device);
            if (!string.IsNullOrEmpty(LastError))
            {
                Log.Info("Error Set: {0}", LastError);
            }
        }

        public override string ToString()
        {
            return string.Format("HID [{0:X4},{1:X4}] (Open? {2})", 
                _mDeviceConfig.VendorId, _mDeviceConfig.ProductId, IsOpen);
        }

        public void Dispose()
        {
            Close();
            Log.Trace("HidWrapper destroyed");
        }
    }
}