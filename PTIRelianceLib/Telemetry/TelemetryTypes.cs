#region Header
// TelemetryTypes.cs
// PTIRelianceLib
// Cory Todd
// 25-06-2018
// 7:40 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    /// <summary>
    /// Context of telemtry data
    /// </summary>
    internal enum TelemetryTypes
    {
        /// <summary>
        /// Data is counted since factory manugacturing
        /// </summary>
        Lifetime = 0,
        /// <summary>
        /// Data is counted since last powerup
        /// </summary>
        Powerup = 1,
    }
}