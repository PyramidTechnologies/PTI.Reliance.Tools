using Xunit;

namespace PTIRelianceLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RevlevTests
    {
        [Fact]
        public void RevlevTestEquals()
        {
            var rev1 = new Revlev("1.2.3");
            var rev2 = new Revlev("1.2.3");
            var rev3 = new Revlev("2.3.4");

            Assert.Equal(rev1, rev2);
            Assert.NotEqual(rev1, rev3);
        }

        [Fact]
        public void RevlevTestCompares()
        {
            var rev1 = new Revlev("1.2.3");
            var rev2 = new Revlev("2.2.2");
            var rev3 = new Revlev("2.2.3");
            var rev4 = new Revlev("1.16.145");
            var rev5 = new Revlev("1.19.146");

            Assert.True(rev1 < rev2);
            Assert.True(rev2 < rev3);
            Assert.True(rev4 < rev5);
            Assert.True(rev3 > rev1);
            Assert.True(rev5 > rev4);
        }

        [Fact]
        public void RevlevTestEqutable()
        {
            var rev1 = new Revlev("2.12.30");
            var rev2 = new Revlev("1.17.147");
            var rev3 = new Revlev("1.17.147");
            var rev4 = new Revlev("1.17.148");

            var list = new List<Revlev>
            {
                rev1,
                rev2,
                rev3
            };
            Assert.Contains(rev1, list);
            Assert.Contains(rev2, list);
            Assert.Contains(rev3, list);
            Assert.DoesNotContain(rev4, list);
        }


        [Fact]
        public void RevlevTestCmpOperators()
        {
            var rev0 = new Revlev(0, 0, 0);
            var rev1 = new Revlev();
            var rev2 = new Revlev("1");
            var rev3 = new Revlev("1.2");
            var rev4 = new Revlev("1.2.3");
            var rev5 = new Revlev("2.3");
            var rev6 = new Revlev("2.3.4");
            var rev7 = new Revlev("0.1.2");
            var rev8 = new Revlev("4.3.2");
            var rev9 = new Revlev("1.2.0");

            Assert.True(rev0.Equals((object) rev1));

            Assert.True(rev0 == rev0);
            Assert.False(null == rev0);
            Assert.False(rev0 == null);

            Assert.False(rev0 != rev0);
            Assert.False(null as Revlev != null as Revlev);
            Assert.True(rev0 != null);

            Assert.Equal(rev0, rev1);
            Assert.True(rev1 < rev2);
            Assert.True(rev1 < rev3);
            Assert.True(rev1 < rev4);
            Assert.True(rev1 < rev5);
            Assert.True(rev1 < rev6);
            Assert.True(rev1 < rev7);
            Assert.True(rev1 < rev8);

            Assert.True(rev4 > rev0);
            Assert.True(rev4 > rev1);
            Assert.True(rev4 > rev2);
            Assert.True(rev4 > rev3);

            Assert.True(rev4 >= rev3);
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(rev4 >= rev4);
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.True(rev3 <= rev4);
            Assert.True(rev3 <= rev5);
            Assert.True(rev3 < rev8);

            Assert.True(rev3 == rev9);
            Assert.True(rev0 <= rev1);
            Assert.True(rev0 <= rev1);
            Assert.True(rev1 == rev0);
            Assert.True(rev2 != rev1);
            Assert.True(rev4 > rev3);
            Assert.True(rev5 > rev4);
            Assert.True(rev7 <= rev8);
            Assert.True(rev2 >= rev1);
            Assert.True(rev4 != rev2);

            Assert.False(rev4 == null);
            Assert.True(rev4 != null);
            Assert.True(rev4 > null);
            Assert.True(rev4 >= null);
            Assert.False(rev4 < null);
            Assert.False(rev4 <= null);
        }

        [Fact]
        public void RevlevCompareNull()
        {
            object revnull = null;
            var rev = new Revlev();
            Assert.False(rev.Equals(revnull));
            Assert.True(rev != revnull);
        }

        [Fact]
        public void RevlevTestMalformed()
        {
            var data = new[]
            {
                "1[", "1,9", "1.h2.3",
            };

            foreach (var str in data)
            {
                Assert.Throws<ArgumentException>(() => new Revlev(str));
            }
        }

        [Fact]
        public void RevlevTestCompareNotRevlev()
        {
            var revlev = new Revlev("1.2.5");
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.Throws<ArgumentException>(() => revlev.CompareTo(new DateTime()));
        }

        [Fact]
        public void RevlevTestHashCode()
        {
            var a = new Revlev();
            var b = new Revlev("1.0.0");
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());

            var c = new Revlev();
            Assert.Equal(a.GetHashCode(), c.GetHashCode());
        }

        [Fact]
        public void RevlevTestToString()
        {
            var data = new List<Tuple<Revlev, string>>
            {
                new Tuple<Revlev, string>(new Revlev(string.Empty), "0.0.0"),
                new Tuple<Revlev, string>(new Revlev(null), "0.0.0"),
                new Tuple<Revlev, string>(new Revlev(), "0.0.0"),
                new Tuple<Revlev, string>(new Revlev("1"), "1.0.0"),
                new Tuple<Revlev, string>(new Revlev("1.2"), "1.2.0"),
                new Tuple<Revlev, string>(new Revlev("1.2.3"), "1.2.3"),
                new Tuple<Revlev, string>(new Revlev(1, 2, 4), "1.2.4"),
            };

            foreach (var kv in data)
            {
                Assert.Equal(kv.Item2, kv.Item1.ToString());
            }           
        }

        [Fact]
        public void RevlevTestSerialize()
        {
            var data = new List<Tuple<Revlev, byte[]>>
            {
                new Tuple<Revlev, byte[]>(new Revlev(), new byte[] {0, 0, 0, 0, 0, 0, 0, 0}),
                new Tuple<Revlev, byte[]>(new Revlev(1, 2, 3), new byte[] {1, 0, 2, 0, 3, 0, 0, 0}),
                new Tuple<Revlev, byte[]>(new Revlev(0x1234, 0x4567, 0x22334455), 
                    new byte[] {0x34, 0x12, 0x67, 0x45, 0x55, 0x44, 0x33, 0x22})
            };

            foreach (var tuple in data)
            {
                Assert.Equal(tuple.Item2, tuple.Item1.Serialize());
            }
        }
    }
}
