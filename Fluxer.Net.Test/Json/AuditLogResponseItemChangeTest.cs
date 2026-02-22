using Fluxer.Net.Data.Responses;
using Newtonsoft.Json;

namespace Fluxer.Net.Test.Json;

public class AuditLogResponseItemChangeTest
{
    [SetUp]
    public void Setup()
    {
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

        var datNull = Deserialize<AuditLogResponseItemChange>(testNull);
        var datNullNothing = Deserialize<AuditLogResponseItemChange>(testNullNothing);
        var datNothingNull = Deserialize<AuditLogResponseItemChange>(testNothingNull);
        var datNothing = Deserialize<AuditLogResponseItemChange>(testNothing);

        Assert.That(datNull, Is.Not.Null);
        Assert.That(datNullNothing, Is.Not.Null);
        Assert.That(datNothingNull, Is.Not.Null);
        Assert.That(datNothing, Is.Not.Null);

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

        var datNullThenString = Deserialize<AuditLogResponseItemChange>(testNullThenString);
        var datStringThenNull = Deserialize<AuditLogResponseItemChange>(testStringThenNull);
        var datStringString = Deserialize<AuditLogResponseItemChange>(testStringString);

        Assert.That(datNullThenString, Is.Not.Null);
        Assert.That(datStringThenNull, Is.Not.Null);
        Assert.That(datStringString, Is.Not.Null);

        Assert.That(datNullThenString.Key, Is.EqualTo("test"));
        Assert.That(datNullThenString.OldValue, Is.Null);
        Assert.That(datNullThenString.NewValue?.GetType(), Is.EqualTo(typeof(string)));
        Assert.That(datNullThenString.NewValue, Is.EqualTo("new value!"));

        Assert.That(datStringThenNull.Key, Is.EqualTo("test"));
        Assert.That(datStringThenNull.OldValue?.GetType(), Is.EqualTo(typeof(string)));
        Assert.That(datStringThenNull.OldValue, Is.EqualTo("old value!"));
        Assert.That(datStringThenNull.NewValue, Is.Null);

        Assert.That(datStringString.Key, Is.EqualTo("test"));
        Assert.That(datStringString.OldValue?.GetType(), Is.EqualTo(typeof(string)));
        Assert.That(datStringString.NewValue?.GetType(), Is.EqualTo(typeof(string)));
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

        var dataAddOnly = Deserialize<AuditLogResponseItemChange>(addOnly);
        var dataRemoveOnly = Deserialize<AuditLogResponseItemChange>(removeOnly);
        var dataAddAndRemove = Deserialize<AuditLogResponseItemChange>(addAndRemove);

        Assert.That(dataAddOnly, Is.Not.Null);
        Assert.That(dataRemoveOnly, Is.Not.Null);
        Assert.That(dataAddAndRemove, Is.Not.Null);

        Assert.That(dataAddOnly.NewValue?.GetType(), Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(dataRemoveOnly.NewValue?.GetType(), Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(dataAddAndRemove.NewValue?.GetType(), Is.EqualTo(typeof(PermissionDiffSchema)));

        var valueAddOnly = (PermissionDiffSchema)dataAddOnly.NewValue!;
        var valueTypedRemoveOnly = (PermissionDiffSchema)dataRemoveOnly.NewValue!;
        var valueAddAndRemove = (PermissionDiffSchema)dataAddAndRemove.NewValue!;

        Assert.That(dataAddOnly.Key, Is.EqualTo("permissions_diff"));
        Assert.That(dataAddOnly.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(dataAddOnly.NewValue, Is.Not.Null);
        Assert.That(valueAddOnly.Added.Count, Is.EqualTo(5));
        Assert.That(valueAddOnly.Added, Has.Some.EqualTo("ADMINISTRATOR"));
        Assert.That(valueAddOnly.Added, Has.Some.EqualTo("MANAGE_CHANNELS"));
        Assert.That(valueAddOnly.Added, Has.Some.EqualTo("MANAGE_GUILD"));
        Assert.That(valueAddOnly.Added, Has.Some.EqualTo("VIEW_AUDIT_LOG"));
        Assert.That(valueAddOnly.Added, Has.Some.EqualTo("MANAGE_ROLES"));
        Assert.That(valueAddOnly.Removed.Count, Is.EqualTo(0));

        Assert.That(dataRemoveOnly.Key, Is.EqualTo("permissions_diff"));
        Assert.That(dataRemoveOnly.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(dataRemoveOnly.NewValue, Is.Not.Null);
        Assert.That(valueTypedRemoveOnly.Added.Count, Is.EqualTo(0));
        Assert.That(valueTypedRemoveOnly.Removed.Count, Is.EqualTo(1));
        Assert.That(valueTypedRemoveOnly.Removed, Has.Some.EqualTo("MANAGE_CHANNELS"));

        Assert.That(dataAddAndRemove.Key, Is.EqualTo("permissions_diff"));
        Assert.That(dataAddAndRemove.NewValue.GetType, Is.EqualTo(typeof(PermissionDiffSchema)));
        Assert.That(dataAddAndRemove.NewValue, Is.Not.Null);
        Assert.That(valueAddAndRemove.Added.Count, Is.EqualTo(1));
        Assert.That(valueAddAndRemove.Removed.Count, Is.EqualTo(1));
        Assert.That(valueAddAndRemove.Added, Has.Some.EqualTo("MANAGE_CHANNELS"));
        Assert.That(valueAddAndRemove.Removed, Has.Some.EqualTo("MANAGE_ROLES"));
    }

    private T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}
