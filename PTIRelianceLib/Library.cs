﻿namespace PTIRelianceLib
{
    using System.Linq;
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
        /// Gets or Sets Library options for this duration of your application.
        /// It is recommended to set this field only once, at the start of your
        /// application. Altering this property or the contents of this property
        /// during runtime may result in undefined behavior.
        /// </summary>
        /// <value>Current Library Options</value>
        public static LibraryOptions Options = LibraryOptions.Default;

        /// <summary>
        /// Returns true if at least one Reliance Thermal printer is attached to this system
        /// </summary>
        /// <returns>true if a Reliance is enumerated on this system, otherwise false</returns>
        /// <value>Boolean</value>
        public static bool IsRelianceAttached()
        {
            var handle = new NativeMethods();
            var reliances = handle.Enumerate(ReliancePrinter.VendorId, ReliancePrinter.ProductId);
            return reliances.Any();
        }
    }
}
