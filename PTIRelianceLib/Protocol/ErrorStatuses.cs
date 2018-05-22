#region Header
// ErrorStatuses.cs
// PTIRelianceLib
// Cory Todd
// 22-05-2018
// 7:12 AM
#endregion

namespace PTIRelianceLib.Protocol
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Translates error byte to status
    /// </summary>
    [Flags]
    public enum ErrorStatuses : byte
    {
        /// <summary>
        /// Printer is jammed
        /// </summary>
        [EnumMember(Value = "Jammed")]
        Jammed = 1 << 0,
        /// <summary>
        /// Printer is overheated
        /// </summary>
        [EnumMember(Value = "OverHeated")]
        OverHeated = 1 << 1,
        /// <summary>
        /// Cutter blade cannot move freely
        /// </summary>
        [EnumMember(Value = "Cutter Blocked")]
        Cutter = 1 << 2,
        /// <summary>
        /// Printer is has too much voltage applied
        /// </summary>
        [EnumMember(Value = "VoltageHigh")]
        VoltageHigh = 1 << 3,
        /// <summary>
        /// Printer has too little voltage applied
        /// </summary>
        [EnumMember(Value = "VoltageLow")]
        VoltageLow = 1 << 4,
        /// <summary>
        /// Printer lid (and by extension, head) are open
        /// </summary>
        [EnumMember(Value = "Platen Open")]
        PlatenOpen = 1 << 5,
        /// <summary>
        /// Reserved
        /// </summary>
        [EnumMember(Value = "Firmware Corruption")]
        CorruptFirmware = 1 << 6,
        /// <summary>
        /// Printer is reporting an unknown error
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown = 1 << 7,
    }
}