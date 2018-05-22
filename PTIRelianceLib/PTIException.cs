#region Header
// PTIException.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 8:07 AM
#endregion

namespace PTIRelianceLib
{
    using System;

    public class PTIException : Exception
    {
        public PTIException(string fmt, params object[] args) 
            : base(string.Format(fmt, args))
        {
        }
    }
}