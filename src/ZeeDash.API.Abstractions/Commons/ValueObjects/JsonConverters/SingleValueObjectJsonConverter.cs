namespace ZeeDash.API.Abstractions.Commons.ValueObjects.JsonConverters;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZeeDash.API.Abstractions.Commons.Extensions;

public class SingleValueObjectJsonConverterFactory
    : JsonConverterFactory {

    #region Static Fields

    private static readonly Type SingleValueObjectType;

    #endregion Static Fields

    #region Ctor.Dtor

    static SingleValueObjectJsonConverterFactory() {
        SingleValueObjectType = typeof(SingleValueObject<>);
    }

    public SingleValueObjectJsonConverterFactory() {
    }

    #endregion Ctor.Dtor

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsSubclassOfRawGeneric(SingleValueObjectType);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
        var converter = (JsonConverter)Activator.CreateInstance(typeof(SingleValueObjectJsonConverter<>).MakeGenericType(typeToConvert));
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.

#pragma warning disable CS8603 // Existence possible d'un retour de référence null.
        return converter;
#pragma warning restore CS8603 // Existence possible d'un retour de référence null.
    }
}

public class SingleValueObjectJsonConverter<T>
    : JsonConverter<T> {

    #region Static Fields

    private static readonly Type SingleValueObjectType;

    private static readonly Dictionary<Type, Type> TypesCache;

    private static readonly Dictionary<Type, PropertyInfo> AccessorsCache;

    #endregion Static Fields

    #region Ctor.Dtor

    static SingleValueObjectJsonConverter() {
        TypesCache = new Dictionary<Type, Type>();
        AccessorsCache = new Dictionary<Type, PropertyInfo>();
        SingleValueObjectType = typeof(SingleValueObject<>);
    }

    public SingleValueObjectJsonConverter() {
    }

    #endregion Ctor.Dtor

    #region NetJsonConverter

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsSubclassOfRawGeneric(SingleValueObjectType);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.HasValueSequence) {
            throw new JsonException();
        }

        var ctor = typeToConvert.GetTypeInfo()
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(ci => ci.GetParameters().Length == 1);

        if (ctor == null) {
#pragma warning disable CS8603 // Existence possible d'un retour de référence null.
            return default;
#pragma warning restore CS8603 // Existence possible d'un retour de référence null.
        }

        var parameterType = ctor.GetParameters()
            .First()
            .ParameterType;
        var value = JsonSerializer.Deserialize(
            ref reader,
            parameterType,
            options
        );

#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
#pragma warning disable CS8603 // Existence possible d'un retour de référence null.
        return (T)Activator.CreateInstance(typeToConvert, value);
#pragma warning restore CS8603 // Existence possible d'un retour de référence null.
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        if (!TypesCache.TryGetValue(typeof(T), out var parameterType)) {
            var singleValueObjectType = typeof(T).GetSubclassOfRawGeneric(SingleValueObjectType);
            if (singleValueObjectType == null) {
                return;
            }

            parameterType = singleValueObjectType.GenericTypeArguments.FirstOrDefault();
            if (parameterType == null) {
                return;
            }

            TypesCache.Add(typeof(T), parameterType);
        }

        if (!AccessorsCache.TryGetValue(typeof(T), out var accessor)) {
            var genType = SingleValueObjectType.MakeGenericType(parameterType);
            accessor = genType.GetProperty(nameof(SingleValueObject<object>.Value), BindingFlags.Public | BindingFlags.Instance);
            if (accessor == null) {
                return;
            }

            AccessorsCache.Add(typeof(T), accessor);
        }

        var rawValue = accessor.GetValue(value);
        if (rawValue == null) {
            writer.WriteNullValue();
        } else {
            writer.WriteStringValue(rawValue.ToString());
        }
    }

    #endregion NetJsonConverter
}