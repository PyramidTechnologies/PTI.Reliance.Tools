# Logging
PTIRelianceLig uses a the framework agnostic logging library [LibLog]([https://github.com/damianh/LibLog). Integrating with your application is simple and we use NLog throughout the sample projects.

## NLog Setup
Install NLog
```sh
    dotnet add package NLog.Config
```

Add a config file named ```NLog.config``` to the root of **your** application. This is typically next to your csproj file. Here is a sample config file:
[!code-xml[Main](NLog.config.sample)]

## Other Frameworks
See [LibLog's sample directory](https://github.com/damianh/LibLog/tree/master/src)

