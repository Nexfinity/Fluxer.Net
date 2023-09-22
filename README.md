# Squll.Net
Squll.Net is currently unsupported for any usecase, please report bug reports in the issues page, and open pull requests for missing endpoints. 

## Building 
to build nupkg:
```sh
dotnet pack --include-symbols --include-source
```

## Installing


> [!WARNING]  
> This installation method will likely **not** be supported when squll leaves alpha. At the very least the package id will change.

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

## 🗃️ Versioning Guarantees

This library generally abides by [Semantic Versioning](https://semver.org). Packages are published in `MAJOR.MINOR.PATCH` version format.

### Patch component

An increment of the **PATCH** component always indicates that an internal-only change was made, generally a bugfix. These changes will not affect the public-facing API in any way, and are always guaranteed to be forward- and backwards-compatible with your codebase, any pre-compiled dependencies of your codebase.

### Minor component

An increment of the **MINOR** component indicates that some addition was made to the library,
and this addition is not backwards-compatible with prior versions.
However, Squll.Net **does not guarantee forward-compatibility** on minor additions.
In other words, we permit a limited set of breaking changes on a minor version bump.

Major version bumps generally indicate some major change to the library,
and as such we are hesitant to bump the major version for every minor addition to the library.

While we will never break the API (outside of interface changes) on minor builds,
we will occasionally need to break the ABI, by introducing parameters to a method to match changes upstream with Discord.
As such, a minor version increment may require you to recompile your code, and dependencies,
such as addons, may also need to be recompiled and republished on the newer version.
When a binary breaking change is made, the change will be noted in the release notes.

### Major component

An increment of the **MAJOR** component indicates that breaking changes have been made to the library;
consumers should check the release notes to determine what changes need to be made.

## 📚 Branches

### Release

Release branch. Upon release, patches will be pushed to these branches.
New NuGet releases will be tagged on this branch.

### Dev

Development branch, available on MyGet. This branch is what pull requests are targetted to.

### Feature/X

Branches that target Dev, adding new features. Feel free to explore these branches and give feedback where necessary.
