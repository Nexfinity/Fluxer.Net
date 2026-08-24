---
uid: Guides.CommandsAttributes
title: Command Attributes
---

# Command Attributes
Add attributes to your modules or commands to change how they run.

## Usage

| Attribute | Description |
|---|---|
| [Command](xref:Fluxer.Net.Commands.CommandAttribute) | Mark a method as a command users can run with a name. |
| [Name](xref:Fluxer.Net.Commands.NameAttribute) | Override the name of a command/group. |
| [Alias](xref:Fluxer.Net.Commands.AliasAttribute) | Allows a command to have multiple names such as prune/purge/clean. |
| [Group](xref:Fluxer.Net.Commands.GroupAttribute) | Group similar commands together such as dev/admin/mod. |
| [Priority](xref:Fluxer.Net.Commands.PriorityAttribute) | Priority used when running a command with similar name. |
| [Remainder](xref:Fluxer.Net.Commands.RemainderAttribute) | Use this on a command input to allow all text. |
| [Dont Auto Load](xref:Fluxer.Net.Commands.DontAutoLoadAttribute) | Prevent a module from being loaded unless manually selected. |
| [Dont Inject](xref:Fluxer.Net.Commands.DontInjectAttribute) | Prevent a property in the module from being used with dependency inject. |

## Cosmetic
These are cosmetic and don't have any function but can be referenced using [Command Info](xref:Fluxer.Net.Commands.CommandInfo)

| Attribute | Description |
|---|---|
| [Summary](xref:Fluxer.Net.Commands.SummaryAttribute) | Description of the command. |
| [Remarks](xref:Fluxer.Net.Commands.RemarksAttribute) | Note of the command. |
| [Docs](xref:Fluxer.Net.Commands.DocsAttribute) | Docs url of the command. |

> [!NOTE]
> Precondition attributes are a special type that allows you to run code to check if a command can be run such as access or permissions.

[Preconditions](xref:Guides.CommandsPreconditions)