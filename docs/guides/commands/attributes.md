# Command Attributes

| Attribute | Description |
|---|---|
| [Command](xref:Fluxer.Net.Commands.CommandAttribute) | Mark a method as a command users can run with a name |
| [Name](xref:Fluxer.Net.Commands.NameAttribute) | Override the name of a command/group |
| [Alias](xref:Fluxer.Net.Commands.AliasAttribute) | Allows a command to have multiple names such as prune/purge/clean |
| [Group](xref:Fluxer.Net.Commands.GroupAttribute) | Group similar commands together such as dev/admin/mod |
| [Summary](xref:Fluxer.Net.Commands.SummaryAttribute) | Decorative description of the command |
| [Remarks](xref:Fluxer.Net.Commands.RemakrsAttribute) | Decorative info of the command |
| [Priority](xref:Fluxer.Net.Commands.PriorityAttribute) | Priority used when running a command with similar name |
| [Remainder](xref:Fluxer.Net.Commands.RemainderAttribute) | Use this on a command input to allow all text |
| [Don't Auto Load](xref:Fluxer.Net.Commands.DontAutoLoadAttribute) | Prevent a module from being loaded unless manually selected |
| [Don't Inject](xref:Fluxer.Net.Commands.DontInjectAttribute) | Prevent a property in the module from being used with dependency inject |

> [!NOTE]
> Precondition attributes are a special type that allows you to run code to check if a command can be run such as access or permissions.