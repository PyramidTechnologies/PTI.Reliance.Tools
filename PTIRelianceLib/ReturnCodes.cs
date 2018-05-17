#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:11 AM
#endregion

namespace PTIRelianceLib
{
    public enum ReturnCodes
    {
        Okay,
        ExecutionFailure,
        DeviceNotConnected,
        InvalidRequest,
        InvalidRequestPayload,
        TargetStoppedResponding,
        FlashFileInvalid,
        FlashPermissionDenied,
        FlashChecksumMismatch,
        OperationAborted
    }
}
