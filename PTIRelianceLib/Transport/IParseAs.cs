#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:08 AM
#endregion

namespace PTIRelianceLib.Transport
{
    internal interface IParseAs<out T> where T : IParseable
    {        
        /// <summary>
        /// Parses a packet into a strongly typed model
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        T Parse(IPacket packet);
    }
}
