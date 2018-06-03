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
    }
}
