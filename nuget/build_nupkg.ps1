# Build a nupkg

del *.nupkg
msbuild /t:DebugVars /t:pack /p:Configuration=Release /p:PackageOutputPath=$PSScriptRoot ..\PTIRelianceLib\PTIRelianceLib.csproj