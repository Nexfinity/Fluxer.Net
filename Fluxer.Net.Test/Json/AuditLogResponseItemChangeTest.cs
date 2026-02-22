using Fluxer.Net.Data.Responses;
using Fluxer.Net.Extensions;
using System.Text.Json;

namespace Fluxer.Net.Test.Json;

public class AuditLogResponseItemChangeTest
{
    private JsonSerializerOptions SerializerOptions;
    [SetUp]
    public void Setup()
    {
        SerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new AuditLogResponseItemChangeConverter()
            }
        };
    }

    [Test]
    public void TestNull()
    {
        const string testNull = """
            {
            "key": "test",
            "old_value": null,
            "new_value": null
            }
            """;
        const string testNullNothing = """
            {
            "key": "test",
            "old_value": null
            }
            """;
        const string testNothingNull = """
            {
            "key": "test",
            "new_value": null
            }
            """;
        const string testNothing = """
            {
            "key": "test"
            }
            """;

        var datNull = Deserialize<AuditLogResponseItemChangeBase>(testNull);
        var datNullNothing = Deserialize<AuditLogResponseItemChangeBase>(testNullNothing);
        var datNothingNull = Deserialize<AuditLogResponseItemChangeBase>(testNothingNull);
        var datNothing = Deserialize<AuditLogResponseItemChangeBase>(testNothing);

        Assert.That(datNull, Is.Not.Null);
        Assert.That(datNullNothing, Is.Not.Null);
        Assert.That(datNothingNull, Is.Not.Null);
        Assert.That(datNothing, Is.Not.Null);

        Assert.That(
            datNull.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChangeBase)));
        Assert.That(
            datNullNothing.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChangeBase)));
        Assert.That(
            datNothingNull.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChangeBase)));
        Assert.That(
            datNothing.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChangeBase)));


        Assert.That(datNull.OldValue, Is.Null);
        Assert.That(datNullNothing.OldValue, Is.Null);
        Assert.That(datNothingNull.OldValue, Is.Null);
        Assert.That(datNothing.OldValue, Is.Null);

        Assert.That(datNull.NewValue, Is.Null);
        Assert.That(datNullNothing.NewValue, Is.Null);
        Assert.That(datNothingNull.NewValue, Is.Null);
        Assert.That(datNothing.NewValue, Is.Null);
    }
    [Test]
    public void TestString()
    {
        const string testNullThenString = """
            {
            "key": "test",
            "old_value": null,
            "new_value": "new value!"
            }
            """;
        const string testStringThenNull = """
            {
            "key": "test",
            "old_value": "old value!",
            "new_value": null
            }
            """;
        const string testStringString = """
            {
            "key": "test",
            "old_value": "old value!",
            "new_value": "new value!"
            }
            """;

        var datNullThenString = Deserialize<AuditLogResponseItemChangeBase>(testNullThenString);
        var datStringThenNull = Deserialize<AuditLogResponseItemChangeBase>(testStringThenNull);
        var datStringString = Deserialize<AuditLogResponseItemChangeBase>(testStringString);

        Assert.That(datNullThenString, Is.Not.Null);
        Assert.That(datStringThenNull, Is.Not.Null);
        Assert.That(datStringString, Is.Not.Null);

        Assert.That(
            datNullThenString.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChange<string?>)));

        Assert.That(
            datStringThenNull.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChange<string?>)));
        Assert.That(
            datStringString.GetType(),
            Is.EqualTo(typeof(AuditLogResponseItemChange<string?>)));

        Assert.That(datNullThenString.Key, Is.EqualTo("test"));
        Assert.That(datNullThenString.OldValue, Is.Null);
        Assert.That(datNullThenString.NewValue, Is.EqualTo("new value!"));

        Assert.That(datStringThenNull.Key, Is.EqualTo("test"));
        Assert.That(datStringThenNull.OldValue, Is.EqualTo("old value!"));
        Assert.That(datStringThenNull.NewValue, Is.Null);

        Assert.That(datStringString.Key, Is.EqualTo("test"));
        Assert.That(datStringString.OldValue, Is.EqualTo("old value!"));
        Assert.That(datStringString.NewValue, Is.EqualTo("new value!"));
    }

    [Test]
    public void TestPermissionDiffSchema()
    {
        const string addOnly = """
            {
                "key": "permissions_diff",
                "new_value": {
                    "added": [
                        "ADMINISTRATOR",
                        "MANAGE_CHANNELS",
                        "MANAGE_GUILD",
                        "VIEW_AUDIT_LOG",
                        "MANAGE_ROLES"
                    ],
                    "removed": []
                }
            }
            """;
        const string removeOnly = """
            {
                "key": "permissions_diff",
                "new_value": {
                    "added": [],
                    "removed": [
                        "MANAGE_CHANNELS"
                    ]
                }
            }
            """;
        const string addAndRemove = """
            {
                "key": "permissions_diff",
                "new_value": {
                    "added": [
                        "MANAGE_CHANNELS"
                    ],
                    "removed": [
                        "MANAGE_ROLES"
                    ]
                }
            }
            """;

        var dataAddOnly = Deserialize<AuditLogResponseItemChangeBase>(addOnly);
        var dataRemoveOnly = Deserialize<AuditLogResponseItemChangeBase>(removeOnly);
        var dataAddAndRemove = Deserialize<AuditLogResponseItemChangeBase>(addAndRemove);

        Assert.That(dataAddOnly, Is.Not.Null);
        Assert.That(dataRemoveOnly, Is.Not.Null);
        Assert.That(dataAddAndRemove, Is.Not.Null);

        Assert.That(dataAddOnly.GetType(), Is.EqualTo(typeof(AuditLogResponseItemChange<PermissionDiffSchema?>)));
        Assert.That(dataRemoveOnly.GetType(), Is.EqualTo(typeof(AuditLogResponseItemChange<PermissionDiffSchema?>)));
        Assert.That(dataAddAndRemove.GetType(), Is.EqualTo(typeof(AuditLogResponseItemChange<PermissionDiffSchema?>)));

        var typedAddOnly = (AuditLogResponseItemChange<PermissionDiffSchema>)dataAddOnly;
        var typedRemoveOnly = (AuditLogResponseItemChange<PermissionDiffSchema>)dataRemoveOnly;
        var typedAddAndRemove = (AuditLogResponseItemChange<PermissionDiffSchema>)dataAddAndRemove;

        Assert.That(typedAddOnly.Key, Is.EqualTo("permissions_diff"));
        Assert.That(typedAddOnly.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(typedAddOnly.NewValue, Is.Not.Null);
        Assert.That(typedAddOnly.NewValue.Added.Count, Is.EqualTo(5));
        Assert.That(typedAddOnly.NewValue.Added, Has.Some.EqualTo("ADMINISTRATOR"));
        Assert.That(typedAddOnly.NewValue.Added, Has.Some.EqualTo("MANAGE_CHANNELS"));
        Assert.That(typedAddOnly.NewValue.Added, Has.Some.EqualTo("MANAGE_GUILD"));
        Assert.That(typedAddOnly.NewValue.Added, Has.Some.EqualTo("VIEW_AUDIT_LOG"));
        Assert.That(typedAddOnly.NewValue.Added, Has.Some.EqualTo("MANAGE_ROLES"));
        Assert.That(typedAddOnly.NewValue.Removed.Count, Is.EqualTo(0));

        Assert.That(typedRemoveOnly.Key, Is.EqualTo("permissions_diff"));
        Assert.That(typedRemoveOnly.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(typedRemoveOnly.NewValue, Is.Not.Null);
        Assert.That(typedRemoveOnly.NewValue.Added.Count, Is.EqualTo(0));
        Assert.That(typedRemoveOnly.NewValue.Removed.Count, Is.EqualTo(1));
        Assert.That(typedRemoveOnly.NewValue.Removed, Has.Some.EqualTo("MANAGE_CHANNELS"));

        Assert.That(typedAddAndRemove.Key, Is.EqualTo("permissions_diff"));
        Assert.That(typedAddAndRemove.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(typedAddAndRemove.NewValue, Is.Not.Null);
        Assert.That(typedAddAndRemove.NewValue.Added.Count, Is.EqualTo(1));
        Assert.That(typedAddAndRemove.NewValue.Removed.Count, Is.EqualTo(1));
        Assert.That(typedAddAndRemove.NewValue.Added, Has.Some.EqualTo("MANAGE_CHANNELS"));
        Assert.That(typedAddAndRemove.NewValue.Removed, Has.Some.EqualTo("MANAGE_ROLES"));
    }

    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }
}
