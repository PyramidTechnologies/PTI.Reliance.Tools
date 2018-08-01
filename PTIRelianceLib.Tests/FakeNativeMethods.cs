#region Header
// FakeNativeMethods.cs
// PTIRelianceLib.Tests
// Cory Todd
// 21-05-2018
// 11:43 AM
#endregion

namespace PTIRelianceLib.Tests
{
    using System;
    using System.Collections.Generic;
    using PTIRelianceLib.IO.Internal;

    internal class FakeHidDevice : HidDevice
    {
        public FakeHidDevice() : base(new IntPtr(1234))
        {
        }

        public override string ToString()
        {
            return "Fake Device";
        }
    }

    /// <summary>
    /// Mocks the native methods inteface at the lowest possible level
    /// </summary>
    internal class FakeNativeMethods : INativeMethods
    {
        public const string SerialNumber = "DEADBEEF1234567890";

        public const string DevicePath = "FakePath";

        public int Init()
        {
            return 0;
        }

        public string Error(HidDevice device)
        {
            return string.Empty;
        }

        public IEnumerable<HidDeviceInfo> Enumerate(ushort vid, ushort pid)
        {
            return new List<HidDeviceInfo>
            {
                new HidDeviceInfo
                {
                    VendorId = vid,
                    ProductId = pid,
                    Path = DevicePath
                }
            };
        }

        public HidDevice OpenPath(string devicePath)
        {
            return new FakeHidDevice();
        }

        public void Close(HidDevice device)
        {
            // Do nothing
        }

        public int Read(HidDevice device, byte[] data, UIntPtr length, int timeout)
        {
            var nextResp = GetNextResponse(data);
            Array.Resize(ref data, nextResp.Length);
            Array.Copy(nextResp, data, data.Length);
            return data.Length;
        }

        public int Write(HidDevice device, byte[] data, UIntPtr length)
        {
            return data.Length;
        }

        public string GetManufacturerString(HidDevice device)
        {
            return "Pyramid Technologies";
        }

        public string GetProductString(HidDevice device)
        {
            return "Test Device";
        }

        public string GetSerialNumber(HidDevice device)
        {
            return SerialNumber;
        }

        public bool IsPathOwned(string path)
        {
            // Always return false because tests may run in parallel and there is no
            // concept of path ownership.
            return false;
        }

        public Func<byte[], byte[]> GetNextResponse { get; set; }
    }
}