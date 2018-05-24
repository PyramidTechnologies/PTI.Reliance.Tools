# Build a bupkg

del *.nupkg
msbuild /t:pack /p:Configuration=Release /p:PackageOutputPath=$PSScriptRoot ..\PTIRelianceLib\PTIRelianceLib.csproj