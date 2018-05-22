#region Header
// SensorStatuses.cs
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
    /// Translates status byte to sensor status
    /// </summary>
    [Flags]
    public enum SensorStatuses : byte
    {
        /// <summary>
        /// 0 = platen off			1 = platen on
        /// </summary>
        [EnumMember(Value = "Platten")]
        Platen = 1 << 0,
        /// <summary>
        /// 0 = cutter not home		1 = cutter home
        /// </summary>
        [EnumMember(Value = "Cutter")]
        Cutter = 1 << 1,
        /// <summary>
        /// 0 = Tach error 			1 = Tach OK
        /// </summary>
        [EnumMember(Value = "Tach")]
        Tach = 1 << 2,
        /// <summary>
        /// 0 = No paper at presenter 	1 = Paper Present at presenter
        /// </summary>
        [EnumMember(Value = "Presenter")]
        Presenter = 1 << 3,
        /// <summary>
        /// 0 = No paper in path 	1 = Paper Present in path
        /// </summary>
        [EnumMember(Value = "Path")]
        Path = 1 << 4,
        /// <summary>
        /// 0 = No paper at presenter 1 = Paper Present at presenter
        /// </summary>
        [EnumMember(Value = "Paper")]
        Paper = 1 << 5,
        /// <summary>
        /// 0 = No notch detected	1 = Notch detected
        /// </summary>
        [EnumMember(Value = "Notch")]
        Notch = 1 << 6,
        /// <summary>
        /// 0 = No paper on arm		1 = Paper detected on arm
        /// </summary>       
        [EnumMember(Value = "Arm")]
        Arm = 1 << 7,
    };
}