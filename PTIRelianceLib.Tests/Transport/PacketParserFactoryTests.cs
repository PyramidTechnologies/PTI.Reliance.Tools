using PTIRelianceLib.Transport;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Transport
{
    internal abstract class AbstractParseable : IParseable
    {
        public abstract byte[] Serialize();
    }

    internal class ConcreteParseable : AbstractParseable
    {
        public override byte[] Serialize() { return new byte[0]; }
    }

    internal abstract class AbstractConcreteParseableParser : IParseAs<ConcreteParseable>
    {
        public abstract ConcreteParseable Parse(IPacket packet);
    }

    internal class ConcreteParseableParser : IParseAs<ConcreteParseable>
    {
        public ConcreteParseable Parse(IPacket packet)
        {
            return new ConcreteParseable(); 
        }
    }

    public class PacketParserFactoryTests
    {
        [Fact]
        public void TestBadRegistration()
        {
            Assert.Throws<ArgumentException>(() =>
                PacketParserFactory.Instance.Register<AbstractParseable, ConcreteParseableParser>());

            Assert.Throws<ArgumentException>(() =>
                PacketParserFactory.Instance.Register<ConcreteParseable, AbstractConcreteParseableParser>());
        }
    }
}
