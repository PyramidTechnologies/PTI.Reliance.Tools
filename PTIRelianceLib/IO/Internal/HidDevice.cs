#region Header
// HIDDevice.cs
// PTIRelianceLib
// Cory Todd
// 21-05-2018
// 11:47 AM
#endregion

namespace PTIRelianceLib.IO.Internal
{
    using System;

    /// <summary>
    /// Opaque HID device handle
    /// </summary>
    internal class HidDevice
    {
        /// <summary>
        /// Handle to device
        /// </summary>
        public readonly IntPtr Handle;

        /// <summary>
        /// Invalidated flag so we can keep an immutable Handle reference
        /// </summary>
        protected bool _mInvalidated;

        /// <summary>
        /// Create a new device with this handle
        /// </summary>
        /// <param name="handle"></param>
        public HidDevice(IntPtr handle)
        {
            Handle = handle;
            _mInvalidated = handle == IntPtr.Zero;
        }

        public bool IsValid => Handle != IntPtr.Zero;

        /// <summary>
        /// Returns an empty (invalid) device handle
        /// </summary>
        /// <returns>Device handle</returns>
        internal static HidDevice Invalid()
        {
            return new HidDevice(IntPtr.Zero);
        }

        public virtual void Dispose()
        {
            if (_mInvalidated)
            {
                return;
            }

            NativeMethods._HidClose(Handle);
            _mInvalidated = true;
        }
    }


}