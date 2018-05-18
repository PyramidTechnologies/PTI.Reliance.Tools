#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:01 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.Collections.Generic;
    using Transport;

    public class Revlev : IComparable, IParseable, IEquatable<Revlev>
    {
        /// <summary>
        /// First component of version Major.Minor.Build
        /// </summary>
        public readonly int Major;

        /// <summary>
        /// Second component of version Major.Minor.Build
        /// </summary>
        public readonly int Minor;

        /// <summary>
        /// Third component of version Major.Minor.Build
        /// </summary>
        public readonly int Build;

        /// <inheritdoc />
        public Revlev()
            : this(0, 0, 0)
        { }

        /// <summary>
        /// Build a new revlev type by parsing a string in the format X.X.XX.
        /// The minor and build fields may be omitted. In this case, the values will be set to 0.
        /// </summary>
        /// <param name="revlev">String of conforming format</param>
        public Revlev(string revlev)
        {
            if (string.IsNullOrEmpty(revlev))
            {
                return;
            }

            var parts = revlev.Split('.');
            Major = SafeInt(parts[0]);
            if (parts.Length <= 1)
            {
                return;
            }
            Minor = SafeInt(parts[1]);
            if (parts.Length > 2)
            {
                Build = SafeInt(parts[2]);
            }
        }

        /// <summary>
        /// Explicitly construct a new revlev in proper order
        /// </summary>
        /// <param name="maj">First part of revision</param>
        /// <param name="min">Middle part of revision</param>
        /// <param name="build">Last part of revision</param>
        public Revlev(int maj, int min, int build)
        {
            Major = maj;
            Minor = min;
            Build = build;
        }

        /// <summary>
        /// Returns this object in the format X.X.XX
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Build);
        }

        public byte[] Serialize()
        {
            var buff = new List<byte>();
            buff.AddRange(Major.ToBytesBE());
            buff.AddRange(Minor.ToBytesBE());
            buff.AddRange(Build.ToBytesBE());
            return buff.ToArray();
        }

        /// <inheritdoc />
        /// <summary>
        /// Return a value less than zero if this object is less than obj
        /// Return zero if this object is equal to obj
        /// Return a value greater than zero if this object is more than obj
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>int</returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is Revlev otherRevlev))
            {
                throw new ArgumentException("Object is not a revlev");
            }

            // Common case
            if (Major == otherRevlev.Major &&
                Minor == otherRevlev.Minor &&
                Build == otherRevlev.Build)
            {
                return 0;
            }


            if (Major > otherRevlev.Major)
            {
                return 1;
            }
            if (Major < otherRevlev.Major)
            {
                return -1;
            }

            // Majors are equal
            if (Minor > otherRevlev.Minor)
            {
                return 1;
            }
            if (Minor < otherRevlev.Minor)
            {
                return -1;
            }

            // Minors are equal
            if (Build > otherRevlev.Build)
            {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// Parses src into an integer or throws an Argument exception on failure
        /// </summary>
        /// <param name="src">Numeric base-10 string</param>
        /// <returns>int</returns>
        private static int SafeInt(string src)
        {
            if (int.TryParse(src, out var temp))
            {
                return temp;
            }

            throw new ArgumentException("revlev part is not an integer: {0}", src);
        }

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            if (!(other is Revlev))
            {
                return false;
            }
            return CompareTo((Revlev)other) == 0;
        }

        /// <inheritdoc />
        public bool Equals(Revlev other)
        {
            return CompareTo(other) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Build;
                return hashCode;
            }
        }

        public static bool operator <(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) < 0;
        }
        public static bool operator >(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) > 0;
        }
        public static bool operator <=(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) <= 0;
        }
        public static bool operator >=(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) >= 0;
        }
        public static bool operator ==(Revlev r1, Revlev r2)
        {
            if (ReferenceEquals(r1, r2))
            {
                return true;
            }
            if (ReferenceEquals(r1, null))
            {
                return false;
            }
            return r1.CompareTo(r2) == 0;
        }
        public static bool operator !=(Revlev r1, Revlev r2)
        {
            if (!ReferenceEquals(r1, r2))
            {
                return true;
            }
            if (ReferenceEquals(r1, null))
            {
                return false;
            }
            return r1.CompareTo(r2) != 0;
        }

        public static Revlev From(int major, int minor, int build)
        {
            return new Revlev(major, minor, build);
        }
        public static Revlev From(string rev)
        {
            return new Revlev(rev);
        }
    }

    internal class RevlevParser : BaseModelParser<Revlev>
    {
        public override Revlev Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            if (packet.Count < 8)
            {
                return new Revlev();
            }

            var payload = packet.GetBytes();
            var temp = new byte[2];

            Array.Copy(payload, 0, temp, 0, 2);
            int maj = temp.ToUshortBE();

            Array.Copy(payload, 2, temp, 0, 2);
            int min = temp.ToUshortBE();

            temp = new byte[4];
            Array.Copy(payload, 4, temp, 0, 4);
            var build = (int) temp.ToUintBE();

            return new Revlev(maj, min, build);
        }
    }
}
