#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:12 AM
#endregion

namespace PTIRelianceLib
{
    public interface IPyramidDevice
    {
        ReturnCodes SendConfiguration(BinaryFile config);

        ReturnCodes FlashUpdateTarget(BinaryFile firmware);

        Revlev GetRevlev();

        string GetSerialNumber();

        ReturnCodes Ping();
    }
}