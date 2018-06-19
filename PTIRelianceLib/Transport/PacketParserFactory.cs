#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:08 AM
#endregion

namespace PTIRelianceLib.Transport
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Telemetry;

    /// <summary>
    /// Typesafe parser locator factory. These functions extended objects known to implement IParseAs.
    /// Some of the Getters require additional parameters in order to generator the correct parser.
    /// </summary>
    internal class PacketParserFactory
    {
        static PacketParserFactory()
        {
            Instance.Register<PacketedBool, PacketedBoolParser>();
            Instance.Register<PacketedByte, PacketedByteParser>();
            Instance.Register<PacketedShort, PacketedShortParser>();
            Instance.Register<PacketedInteger, PacketedIntegerParser>();
            Instance.Register<PacketedString, PacketedStringParser>();
            Instance.Register<Revlev, RevlevParser>();
            Instance.Register<RELSerialConfig, RELSerialConfigParser>();
            Instance.Register<RELFont, RELFontParser>();
            Instance.Register<RELBezel, RELBezelParser>();
            Instance.Register<XonXoffConfig, XonXoffConfigParser>();
            Instance.Register<ConfigRev, ConfigRevParser>();
            Instance.Register<Status, StatusParser>();
            Instance.Register<PowerupTelemetry, PowerupTelemetryParser>();
            Instance.Register<LifetimeTelemetry, LifetimeTelemetryParser>();
        }

        private PacketParserFactory() { }

        public static PacketParserFactory Instance { get; } = new PacketParserFactory();

        /// <summary>
        /// Holds parser return type as key, parser class type as value
        /// </summary>
        private static readonly Dictionary<Type, Type> MRegisteredType = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers a new parser-result pair in this factory.
        /// </summary>
        /// <typeparam name="T">Class to generate</typeparam>
        /// <typeparam name="TK">Parser class</typeparam>
        public void Register<T, TK>()
            where T : IParseable
            where TK : IParseAs<T>
        {
            var returnType = typeof(T);
            if (returnType.IsAbstract || returnType.IsInterface)
            {
                throw new ArgumentException("Parser: Cannot create instance of interface or abstract class");
            }

            var parserType = typeof(TK);
            if (parserType.IsAbstract || parserType.IsInterface)
            {
                throw new ArgumentException("ReturnType : Cannot create instance of interface or abstract class");
            }

            MRegisteredType.Add(returnType, parserType);
        }

        /// <summary>
        /// Create a new parser instance as required by the specified model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IParseAs<T> Create<T>(params object[] parameters)
            where T : IParseable
        {
            var shouldFind = typeof(T);
            return !MRegisteredType.TryGetValue(shouldFind, out var type)
                ? throw new InvalidOperationException(
                    $"Creator is not register. Add call to Instance.Register<{shouldFind},SOME_PARSER_CLASS>() to static PacketParserFactory()")
                : (IParseAs<T>)Activator.CreateInstance(type, parameters);
        }
    }

    /// <summary>
    /// Base parser performs standard packet sanitization checks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class BaseModelParser<T> : IParseAs<T> where T : IParseable
    {
        /// <summary>
        /// Checks that packet is non-null and returns the payload of packet
        /// </summary>
        /// <param name="packet">Packet to inspect</param>
        /// <returns>Payload of packet or <c>null</c> on error</returns>
        protected IPacket CheckPacket(IPacket packet)
        {
            if (packet == null)
            {
                return null;
            }

            return packet.IsPackaged ? packet.ExtractPayload() : packet;
        }

        public abstract T Parse(IPacket packet);
    }
}
