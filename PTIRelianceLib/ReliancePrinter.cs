namespace PTIRelianceLib
{
    public class ReliancePrinter : IPyramidDevice
    {
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;

        public ReturnCodes SendConfiguration(BinaryFile config)
        {
            throw new System.NotImplementedException();
        }

        public ReturnCodes FlashUpdateTarget(BinaryFile firmware)
        {
            throw new System.NotImplementedException();
        }

        public Revlev GetRevlev()
        {
            throw new System.NotImplementedException();
        }

        public string GetSerialNumber()
        {
            throw new System.NotImplementedException();
        }

        public ReturnCodes Ping()
        {
            throw new System.NotImplementedException();
        }
    }
}
