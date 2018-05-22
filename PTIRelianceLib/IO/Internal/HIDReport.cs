#region Header
// HIDReport.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:19 AM
#endregion

namespace PTIRelianceLib.IO
{
    using System;
    using Internal;

    /// <summary>
    /// HID report structure
    /// </summary>
    internal class HidReport
    {
        public static HidReport MakeOutputReport(HidDeviceConfig config, byte[] data)
        {
            // [report id] [length] [... data ...]
            var buff = new byte[config.OutReportLength];
            buff[0] = config.OutReportId;
            buff[1] = (byte)(data.Length & 0xFF);
            Array.Copy(data, 0, buff, 2, Math.Min(data.Length, buff.Length-2));
            return new HidReport(buff);
        }

        public static HidReport MakeInputReport(HidDeviceConfig config)
        {
            // [report id] [length] [... data ...]
            var buff = new byte[config.InReportLength];
            buff[0] = config.InReportId;
            return new HidReport(buff);
        }

        /// <summary>
        /// Construct report with specified data of specified size
        /// </summary>
        /// <param name="data">Data to copy into report</param>
        private HidReport(byte[] data)
        {
            Data = (byte[])data.Clone();
            ReportId = data[0];
        }

        /// <summary>
        /// Get or set report id
        /// </summary>
        public byte ReportId { get; set; }

        /// <summary>
        /// Report data
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Returns a copy of the payload portion of the report
        /// </summary>
        /// <returns></returns>
        public byte[] GetPayload()
        {
            // [id] [length] [... payload ...]
            var payload = new byte[Data[1]];
            Array.Copy(Data, 2, payload, 0, payload.Length);            
            return payload;
        }

        /// <summary>
        /// Returns size of data as UInPtr
        /// </summary>
        public UIntPtr Size => new UIntPtr(Convert.ToUInt32(Data.Length));
    }
}