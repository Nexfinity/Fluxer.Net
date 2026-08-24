---
uid: Guides.CommandsPreconditions
title: Command Precondition
---

# Command Preconditions
Preconditions can be used to check for access or permissions when running a command.

## Standard

| Attribute | Description |
|---|---|
| [Require Owner](xref:Fluxer.Net.Commands.RequireOwnerAttribute) | Only the owner can run the command. |
| [Require Context](xref:Fluxer.Net.Commands.RequireContextAttribute) | Restrict command to DM, Group or Server. |
| [Require DM](xref:Fluxer.Net.Commands.RequireDMAttribute) | Restrict command to DM. |
| [Require Group](xref:Fluxer.Net.Commands.RequireGroupAttribute) | Restrict command to DM. |
| [Require Group Owner](xref:Fluxer.Net.Commands.RequireGroupOwnerAttribute) | Only the group owner can run the command. |

## Server

| Attribute | Description |
|---|---|
| [Require Server](xref:Fluxer.Net.Commands.RequireServerAttribute) | Restrict command to Server. |
| [Require Server Owner](xref:Fluxer.Net.Commands.RequireServerOwnerAttribute) | Only the community owner can run the command. |
| [Require Nsfw](xref:Fluxer.Net.Commands.RequireNsfwAttribute) | Restrict command to nsfw channels. |
| [Require Bot Permission](xref:Fluxer.Net.Commands.RequireBotPermissionAttribute) | Check if current user/bot has a permission. |
| [Require User Permission](xref:Fluxer.Net.Commands.RequireUserPermissionAttribute) | Check if command user has a permission. |

## Custom
You can create a custom precondition check, here is an example you can copy!

[!code-csharp[Precondition](../../example/samples/precondition.cs)]