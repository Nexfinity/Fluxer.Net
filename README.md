# Squll.Net
Squll.Net is currently unsupported for any usecase, please report bug reports in the issues page, and open pull requests for missing endpoints. 

## Building 
to build nupkg:
```sh
dotnet pack --include-symbols --include-source
```

## Installing
Installing from @CottageDwellingCat's official myget feed:
add the following to the `<PropertyGroup>` tag that contains your app's configuration (prefer nuget to the private feed, but use it when packages are missing):
```xml
<RestoreSources>
    $(RestoreSources);https://api.nuget.org/v3/index.json;https://www.myget.org/F/nexfinity-alpha/api/v3/index.json
</RestoreSources>
```
then reference our package in the `<ItemGroup>` that contains your all of your `<PackageReference ... />` tags
```xml
<PackageReference Include="Nexfinity.Squll.Net.Alpha" Version="0.3.0" />
```
