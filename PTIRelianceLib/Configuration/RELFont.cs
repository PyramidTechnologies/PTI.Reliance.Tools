#region Header
// RELFont.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 9:44 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System;
    using PTIRelianceLib.Transport;

    /// <inheritdoc cref="IParseable" />
    internal class RELFont : IParseable
    {
        /// <summary>
        /// Fontsize code
        /// </summary>
        public RelianceFontSizes FontSize;
        /// <summary>
        /// Which font size is active
        /// </summary>
        public char FontWhich;
        /// <summary>
        /// Which codepage is active
        /// </summary>
        public ushort CodePage;

        public byte[] Serialize()
        {
            var buff = new byte[4];
            buff[0] = (byte) FontSize;
            buff[1] = (byte) FontWhich;
            var temp = CodePage.ToBytesBE();
            Array.Copy(temp, 0, buff, 2, 2);
            return buff;
        }
 
        /// <summary>
        /// Default - A11, B12 Font A 771 codepage
        /// </summary>
        public static RELFont Default => new RELFont()
        {
            FontSize = RelianceFontSizes.A11B15,
            FontWhich = 'A',
            CodePage = 771,
        };
    }

    internal class RELFontParser : BaseModelParser<RELFont>
    {

        public override RELFont Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            var data = packet.GetBytes();

            if (data.Length < 3)
            {
                return null;
            }

            var font = new RELFont
            {
                FontSize = (RelianceFontSizes)data[0],
                FontWhich = (char)data[1],
            };

            var temp = new byte[2];
            Array.Copy(data, 2, temp, 0, 2);
            font.CodePage = temp.ToUshortBE();

            return font;
        }
    }
}