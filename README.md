[![NuGet](https://img.shields.io/nuget/v/PTIRelianceLib.svg?style=flat-square)](https://www.nuget.org/packages/PTIRelianceLib/)
[![Build status](https://ci.appveyor.com/api/projects/status/6s7woo3b9kp5khxw/branch/master?svg=true)](https://ci.appveyor.com/project/corytodd/pti-reliance-tools/branch/master)
# Reliance Tools API
This is a collection of tools for updating, configuring, and maintaining your Reliance Thermal Printer. We support netstandard2.0 which means you are free to use .NET Framework 4.6.1+, dotnetcore 2.0, or any of the other frameworks listed in the [.NET Standard Support Matrix](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

## Features
* Flash update firmware
* Status checks (paper status, ticket pull, sensor status, etc.)
* Logo printing
* Configuration

## Supported Architecture

We use a native library to access the Reliance hardware port. Here is the current list of supported architectures:

| OS | Architecture | Tested | Supported |
|----|--------------|--------|-----------|
| Windows | x86 | ✓ | ✓ |
| Windows | x64 | ✓| ✓ |
| Windows | ARM | | |
| Linux | x86 | | | 
| Linux | x64 | ✓ | ✓ |
| Linux | ARM32 | ✓ | ✓ |
| Linux | ARM64 | | |
| OSX | x64 | ✓ | |

Adding support for a new system requires compiling hidapi for your target and placing the binary in the runtimes directory of PTIRelianceLib. The [targets file](https://github.com/PyramidTechnologies/PTI.Reliance.Tools/blob/master/PTIRelianceLib/build/netstandard2.0/PTIRelianceLib.targets) must also be updated accordingly.

# Installation for dotnetcore

### Dotnet CLI

    dotnet add package PTIRelianceLib
	
# Installation for .NET Framework 4.6.1+

    Install-Package PTIRelianceLib

## Tutorials
See [the quick-start guide](http://developers.pyramidacceptors.com/PTI.Reliance.Tools/)

## Examples
- [RelianceCLI](RelianceCLI) is a traditional command line tool
- [ASP.NET Core](samples/reliance-asp-core-docker) is a bare-bones ASP.NET example that can be run inside a Docker container
- [Docker](samples/reliance--docker) is an example of a non-ASP.NET core application running inside a container. This example also includes the scripts and VS Code tasks required for debugging inside your container.
## Third Party Licenses
See [Third Party Licenses](third-party-license-readme.md)
