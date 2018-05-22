#region Header
// Status.cs
// PTIRelianceLib
// Cory Todd
// 22-05-2018
// 7:11 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Protocol;
    using Transport;

    /// <inheritdoc />
    /// <summary>
    /// @clr
    /// CLR wrapper for Printer Status struct
    /// </summary>
    public class Status : IParseable
    {
        /// <summary>
        /// 12-bit ADC factor for a 3.3 volt reference
        /// </summary>
        private const float AdcConstant = 0.000806f;

        /// <summary>
        /// ASCII string of the head input voltage. "XX.XX" (Volts)
        /// </summary>
        public string HeadVoltage { get; internal set; }
        /// <summary>
        /// Temperature of the head in deg C
        /// </summary>
        public byte HeadTemp { get; internal set; }

        /// <summary>
        /// Sensor statuses
        /// </summary>
        public SensorStatuses SensorStatus { get; internal set; }

        /// <summary>
        /// Raw ADC value for presenter sensor
        /// </summary>
        public ushort PresenterRaw { get; internal set; }
        /// <summary>
        /// Raw ADC value for path sensor
        /// </summary>
        public ushort PathRaw { get; internal set; }
        /// <summary>
        /// Raw ADC value for paper sensor
        /// </summary>
        public ushort PaperRaw { get; internal set; }
        /// <summary>
        /// Raw ADC value for notch sensor
        /// </summary>
        public ushort NotchRaw { get; internal set; }
        /// <summary>
        /// Raw ADC value for arm sensor
        /// </summary>
        public ushort ArmRaw { get; internal set; }

        /// <summary>
        /// Where the ticket is at.
        /// - 0 : Idle = no ticket
        /// - 1 : Printing = Ticket has data
        /// - 2 : Un-presented Ticket = Ticket is cut but not presented
        /// - 3 : Presented Ticket = Ticket is cut and presented
        /// </summary>
        public TicketStates TicketStatus { get; internal set; }

        /// <summary>
        /// Printer error status
        /// </summary>
        public ErrorStatuses PrinterErrors { get; internal set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Head Voltage: {0} V DC\n", HeadVoltage);
            sb.AppendFormat("Head Temperature: {0} °C\n", HeadTemp);
            sb.AppendFormat("Head Temperature: {0} °F\n", HeadTemp);
            sb.AppendFormat("Sensor Status: {0} \n", SensorStatus);
            sb.AppendFormat("Presenter Raw: {0} V DC\n", PresenterRaw * AdcConstant);
            sb.AppendFormat("Path Raw: {0} V DC\n",  PathRaw * AdcConstant);
            sb.AppendFormat("Paper Raw: {0} V DC\n", PaperRaw * AdcConstant);
            sb.AppendFormat("Notch Raw: {0} V DC\n", NotchRaw * AdcConstant);
            sb.AppendFormat("Arm Raw: {0} V DC\n", ArmRaw * AdcConstant);
            sb.AppendFormat("Ticket Status: {0} \n", TicketStatus);
            sb.AppendFormat("Errors Status: {0} \n", PrinterErrors);

            return sb.ToString();
        }

        /// <inheritdoc />
        public byte[] Serialize()
        {
            var payload = new List<byte>();

            // Must be exactly 5 chars + null term
            var temp = Encoding.ASCII.GetBytes(HeadVoltage);
            var voltage = new byte[6];
            Array.Copy(temp, 0, voltage, 0, Math.Min(temp.Length, voltage.Length));

            payload.AddRange(voltage);
            payload.Add(HeadTemp);
            payload.Add((byte)TicketStatus);
            payload.Add((byte)PrinterErrors);
            payload.Add((byte)SensorStatus);
            payload.AddRange(PresenterRaw.ToBytesBE());
            payload.AddRange(PathRaw.ToBytesBE());
            payload.AddRange(PaperRaw.ToBytesBE());
            payload.AddRange(NotchRaw.ToBytesBE());
            payload.AddRange(ArmRaw.ToBytesBE());

            return payload.ToArray();
        }
    };

    internal class StatusParser : BaseModelParser<Status>
    {
        public override Status Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.Count < 20)
            {
                return null;
            }

            var status = new Status();
            var payload = packet.GetBytes();

            // voltage is 6 bytes including null terminator
            var temp = new byte[5];
            Array.Copy(payload, 0, temp, 0, 5);
            status.HeadVoltage = temp.GetPrintableString();

            // Skip over the null terminator
            status.HeadTemp = payload[6];
            status.TicketStatus = (TicketStates)payload[7];
            status.PrinterErrors = (ErrorStatuses)payload[8];
            status.SensorStatus = (SensorStatuses)payload[9];

            temp = new byte[2];
            Array.Copy(payload, 10, temp, 0, 2);
            status.PresenterRaw = temp.ToUshortBE();

            Array.Copy(payload, 12, temp, 0, 2);
            status.PathRaw = temp.ToUshortBE();

            Array.Copy(payload, 14, temp, 0, 2);
            status.PaperRaw = temp.ToUshortBE();

            Array.Copy(payload, 16, temp, 0, 2);
            status.NotchRaw = temp.ToUshortBE();

            Array.Copy(payload, 18, temp, 0, 2);
            status.ArmRaw = temp.ToUshortBE();

            return status;
        }
    }
}