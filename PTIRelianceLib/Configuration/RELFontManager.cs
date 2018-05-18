#region Header
// RELFontManager.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 11:42 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.Collections.Generic;
    using Properties;

    /// <summary>
    /// Builder helper for Reliance Font Map files
    /// </summary>
    internal static class RELFontManager
    {
        /// <summary>
        /// Returns the list of codepages embedded in this library
        /// </summary>
        /// <returns></returns>
        public static IList<int> GetEmbeddedCodepageIds()
        {
            return new List<int>()
            {
                32,100,101,437,600,808,850,863,1252,4256
            };
        }

        /// <summary>
        /// Produces a list of codepage map data based on the provided list. All 
        /// items marked as instaled will be added to the resulting list.
        /// </summary>
        /// <param name="items">To use a use. Any item with the IsInstalled property set true will be included</param>
        /// <returns>List of BinaryFile</returns>
        public static IList<BinaryFile> MakeFileList(IList<int> items)
        {
            var result = new List<BinaryFile>();

            // Ugly but effective way to get the raw bytes for each codepage map
            foreach (var id in items)
            {
                switch (id)
                {
                    case 32:
                        result.Add(BinaryFile.From(Resources.cp_space));
                        break;
                    case 100:
                        result.Add(BinaryFile.From(Resources.pti_100));
                        break;
                    case 101:
                        result.Add(BinaryFile.From(Resources.pti_101));
                        break;
                    case 437:
                        result.Add(BinaryFile.From(Resources.cp_437));
                        break;
                    case 600:
                        result.Add(BinaryFile.From(Resources.cp_600));
                        break;
                    case 808:
                        result.Add(BinaryFile.From(Resources.cp_808));
                        break;
                    case 850:
                        result.Add(BinaryFile.From(Resources.cp_850));
                        break;
                    case 863:
                        result.Add(BinaryFile.From(Resources.cp_863));
                        break;
                    case 1252:
                        result.Add(BinaryFile.From(Resources.cp_1252));
                        break;
                    case 4256:
                        result.Add(BinaryFile.From(Resources.cp_4256));
                        break;
                    default:
                        var zeros = new byte[512];
                        result.Add(BinaryFile.From(zeros));
                        continue;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns latest font module as BinaryFile
        /// </summary>
        /// <returns>BinaryFile</returns>
        public static BinaryFile GetDefaultFont()
        {
            return BinaryFile.From(Properties.Resources.default_font);
        }
    }
}