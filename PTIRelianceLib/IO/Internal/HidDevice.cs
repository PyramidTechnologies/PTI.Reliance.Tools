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
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Create a new device with this handle
        /// </summary>
        /// <param name="handle"></param>
        public HidDevice(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Returns true if this device has a valid handle
        /// </summary>
        public bool IsValid
        {
            get=> Handle != IntPtr.Zero;
            protected set => Handle = value ? Handle : IntPtr.Zero;
        }

        /// <summary>
        /// Returns an empty (invalid) device handle
        /// </summary>
        /// <returns>Device handle</returns>
        internal static HidDevice Invalid()
        {
            return new HidDevice(IntPtr.Zero);
        }
    }
}