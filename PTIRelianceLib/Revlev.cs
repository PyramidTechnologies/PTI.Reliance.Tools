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

    /// <inheritdoc cref="IEquatable" />
    /// <summary>
    /// Standard <see cref="T:PTIRelianceLib.IPyramidDevice" /> revision descriptor. A revision
    /// can be compared using fluent comparison operations such as Equals,
    /// Less Than, Greater Than, etc. Older firmware is considerd "less than" newer
    /// firmware using semantic versioning rules.
    /// </summary>
    public class Revlev : IComparable, IParseable, IEquatable<Revlev>
    {
        /// <summary>
        /// First component of version Major.Minor.Build
        /// </summary>
        /// <value>Major component of revision</value>
        public readonly int Major;

        /// <summary>
        /// Second component of version Major.Minor.Build
        /// </summary>
        /// <value>Minor component of revision</value>
        public readonly int Minor;

        /// <summary>
        /// Third component of version Major.Minor.Build
        /// </summary>
        /// <value>Build component of revision</value>
        public readonly int Build;

        /// <summary>
        /// Create a new Revlev object with default value of <c>0.0.0</c>.
        /// </summary>
        public Revlev()
            : this(0, 0, 0)
        { }

        /// <summary>
        /// Build a new <see cref="Revlev"/> type by parsing <paramref name="revlev"/> in the
        /// format <c>X.X.XX</c>.
        /// The minor and build fields may be omitted. In this case, the values will be set to 0.
        /// If a non-numeric or otheriwse invalid <paramref name="revlev"/> is provided, an exception
        /// of type <exception cref="ArgumentException"></exception> will be thrown.
        /// </summary>
        /// <param name="revlev">String of conforming format <c>X.X.XX</c></param>
        /// <exception cref="ArgumentException">Thrown if any value cannot be parsed as an integer</exception>
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
        /// Explicitly construct a new revlev in proper order of
        /// <paramref name="maj"/>.<paramref name="min"/>.<paramref name="build"/>.
        /// Only positive values will be stored.
        /// </summary>
        /// <param name="maj">First part of revision</param>
        /// <param name="min">Middle part of revision</param>
        /// <param name="build">Last part of revision</param>
        public Revlev(int maj, int min, int build)
        {
            Major = Math.Abs(maj);
            Minor = Math.Abs(min);
            Build = Math.Abs(build);
        }

        /// <summary>
        /// Returns this object in the format <c>X.X.XX</c>
        /// If the device is not conneted, the string "Not Connected"
        /// will instead be returned.
        /// </summary>
        /// <returns>string formatted as <c>X.X.XX</c> or "Not Connected"</returns>
        public override string ToString()
        {
            if (Major == 0 && Minor == 0 && Build == 0)
            {
                return "Not Connected";
            }
            return string.Format("{0}.{1}.{2}", Major, Minor, Build);
        }

        /// <inheritdoc />
        /// <value>Payload data</value>
        public byte[] Serialize()
        {
            var buff = new List<byte>();
            buff.AddRange(((ushort)Major).ToBytesBE());
            buff.AddRange(((ushort)Minor).ToBytesBE());
            buff.AddRange(Build.ToBytesBE());
            return buff.ToArray();
        }

        /// <inheritdoc />
        /// <summary>Compares this <see cref="Revlev"/> to another object <paramref name="obj"/></summary>
        /// <param name="obj">Object to compare</param>
        /// <value>
        /// <list type="bullet">
        /// <item><description> &lt; 0 if this &lt; obj</description></item>
        /// <item><description> 0 if this == obj</description></item>
        /// <item><description> &gt; 0 if this &gt; obj</description></item>
        /// </list>
        /// </value>
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
        /// Parses <paramref name="src"/> into an integer or throws an
        /// <exception cref="ArgumentException"></exception> exception on failure.
        /// </summary>
        /// <param name="src">Numeric <c>base-10</c> string</param>
        /// <returns>int</returns>
        private static int SafeInt(string src)
        {
            if (int.TryParse(src, out var temp))
            {
                return Math.Abs(temp);
            }

            throw new ArgumentException("revlev part is not an integer: {0}", src);
        }

        /// <inheritdoc />
        /// <summary>
        /// Compares this to another <see cref="T:Object" />.
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object other)
        {
            if (!(other is Revlev))
            {
                return false;
            }
            return CompareTo((Revlev)other) == 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// Compares this to another <see cref="T:PTIRelianceLib.Revlev" />.
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Revlev other)
        {
            return CompareTo(other) == 0;
        }
        
        /// <summary>
        /// Returns hashcode for this revision
        /// </summary>
        /// <returns>Integer hashcode</returns>
        /// <value>Hascode computed from revision fields</value>
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

        /// <summary>
        /// Returns true if <paramref name="r1"/> is &lt; <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator <(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) < 0;
        }
        /// <summary>
        /// Returns true if <paramref name="r1"/> is &gt; <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator >(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) > 0;
        }
        /// <summary>
        /// Returns true if <paramref name="r1"/> is &lt;= <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator <=(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) <= 0;
        }
        /// <summary>
        /// Returns true if <paramref name="r1"/> is &gt;= <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator >=(Revlev r1, Revlev r2)
        {
            return r1.CompareTo(r2) >= 0;
        }
        /// <summary>
        /// Returns true if <paramref name="r1"/> is == <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator ==(Revlev r1, Revlev r2)
        {
            if (ReferenceEquals(r1, r2))
            {
                return true;
            }
            if (r1 is null)
            {
                return false;
            }
            return r1.CompareTo(r2) == 0;
        }
        /// <summary>
        /// Returns true if <paramref name="r1"/> is != <paramref name="r2"/>
        /// </summary>
        /// <param name="r1">Left hand side</param>
        /// <param name="r2">Right hand side</param>
        /// <returns>bool</returns>
        /// <value>True if condition is true</value>
        public static bool operator !=(Revlev r1, Revlev r2)
        {
            if (!ReferenceEquals(r1, r2))
            {
                return true;
            }
            if (r1 is null)
            {
                return false;
            }
            return r1.CompareTo(r2) != 0;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Parser class for <see cref="T:PTIRelianceLib.Revlev" /> consumes an <see cref="T:PTIRelianceLib.Transport.IPacket" />
    /// and produces a <see cref="T:PTIRelianceLib.Revlev" />.
    /// </summary>
    internal class RevlevParser : BaseModelParser<Revlev>
    {
        /// <summary>
        /// Parses <paramref name="packet"/> into a <see cref="Revlev"/>. If
        /// <paramref name="packet"/> is malformed, the default <c>0.0.0</c> will
        /// be returned.
        /// </summary>
        /// <param name="packet">Packet to parse</param>
        /// <returns>Revlev instance of <c>0.0.0</c> if <paramref name="packet"/> is malformed</returns>
        public override Revlev Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return new Revlev();
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
