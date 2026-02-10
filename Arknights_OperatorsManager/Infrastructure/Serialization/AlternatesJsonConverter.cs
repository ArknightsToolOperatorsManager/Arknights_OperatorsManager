using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arknights_OperatorsManager.Infrastructure.Serialization;

/// <summary>
/// Alternatesプロパティの互換性を保つためのJsonConverter
/// string（旧形式）とList&lt;string&gt;（新形式）の両方に対応
/// </summary>
public class AlternatesJsonConverter : JsonConverter<List<string>?>
{
    public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // nullの場合
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 旧形式: "alternate": "R112" → ["R112"]に変換
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return string.IsNullOrEmpty(value) ? null : new List<string> { value };
        }

        // 新形式: "alternates": ["R112", "R113"]
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var value = reader.GetString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        list.Add(value);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, List<string>? value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteNullValue();
            return;
        }

        // 常に配列形式で書き込み
        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }
        writer.WriteEndArray();
    }
}
