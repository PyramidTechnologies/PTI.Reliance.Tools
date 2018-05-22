#region Header
// IMemoryMap.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:02 AM
#endregion

namespace PTIRelianceLib.Firmware.Internal
{
    internal interface IMemoryMap
    {
        /// <summary>
        /// First address is the starting non-volatile address to which we flash
        /// a firmware image.
        /// </summary>
        uint FirstAddress { get; }

        /// <summary>
        /// Returns true if the given address is illegal for this implementation
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool IsIllegalAddress(uint address);

        /// <summary>
        /// Returns true if the given address falls within a range that should be checksummed
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool IsRangeChecksummed(uint address);

        /// <summary>
        /// Returns true if the specified address is beyond the last flashable address
        /// </summary>
        /// <param name="address">Target address</param>
        /// <returns>True if the currently specified address greater than or equal to 
        /// the absolute last flash address</returns>
        bool IsLastAddress(uint address);
    }
}