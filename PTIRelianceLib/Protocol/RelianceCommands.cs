#region Header
// RelianceCommands.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 10:06 AM
#endregion

namespace PTIRelianceLib.Protocol
{
    internal enum RelianceCommands
    {
        /// <summary>
        /// Request permission to flash update
        /// </summary>
        FlashRequest = 0x35,
        /// <summary>
        /// Execute flash update process
        /// </summary>
        FlashDo = 0x55,
        /// <summary>
        /// Ping target for ACK
        /// </summary>
        Ping = 0x75,
        /// <summary>
        /// Request expected checksum
        /// </summary>
        GetExpectedCsum = 0x85,
        /// <summary>
        /// Request actual checksum
        /// </summary>
        GetActualCsum = 0x95,
        /// <summary>
        /// Request version string of target
        /// </summary>
        GetRevlev = 0x15,
        /// <summary>
        /// Hard reset target
        /// </summary>
        Reboot = 0x25,
        /// <summary>
        /// Get the unique ID for target (hardware based)
        /// </summary>
        GetUniqueId = 0x66,
        /// <summary>
        /// Get the ID of the current bootlaoder
        /// </summary>
        GetBootId = 0x57,
        /// <summary>
        /// Get the CRC Ranges that should be checked
        /// </summary>
        GetCRCRanges = 0x37,
        /// <summary>
        /// Get the communication timeout for target. This is the PC communication
        /// timeout over HID.
        /// </summary>
        GetCommTimeout = 0x67,
        /// <summary>
        /// Set RAM flags to boot into target application
        /// </summary>
        SetBootMode = 0x59,
        /// <summary>
        /// Returns the RS-232 serial configuration
        /// </summary>
        GetSerialConfig = 0x40,
        /// <summary>
        /// Sets the RS-232 serial configuration
        /// </summary>
		SetSerialConfig = 0x41,
        /// <summary>
        /// Returns the print quality encoding
        /// </summary>
		GetPrintQuality = 0x42,
        /// <summary>
        /// Sets the print quality encoding
        /// </summary>
		SetPrintQuality = 0x43,
        /// <summary>
        /// Returns the retraction enabled state
        /// </summary>
		GetRetractEnabled = 0x44,
        /// <summary>
        /// Sets the retraction enabled state
        /// </summary>
		SetRetractEnabled = 0x62,
        /// <summary>
        /// Returns the ejector mode
        /// </summary>
		GetEjectorMode = 0x46,
        /// <summary>
        /// Sets the ejector mode
        /// </summary>
		SetEjectorMode = 0x47,
        /// <summary>
        /// Returns the present length in mm
        /// </summary>
		GetPresentLen = 0x48,
        /// <summary>
        /// Sets the present length in mm
        /// </summary>
		SetPresentLen = 0x49,
        /// <summary>
        /// Returns the CRLF (carriage return) config
        /// </summary>
		GetCRLFConf = 0x4A,
        /// <summary>
        /// Sets the CRLF (carriage return) config
        /// </summary>
		SetCRLFConf = 0x4B,
        /// <summary>
        /// Returns the action taken when a ticket timeout occurs
        /// </summary>
		GetTimeoutAction = 0x4C,
        /// <summary>
        /// Sets the action taken when a ticket timeout occurs
        /// </summary>
		SetTimeoutAction = 0x4D,
        /// <summary>
        /// Returns the action taken when a new ticket arrives
        /// </summary>
		GetNewTicketAction = 0x4E,
        /// <summary>
        /// Sets the action taken when a new ticket arrives
        /// </summary>
		SetNewTicketAction = 0x4F,
        /// <summary>
        /// Returns the serial number of the target
        /// </summary>
		GetSerialNumber = 0x50,
        /// <summary>
        /// Sets the serial number of the target
        /// </summary>
		SetSerialNumber = 0x51,
        /// <summary>
        /// Saves targets current configuraton to nvflash
        /// </summary>
		SaveConfig = 0x52,
        /// <summary>
        /// Returns the current print density
        /// </summary>
		GetPrintDensity = 0x53,
        /// <summary>
        /// Sets the current print density
        /// </summary>
		SetPrintDensity = 0x54,
        /// <summary>
        /// Returns the ticket timeout period
        /// </summary>
		GetTicketTimeoutPeriod = 0x60,
        /// <summary>
        /// Sets the ticket timeout period
        /// </summary>
		SetTicketTimeoutPeriod = 0x61,
        /// <summary>
        /// Sets the friendly name of the printer
        /// </summary>
		SetFriendlyName = 0x63,
        /// <summary>
        /// Gets the friendly name of the printer
        /// </summary>
		GetFriendlyName = 0x64,
        /// <summary>
        /// Reserved
        /// </summary>
		Reserved1 = 0x69,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved2 = 0x70,
        /// <summary>
        /// Returns the printers real-time status
        /// </summary>
		GetPrinterStatus = 0x71,
        /// <summary>
        /// Subcommand for RTC operations
        /// </summary>
		RTCSub = 0x73,
        /// <summary>
        /// Subcmmand for PCB operations
        /// </summary>
		PCBSub = 0x78,
        /// <summary>
        /// Request to write general data to flash (not firmware image)
        /// </summary>
		DataWriteRequest = 0x79,
        /// <summary>
        /// Subcommand for logos
        /// </summary>
		LogoSub = 0x80,
        /// <summary>
        /// Print a page containing current codepage
        /// </summary>
		PrintCodepage = 0x81,
        /// <summary>
        /// Subcommand for font config
        /// </summary>
		FontSub = 0x82,
        /// <summary>
        /// Subcommand for paper size
        /// </summary>
		PaperSizeSub = 0x83,
        /// <summary>
        /// Subcommand for Logs
        /// </summary>
		LogSub = 0x84,
        /// <summary>
        /// Subcommand for bezel
        /// </summary>
		BezelSub = 0x85,
        /// <summary>
        /// Subcommand for autocut settings
        /// </summary>
		AutocutSub = 0x88,
        /// <summary>
        /// Perform general configuration
        /// <see cref="GeneralConfigCodes"/>
        /// </summary>
        GeneralConfigSub = 0x91,
        /// <summary>
        /// Prints an empty (no-text) ticket
        /// </summary>
        PrintBlankTicket = 0x92,
        /// <summary>
        /// Returns the length of paper moved
        /// </summary>
        GetPaperMoved = 0x93,
        /// <summary>
        /// No command set (reserved)
        /// </summary>
		None = 0xFF,
    }

    internal enum GeneralConfigCodes
    {
        /// <summary>
        /// Sets the stepper motor version. This is used
        /// to control the low-level stepper movement. For
        /// example, the original motor is a 15° and the second
        /// is a 7° which means the later requires twice as many
        /// steps for the same radial distance. That's why this must
        /// be exposed as a configurable.
        /// </summary>
        SetStepperMotor = 0x00,
        /// <summary>
        /// Returns the stepper motor code
        /// </summary>
        GetStepperMotor = 0x01,
        /// <summary>
        /// Sets the paper slack compensation flag
        /// </summary>
        SetPaperSlackComp = 0x02,
        /// <summary>
        /// Gets the paper slack compenstation flag
        /// </summary>
        GetPaperSlackComp = 0x03,
        /// <summary>
        /// Sets CDC enabled or disabled
        /// </summary>
        SetCDCEnable = 0x04,
        /// <summary>
        /// Get sthe CDC enabled state
        /// </summary>
        GetCDCEnable = 0x05,
        /// <summary>
        /// Set the USB unique serial number feature
        /// </summary>
        SetUSBSerialUnique = 0x06,
        /// <summary>
        /// Gets the USB unique serial numnber feature
        /// </summary>
        GetUSBSerialUnique = 0x07,
        /// <summary>
        /// Sets the stepper motor current
        /// <since>1.15</since>
        /// </summary>
        SetStepperMotorCurrent = 0x08,
        /// <summary>
        /// Gets the stepper motor current
        /// <since>1.15</since>
        /// </summary>
        GetStepperMotorCurrent = 0x09,
        /// <summary>
        /// Sets the XonXoff control code
        /// </summary>
        SetXonXoffCodes = 0x0A,
        /// <summary>
        /// Gets the XonXoff control codes
        /// </summary>
        GetXonXoffCodes = 0x0B,
        /// <summary>
        /// Sets the startup ticket enabled feature
        /// </summary>
        SetStartupTicketEnabled = 0x0C,
        /// <summary>
        /// Gets the startup ticket enabled feature
        /// </summary>
        GetStartupTicketEnabled = 0x0D,
        /// <summary>
        /// Sets the configuration file revision
        /// <since>1.21</since>
        /// </summary>
        SetConfigFileRev = 0x0E,
        /// <summary>
        /// Sets the configuration file revision
        /// <since>1.21</since>
        /// </summary>
        GetConfigFileRev = 0x0F,
        /// <summary>
        /// Sets the truncation enabled feature state
        /// </summary>
        SetTruncationEnabled = 0x10,
        /// <summary>
        /// Gets the truncation enabled feature state
        /// </summary>
        GetTruncationEnabled = 0x11,
    }
}