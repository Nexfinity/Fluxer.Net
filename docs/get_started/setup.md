---
uid: GetStarted.Setup
title: Setup Project
---

# Create Project
Use Visual Studio to start making your Fluxer bot!

| | |
| -------- | ------- |
| Click Create Project | ![Install](images/select_create.png) |
| Create Console App | ![Install](images/select_project.png) |
| Enter a name for your bot | ![Install](images/project_name.png) |
| Configure the project with .NET 8 or higher and check Do not use top level | ![Install](images/project_config.png) |

## Project Code
You will need to update your code and install Fluxer.net

> [!NOTE]
> Change your Program class to look like this
> [!code-csharp[Program](samples/program.cs)]

| | |
| -------- | ------- |
| Right click your project in the sidebar and select Manage NuGet Packages | ![Install](images/project_packages.png) |
| Search for Fluxer.net and install the package | ![Install](images/install_package.png) |
| Update your code to use Fluxer.net with the example below | |

[!code-csharp[Program](../example/samples/client.cs)]