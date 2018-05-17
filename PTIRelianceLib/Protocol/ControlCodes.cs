#region Header
// ControlCodes.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 10:40 AM
#endregion

namespace PTIRelianceLib.Protocol
{
    internal enum ControlCodes
    {
        Ack = 0xAA,
        Nak = 0xAC,
        Ser = 0xAB,
        Timeout = 0xFF,
    }
}