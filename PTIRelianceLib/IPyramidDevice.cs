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
    /// <see cref="IPyramidDevice"/> is a contract defining what actions all Pyramid
    /// products in this library will support. These devices own their own communication source
    /// and can be used in MVC binding patterns which is why they are marked IDisposable/>.
    /// </summary>
    public interface IPyramidDevice : IDisposable
    {
        /// <summary>
        /// Writes the configuration <see cref="BinaryFile"/> specified by
        /// <paramref name="config"/> to this device. The configuration is a JSON file
        /// describing the configuration to apply. Any fields omitted from the
        /// configuration file will be set to its default value.
        ///
        /// There are two ways to obtain a JSON configuration file.
        /// <list type="bullet">
        /// <item>
        ///<description>Using Reliance Tools for PC you may click File->Save Config.
        /// This will produce a valid JSON configuration of the attached printer.
        /// <see href="https://pyramidacceptors.com/app/reliance-tools/"/></description>
        /// </item>
        /// <item>
        ///<description>Use <see cref="IPyramidDevice.ReadConfiguration()"></see> to get
        /// a configuration file.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="config">Confguration to apply</param>
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description><see cref="ReturnCodes.Okay"/> when configuration is successful</description>
        /// </item>
        ///<item>
        /// <description>Other <see cref="ReturnCodes"/> if configuration fails</description>
        /// </item>
        /// </list>
        /// </returns>
        ReturnCodes SendConfiguration(BinaryFile config);

        /// <summary>
        /// Reads current configuration from this device and returns result
        /// as a JSON <see cref="BinaryFile"/> If there is an error reading the
        /// configuration, an empty <see cref="BinaryFile"/> is returned. You can check for
        /// an empty result via <see cref="BinaryFile.Empty"/>.
        /// </summary>
        /// <returns>BinaryFile</returns>
        BinaryFile ReadConfiguration();

        /// <summary>
        /// Install the specified firmware <see cref="BinaryFile"/> on
        /// this device. With few exceptions, most any firmware can be installed
        /// using this method. The <paramref name="reporter"/> <see cref="ProgressMonitor"/>
        /// provides callbacks for messages, progress, and errors.
        /// </summary>
        /// <param name="firmware">Firmware image data</param>
        /// <param name="reporter">Progress callback instance</param>
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description><see cref="ReturnCodes.Okay"/> when update is successful</description>
        /// </item>
        ///<item>
        /// <description>Another <see cref="ReturnCodes"/> if update fails</description>
        /// </item>
        /// </list>
        /// </returns>
        ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter);

        /// <summary>
        /// Returns the firmware <see cref="Revlev"/> of this device.
        /// If there is an error reading the firmware revision, a Revlev of
        /// <c>0.0.0</c> will instead be returned.
        /// </summary>
        /// <returns>Revision</returns>
        Revlev GetFirmwareRevision();

        /// <summary>
        /// Returns the serial number for this device
        /// </summary>
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description>9-digit serial number string when successful</description>
        /// </item>
        ///<item>
        /// <description>An empty string on failure</description>
        /// </item>
        /// </list>
        /// </returns>
        string GetSerialNumber();

        /// <summary>
        /// Ping returns <see cref="ReturnCodes.Okay"/> if the device is online and not in the middle of critical work.
        /// </summary>
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description><see cref="ReturnCodes.Okay"/> when device is online and ready</description>
        /// </item>
        ///<item>
        /// <description><see cref="ReturnCodes.ExecutionFailure"/> if device is offline or busy</description>
        /// </item>
        /// </list>
        /// </returns>
        ReturnCodes Ping();

        /// <summary>
        /// Immediately reboots this device, taking care to handle any required port disconnection
        /// and reconnection details.
        /// </summary>
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description><see cref="ReturnCodes.Okay"/> when update is successful</description>
        /// </item>
        ///<item>
        /// <description>Other <see cref="ReturnCodes"/> if update fails</description>
        /// </item>
        /// </list>
        /// </returns>
        ReturnCodes Reboot();
    }
}