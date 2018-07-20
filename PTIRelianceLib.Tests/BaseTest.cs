namespace PTIRelianceLib.Tests
{
    using System.Reflection;

    public class BaseTest
    {
        private readonly Assembly _thisAsm;
        public BaseTest()
        {
            _thisAsm = Assembly.GetExecutingAssembly();
        }

        protected byte[] GetResource(string name)
        {
            var id = string.Format("PTIRelianceLib.Tests.Resources.{0}", name);
            var stream = _thisAsm.GetManifestResourceStream(id);
            var result = new byte[stream.Length];
            stream.Read(result, 0, result.Length);
            return result;
        }
    }
}
