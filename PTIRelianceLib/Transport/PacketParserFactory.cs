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

    /// <summary>
    /// Typesafe parser locator factory. These functions extended objects known to implement IParseAs.
    /// Some of the Getters require additional parameters in order to generator the correct parser.
    /// </summary>
    internal class PacketParserFactory
    {
        public delegate int ParseIntPacket(IPacket packet);

        static PacketParserFactory()
        {
            Instance.Register<StringModel, RawASCIIParser>();
            Instance.Register<Revlev, RevlevParser>();
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

            try
            {
                MRegisteredType.Add(returnType, parserType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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

        /// <summary>
        /// Parses packets as an integer. Automatically detects best numberic
        /// type to use (byte, short, or int). Packet will be unpackaged if necessary.
        /// </summary>
        /// <param name="packet">Data to parse</param>
        /// <returns>Parsed integer or -1 on error</returns>
        public static int ParseInteger(IPacket packet)
        {         
            if (packet == null || packet.Count == 0)
            {
                return -1;
            }

            if (packet.IsPackaged)
            {
                packet = packet.ExtractPayload();
            }

            var data = packet.GetBytes();
            switch (data.Length)
            {
                case 0: return -1;
                case 1: return data[0];
                case 2: return (int) data.ToUintBE();
                case 4: return (int) data.ToUintBE();

                default: return -1;
            }
        }
    }
}
