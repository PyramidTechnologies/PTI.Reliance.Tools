#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:12 AM
#endregion

namespace PTIRelianceLib
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Contract for Pyramid Device interaction
    /// </summary>
    public interface IPyramidDevice : IDisposable
    {
        /// <summary>
        /// Writes the configuration data specified by config to this device.
        /// Config is a JSON file describing the configuration to apply. Any
        /// fields omitted from the configuration file will be set to its
        /// default value.
        /// </summary>
        /// <param name="config">Confguration to apply</param>
        /// <returns>Return code</returns>
        ReturnCodes SendConfiguration(BinaryFile config);

        /// <summary>
        /// Reads current configuration from this device and returns result
        /// as a JSON stream. If there is an error reading the configuration,
        /// an empty stream is returned (check the Empty property of the result).
        /// </summary>
        /// <returns>BinaryFile</returns>
        BinaryFile ReadConfiguration();

        /// <summary>
        /// Install the specified firmware on this device. With few exceptions,
        /// most any firmware can be installed using this method. The reporter
        /// parameter provides callbacks for messages, progress, and errors.
        /// </summary>
        /// <param name="firmware">Firmware image</param>
        /// <param name="reporter">Progress callback instance</param>
        /// <returns>Return code</returns>
        ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter);

        /// <summary>
        /// Returns the firmware revision of this device. If there is an error
        /// reading the firmware revision, "0.0.0" will be returned.
        /// </summary>
        /// <returns>Revision</returns>
        Revlev GetFirmwareRevision();

        /// <summary>
        /// Returns the serial number for this device
        /// </summary>
        /// <returns></returns>
        string GetSerialNumber();

        /// <summary>
        /// Ping returns Okay if the device is online and not in the middle of printing.
        /// </summary>
        /// <returns>Okay if online and ready, else ExecutionFailure</returns>
        ReturnCodes Ping();

        /// <summary>
        /// Immediately reboots this device.
        /// </summary>
        /// <returns>Return code</returns>
        ReturnCodes Reboot();
    }
}