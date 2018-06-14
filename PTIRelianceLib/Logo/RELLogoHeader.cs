#region Header
// RELLogoHeader.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:11 PM
#endregion

namespace PTIRelianceLib.Logo
{
    using System.Collections.Generic;
    using Transport;

    /// <inheritdoc />
    /// <summary>
    /// The logo header describes the location of a single logo
    /// </summary>
    internal class RELLogoHeader : IParseable
    {       
        /// <summary>
        /// External flash chip alignment
        /// 4096 bytes (4K)
        /// </summary>
        public static int Alignment = 0x1000;

        /// <summary>
        /// The index the is stored in the bank
        /// </summary>
        public byte Index;

        public RELLogoHeader()
        {
            Padding2 = 0;
            Padding1 = 0;
        }

        /// <summary>
        /// Name of logo that can be up to 10 bytes in length
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Address that start of the logo is. Start = top left of image.
        /// </summary>
        public uint StartAddr { get; set; }

        /// <summary>
        /// Size in bytes of the logo
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Width in bytes of the logo
        /// </summary>
        public ushort WidthBytes { get; set; }

        /// <summary>
        /// Height of the logo
        /// </summary>
        public ushort HeightDots { get; set; }

        /// <summary>
        /// Number of bytes for the left margin.
        /// </summary>
        public byte LeftMargin { get; set; }

        /// <summary>
        /// Struct packing
        /// </summary>
        private byte Padding1 { get; }

        /// <summary>
        /// Struct packing
        /// </summary>
        private ushort Padding2 { get; }

        /// <summary>
        /// For convenience, store a copy of the data this header
        /// is describing. This is not used in serialization.
        /// </summary>
        public byte[] LogoData { get; set; }

        /// <inheritdoc />
        public byte[] Serialize()
        {
            var dst = new List<byte> {Index};

            if (string.IsNullOrEmpty(Name))
            {
                Name = new string(' ', 10);
            }

            // Enforce 10 byte length, space pad, and set null terminator on the end
            if (Name.Length > 10)
            {
                Name = Name.Substring(0, 10);
            }

            Name = Name.PadRight(10, ' ');
            var buff = Name.ToCharArray();
            buff[9] = '\0';
            Name = new string(buff);
            dst.AddRange(System.Text.Encoding.ASCII.GetBytes(Name));

            dst.Add(0);
            dst.AddRange(StartAddr.ToBytesBE());
            dst.AddRange(Size.ToBytesBE());
            dst.AddRange(WidthBytes.ToBytesBE());
            dst.AddRange(HeightDots.ToBytesBE());
            dst.Add(LeftMargin);
            dst.Add(Padding1);
            dst.AddRange(Padding2.ToBytesBE());

            return dst.ToArray();
        }
    }
}