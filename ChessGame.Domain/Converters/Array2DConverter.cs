using System.Text.Json;
using System.Text.Json.Serialization;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Domain.Converters
{
    public class Array2DConverter : JsonConverter<BoardCell[,]>
    {
        public override BoardCell[,]? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            BoardCell[,] value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStartArray();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    JsonSerializer.Serialize(writer, value[i, j], options);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }
}
