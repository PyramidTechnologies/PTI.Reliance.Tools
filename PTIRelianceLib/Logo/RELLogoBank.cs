#region Header
// RELLogoBank.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:11 PM
#endregion

namespace PTIRelianceLib.Logo
{
    using System.Collections.Generic;
    using Imaging;

    public class RELLogoBank
    {      
        /// <summary>
        /// See logos.h in firmware
        /// #define LOGO_EXTERNAL_FLASH_START_0	(1966080)
        /// Logo 0 Start addr. Needs to be flash sector size aligned
        /// </summary>
        private const uint StartAddress = 0x001E0000;

        /// <summary>
        /// TODO make dynamic for reliance logo size
        /// </summary>
        private const int BankSize = 131072;

        /// <summary>
        /// Returns the total size in bytes of the logo bank
        /// </summary>
        /// <returns></returns>
        public int TotalBankSize => BankSize;

        /// <summary>
        /// Gets the BankSize in KB (1024 bytes)
        /// </summary>
        public int TotalBankSizeKb => BankSize / 1024;

        /// <summary>
        /// Builds and returns a logo header based on the current state of this bank. The current
        /// state of the logos will be stored, do not modify logos after calling this method else
        /// your changes will not be applied.
        /// </summary>
        /// <param name="logos">Ordered list of logos to add</param>        
        /// <returns>Liust of logo headers in same order as logos list</returns>
        internal IList<RELLogoHeader> MakeHeaders(IList<IPrintLogo> logos)
        {
            var nextAddr = StartAddress;

            var result = new List<RELLogoHeader>();

            byte index = 0;
            foreach (var logo in logos)
            {
                var header = new RELLogoHeader()
                {
                    Index = index++,
                    LeftMargin = 0,
                    Name = "".PadRight(10, ' '),
                    Size = (uint) logo.Dimensions.SizeInBytes,
                    HeightDots = (ushort) logo.Dimensions.Height,
                    WidthBytes = (ushort) logo.Dimensions.WidthBytes,
                    StartAddr = nextAddr,
                    LogoData = logo.ToBuffer(),
                };

                result.Add(header);
                nextAddr = (nextAddr + header.Size).RoundUp(RELLogoHeader.Alignment);
            }

            return result;
        }
    }
}