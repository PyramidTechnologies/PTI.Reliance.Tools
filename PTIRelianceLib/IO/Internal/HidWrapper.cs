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
    using System.Linq;
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

            Device = HidDevice.Invalid();

            // Setup HIDAPI structures
            config.NativeHid.Init();

            // Attempt to open device
            Open();

            Log.Trace("New HidWrapper created and is {0}", IsOpen ? "Open" : "Not Open");
        }

        /// <summary>
        /// Returns last error message
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Returns path to this device if in a valid state
        /// </summary>
        public string DevicePath => Device.IsValid ? Device.DevicePath : string.Empty;

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
            Log.Trace("Called HidWrapper.Open()");

            if (IsOpen)
            {
                Close();
            }

            Device = GetHidHandle(_mDeviceConfig.DevicePath);

            if (!Device.IsValid)
            {
                Log.Trace("Device has invalid handle");
                return IsOpen;
            }

            var path = string.IsNullOrEmpty(Device.DevicePath) ? "{missing}" : Device.DevicePath;
            Log.Trace("Connected to device path {0}", path);

            return IsOpen;
        }

        /// <summary>
        /// Close and release USB handle
        /// </summary>
        public void Close()
        {
            _mDeviceConfig.NativeHid.Close(Device);

            Log.Trace("HidWrapper closed");

            if (Library.Options.HidCleanupDelayMs <= 0)
            {
                return;
            }

            Log.Info(" ({0} ms delay here)", Library.Options.HidCleanupDelayMs);

            // Make sure handle is closed in case enumeration call is made immedately
            // after device is closed.
            Thread.Sleep(Library.Options.HidCleanupDelayMs);
        }

        /// <summary>
        /// Returns true if device info's path matches requested path. If the requested
        /// path is null or empty, true will be returned.
        /// </summary>
        /// <param name="devinfo">Device info</param>
        /// <param name="devicePath">Path to match</param>
        /// <returns>True on match</returns>
        private static bool IsDevicePathMatch(HidDeviceInfo devinfo, string devicePath)
        {
            Log.Trace("Attempting to match device to path: {0}", devicePath);
            return string.IsNullOrEmpty(devicePath) || devicePath.Equals(devinfo.Path);
        }

        /// <summary>
        /// Searchs USB HID devices for the specified vid/pid pair. Returns the raw
        /// handle to the device if found. Otherwise IntPtr.Zero is returned
        /// </summary>
        /// <param name="requestedPath">Optional device path to match on.
        /// Set to empty or null to accept first available</param>
        /// <returns>Device handle or IntPtr.Zero if no match found</returns>
        private HidDevice GetHidHandle(string requestedPath = "")
        {
            var devices = _mDeviceConfig.NativeHid.Enumerate(_mDeviceConfig.VendorId, _mDeviceConfig.ProductId);

            foreach (var devinfo in devices)
            {
                if (_mDeviceConfig.NativeHid.IsPathOwned(devinfo.Path))
                {
                    Log.Info("Path is already open, skipping");
                    continue;
                }

                var handle = _mDeviceConfig.NativeHid.OpenPath(devinfo.Path);
                var err = _mDeviceConfig.NativeHid.Error(handle);

                if (!string.IsNullOrEmpty(err))
                {
                    Log.Error("Error opening device handle: {0}", err);
                }                
                else if (!handle.IsValid)
                {
                    Log.Warn("OpenPath returned invalid handle");
                }
                else if (!IsDevicePathMatch(devinfo, requestedPath))
                {
                    Log.Info("Skipping non-matched device");

                    _mDeviceConfig.NativeHid.Close(handle);                    
                }
                else
                {
                    Log.Info("Found valid device handle");

                    handle.DevicePath = devinfo.Path;

                    return handle;
                }
            }

            return HidDevice.Invalid();
        }

        /// <summary>
        /// Writes data to USB port
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <returns>Actual number of bytes written, -1 on error</returns>
        public int WriteData(byte[] data)
        {
            if (!Device.IsValid)
            {
                return -1;
            }

            var report = HidReport.MakeOutputReport(_mDeviceConfig, data);
            var result = _mDeviceConfig.NativeHid.Write(Device, report.Data, report.Size);

            if (result < 0)
            {
                CheckError();
            }

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
            else
            {
                CheckError();
            }

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