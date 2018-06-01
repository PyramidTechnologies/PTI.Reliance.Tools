del build\*.nupkg
dotnet pack -c Debug -o ..\build /p:Platform=x64 --include-symbols --include-source PTIRelianceLib\PTIRelianceLib.csproj

# Either myself or nuget are too stupid to figure out how to easily push my package + symbols
# List the files in PTIRelianceLib\build, there should be exactly two.
# Once is the actual library and the other is the symbols. Push just the nuget.

Get-ChildItem $path -recurse | where {$_.name -notlike "*symbols*"} | % {
 #nuget push $_.FullName -Source https://api.nuget.org/v3/index.json
}