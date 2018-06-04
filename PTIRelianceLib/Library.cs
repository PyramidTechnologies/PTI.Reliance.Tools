namespace PTIRelianceLib
{
    using System.Reflection;

    /// <summary>
    /// Contains library metadata and configurables
    /// </summary>
    public static class Library
    {
        /// <summary>
        /// Returns the assembly file version of this library
        /// </summary>
        /// <value>Assembly version string</value>
        public static string Version => typeof(Library).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        /// <summary>
        /// Gets or Sets the delay in milliseconds that is used to way after
        /// closing and cleaning up after an HID port.
        /// </summary>
        /// <value>Integer delay in milliseconds</value>
        public static int HidCleanupDelayMs { get; set; } = 50;
    }
}
