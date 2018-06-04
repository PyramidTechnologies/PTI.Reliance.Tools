namespace PTIRelianceLib
{
    using System;
    using System.Collections.Generic;
    using IO.Internal;

    internal interface INativeMethods
    {
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
        int Init();

        /// <summary>
        /// Get a string describing the last error which occurred.
        /// </summary>
        /// <param name="device">A device handle returned from <see cref="OpenPath"/>.</param>
        /// <returns>This function returns a string containing the last error
        /// which occurred or empty string if none has occurred.</returns>
        string Error(HidDevice device);

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
        IEnumerable<HidDeviceInfo> Enumerate(ushort vid, ushort pid);

        /// <summary>
        /// Open a HID device by its path name.
        /// 
        /// The path name be determined by calling HidEnumerate(), or a
        /// platform-specific path name can be used (eg: /dev/hidraw0 on
        /// Linux).
        /// </summary>
        /// <param name="devicePath">The path name of the device to open</param>
        /// <returns>This function returns a pointer to a #hid_device object on
        /// success or <c>null</c> on failure.</returns>
        HidDevice OpenPath(string devicePath);

        /// <summary>
        /// Close a HID device.
        /// </summary>
        /// <param name="device">A device handle returned from hid_open().</param>
        void Close(HidDevice device);

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
        int Read(HidDevice device, byte[] data, UIntPtr length, int timeout);

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
        int Write(HidDevice device, byte[] data, UIntPtr length);
    }
}