---
uid: tut_customization
---
# Library Customization

PTIRelianceLib is designed to support as many operating systems as possible. We try to support as much as possible with defaults but in some cases, there are outliers that we need to make special accommodations for without affecting other systems.

These options are accessible in the <xref:PTIRelianceLib.LibraryOptions> class and should be attached to the global <xref:PTIRelianceLib.Library>.

# Connection Failures
The most common issue users experience with hardware SDKs is with the initial connection. This can be due to driver issues, operating system configuration, or many other possibilities. In this library, there are two likely causes for your connection issue.

- 1 Incorrect native library version: We use HIDAPI to access the Reliance Printer as a HID USB device. Our nuget package installs the native library for Windows(x86 and x64), Linux x64, ans OSX x64. If you use a different CPU or operating system, your environment is not supported at this time. Contact us!
- 2 Your operating system is not propagating device information in a standard way. This is common in Docker containers.

If your system is supported and you've followed all the tutorials, read on to learn how to customize PTIRelianceLib for your environment.

# Options

<xref:PTIRelianceLib.LibraryOptions> provides following properties:

- HidCleanupDelayMs: Used to ensure operating system is given enough time to remove and create device handles
- HidReconnectDelayMs: Used to give the operating system more time to enumerate devices during reconnection
- HidFlushStructuresOnEnumError: When no devices are found during enumeration, this may indicate stale device data is being forwarded by your system. Set this flag to force receipt of fresh information.

# Relation to Flash Update
The Reliance Thermal Printer executes a reboot at the start and end of the flash update process. This generates USB disconnect and reconnect events which, as stated above, can be tricky on some systems. If you are having trouble flash updating for your system, take as look at some of the preconfigured settings available in <xref:PTIRelianceLib.LibraryOptions> such as DockerLinuxStretch.

[!include[<Exceptions>](<exceptions.md>)]