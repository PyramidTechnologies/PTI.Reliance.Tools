---
uid: tut_intro
---
# Getting Started

This tutorial if for dotnetcore users. If you are trying to create a .NET Framework project, skip to the end of this page.

1. Install [.NET Core SDK](https://www.microsoft.com/net/download/windows) for your operating system
2. Open your favorite terminal
3. Setup Project and install library
```
dotnet new console -o reliance_sample 
cd reliance_sample
dotnet add package PTIRelianceLib
```
4. Open Program.cs in your favorite editor and replace the code with the following:
[!code-csharp[Main](Sample_01.cs)]

> [!IMPORTANT]
> Linux users, please make sure libusb is installed before proceeding

5. Execute
```sh
dotnet run
```

## Results
![Output](images/intro_01.gif)


# .NET Framework 4.6.1+ or other netstandard2.0 Framework

We recommend first creating a console application to get a feel for the library. Once this works, you can then create a GUI application (WPF, WinForms, etc.). This example assumes the latest version of Visual Studio is being used.

1. Ensure you have a compatible framework installed
2. Create a new C# desktop project, select Console
3. In the project settings, make sure .NET Framework 4.6.1+ is selected
4. Click create project
5. Once project is generated, add this library to your application

    Install-Package PTIRelianceLib
	
6. Copy the sample code from above into your application
7. Build, run, and done!