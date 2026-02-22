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

    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }
}
