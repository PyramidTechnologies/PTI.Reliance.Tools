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

    public interface IPyramidDevice : IDisposable
    {
        ReturnCodes SendConfiguration(BinaryFile config);

        ReturnCodes FlashUpdateTarget(BinaryFile firmware);

        Revlev GetRevlev();

        string GetSerialNumber();

        ReturnCodes Ping();
    }
}