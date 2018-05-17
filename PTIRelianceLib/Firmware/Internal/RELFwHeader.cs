#region Header
// RELFwHeader.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:15 AM
#endregion

namespace PTIRelianceLib.Firmware.Internal
{
    using System;

    /// <summary>
    /// Contains the header information encoded in a Reliance PTIX binary image
    /// </summary>
    internal struct RELFwHeader
    {
        /// <summary>
        /// Identity matrix
        /// </summary>
        public byte[] IdMatrix;		         
        
        /// <summary>
        /// Magix number for file system
        /// </summary>                           
        public byte[] MagicNum;	            

        /// <summary>
        /// Original size of file in bytes e
        /// excluding header
        /// </summary>
        public ulong OriginalSize;              

        /// <summary>
        /// Starting address to where this
        /// image should be loaded
        /// </summary>
        public uint StartAddr;	  
      
        /// <summary>
        /// Algorithm version for PTIX encoding
        /// </summary>
        public uint Algorithm;	            

        /// <summary>
        /// Firmware model code
        /// </summary>
        public uint Model;		
        
        /// <summary>
        /// Expected CRC32 checksum for image
        /// </summary>
        public uint ExpectedCsum;		     
   
        /// <summary>
        /// Original size of firmware in bytes
        /// This is the entire file including header
        /// </summary>
        public uint RawFileSize;                    
        
        /// <summary>
        /// Flag indicates if this header and payload
        /// are valid.
        /// </summary>                    
        public bool IsValid;                    

        /// <summary>
        /// Assignes the ID matrix
        /// </summary>
        /// <param name="id">Identify matrix</param>
        public void SetIdMat(byte[] id)
        {
            IdMatrix = new byte[id.Length];
            Array.Copy(id, IdMatrix, id.Length);
        }

        /// <summary>
        /// Assign the magic number value
        /// </summary>
        /// <param name="d">magic number</param>
        public void SetMagicNumber(byte[] d)
        {
            MagicNum = new byte[d.Length];
            Array.Copy(d, MagicNum, d.Length);
        }
    }
}