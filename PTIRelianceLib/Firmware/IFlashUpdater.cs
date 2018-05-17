#region Header
// IFlashUpdater.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 8:58 AM
#endregion

namespace PTIRelianceLib.Firmware
{
    internal interface IFlashUpdater
    {
        /// <summary>
        /// Perform the flash update and return result
        /// </summary>
        /// <returns>Result</returns>
        ReturnCodes ExecuteUpdate();
    }
}