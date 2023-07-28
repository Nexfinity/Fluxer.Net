using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Squll.Net.Extensions;

// slightly modified version of https://stackoverflow.com/questions/33321698/jsonconverter-with-interface
public class JsonDerivedTypeConverter<T> : JsonConverter
{
    public JsonDerivedTypeConverter()
    {
        DerivedTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(T)));
    }

    readonly HashSet<Type> derivedTypes = new();

    public IEnumerable<Type> DerivedTypes
    {
        get
        {
            return derivedTypes.ToArray();
        }
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            derivedTypes.Clear();
            if (value != null)
                derivedTypes.UnionWith(value);
        }
    }

    JsonObjectContract FindContract(JObject obj, JsonSerializer serializer)
    {
        List<(JsonObjectContract Contract, int MissingProps)> bestContracts = new();
        foreach (var type in derivedTypes)
        {
            if (type.IsAbstract)
                continue;
            if (serializer.ContractResolver.ResolveContract(type) is not JsonObjectContract contract)
                continue;
            var sel = (contract, obj.Properties().Select(p => p.Name).Count(n => contract.Properties.GetClosestMatchProperty(n) == null));

            if (bestContracts.Count == 0 || bestContracts[0].MissingProps > sel.Item2)
            {
                bestContracts.Clear();
                bestContracts.Add(sel);
            }
        }
        return bestContracts.Single().Contract;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        var obj = JObject.Load(reader); // Throws an exception if the current token is not an object.
        var contract = FindContract(obj, serializer)
            ?? throw new JsonSerializationException("no contract found for " + obj.ToString());
        if (existingValue == null || !contract.UnderlyingType.IsAssignableFrom(existingValue.GetType()))
            existingValue = contract.DefaultCreator();
        using (var sr = obj.CreateReader())
        {
            serializer.Populate(sr, existingValue);
        }
        return existingValue;
    }

    public override bool CanWrite { get { return false; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
