#region Header
// LibraryOptions.cs
// PTIRelianceLib
// Cory Todd
// 04-06-2018
// 9:46 AM
#endregion

namespace PTIRelianceLib
{
    /// <summary>
    /// Configurable Library settings for compatibility with non-standard configurations.
    /// </summary>
    public class LibraryOptions
    {
        /// <summary>
        /// Gets or Sets the delay in milliseconds to block for after
        /// closing and cleaning up after an HID port. This primarily affects
        /// reboot calls during the flash update process.
        /// Default: 0 ms
        /// </summary>
        /// <value>Integer delay in milliseconds</value>
        public int HidCleanupDelayMs { get; set; }

        /// <summary>
        /// Gets or Sets the delay in millisecond that is used during Hid reconnection
        /// attempts. For devices with slow/no device event loops this has no effect.
        /// Instead, set <see cref="HidFlushStructuresOnEnumError"/> to make sure
        /// that fresh HID devices are getting discovered on enumeration.
        /// Default: 1000
        /// </summary>
        public int HidReconnectDelayMs { get; set; }

        /// <summary>
        /// When true, if HID enumeration returns no results, flush the
        /// HID data structures (effectively forcing a device event loop poll)
        /// and delay <see cref="HidCleanupDelayMs"/> ms before returning. This
        /// does not fix the current enumeration but the next call to enumeration
        /// will have fresh device data. This primarily affects reboot calls
        /// during the flash update process.
        /// Default: false
        /// </summary>
        public bool HidFlushStructuresOnEnumError { get; set; }

        /// <summary>
        /// Returns the default library options for this library.
        /// </summary>
        public static LibraryOptions Default => new LibraryOptions
        {
            HidCleanupDelayMs = 0,
            HidReconnectDelayMs = 1000,
            HidFlushStructuresOnEnumError = false,
        };

        /// <summary>
        /// Returns the library options that work well for Debian Stretch Docker images
        /// </summary>
        public static LibraryOptions DockerLinuxStretch => new LibraryOptions
        {
            HidCleanupDelayMs = 250,
            HidReconnectDelayMs = 1000,
            HidFlushStructuresOnEnumError = true,
        };
    }    
}