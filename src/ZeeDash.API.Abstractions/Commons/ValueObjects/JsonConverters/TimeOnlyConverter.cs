namespace ZeeDash.API.Abstractions.Commons.ValueObjects.JsonConverters;

using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeOnlyConverter : JsonConverter<TimeOnly> {
    private readonly string serializationFormat;

    public TimeOnlyConverter() : this(null) {
    }

    public TimeOnlyConverter(string? serializationFormat) {
        this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(this.serializationFormat));
}
