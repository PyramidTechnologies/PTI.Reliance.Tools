# Getting Started

- Install [.NET Core](https://www.microsoft.com/net/download/windows) for your operating system
- Open your favorite terminal
- Execute
```
dotnet new console -o reliance_sample 
cd reliance_sample
dotnet add package PTIRelianceLib
```
- Open Program.cs in your favorite editor and replace the code with the following:
[!code-csharp[Main](Sample_01.cs)]
- Execute
```
dotnet run
```