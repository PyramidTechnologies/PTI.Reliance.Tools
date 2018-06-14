#region Header
// IFlashUpdater.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 8:58 AM
#endregion

namespace PTIRelianceLib.Flash
{
    internal interface IFlashUpdater
    {
        /// <summary>
        /// Perform the flash data write and return result
        /// </summary>
        /// <returns>Result</returns>
        ReturnCodes ExecuteUpdate();
    }
}