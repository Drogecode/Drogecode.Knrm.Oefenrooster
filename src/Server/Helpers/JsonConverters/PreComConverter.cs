using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;

public class PreComConverter : JsonConverter<NotificationDataBase>
{
    public override NotificationDataBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            var jsonObject = jsonDocument.RootElement.GetRawText();
            if (jsonDocument.RootElement.TryGetProperty(nameof(NotificationDataSoundObject._sound), out var typeProperty) && typeProperty.ValueKind.Equals("object"))
            {
                return JsonSerializer.Deserialize<NotificationDataSoundObject>(jsonObject, options);
            }
            else
            {
                return JsonSerializer.Deserialize<NotificationDataSoundString>(jsonObject, options);
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, NotificationDataBase value, JsonSerializerOptions options)
    {
        if (value is NotificationDataSoundObject)
        {
            JsonSerializer.Serialize(writer, value as NotificationDataSoundObject, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, value as NotificationDataSoundString, options);
        }
    }
}
