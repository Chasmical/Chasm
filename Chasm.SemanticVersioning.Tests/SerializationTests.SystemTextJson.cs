using System;
using System.Text.Json;
using Xunit;
#pragma warning disable xUnit1045

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SerializationTests
    {
        [Theory, MemberData(nameof(CreateSerializationFixtures))]
        public void SystemTextJson(object value)
        {
            // test System.Text.Json serialization
            string serialized = SystemTextJsonSerialize(value, value.GetType());
            Output.WriteLine(serialized);

            // Note: System.Text.Json's default encoder encodes some characters as escape sequences
            string expected = $"\"{value.ToString()!.Replace("+", "\\u002B").Replace("<", "\\u003C").Replace(">", "\\u003E")}\"";
            Assert.Equal(expected, serialized);

            // test System.Text.Json deserialization
            object? deserialized = SystemTextJsonDeserialize(serialized, value.GetType());
            Assert.Equal(value, deserialized);

        }

        [Fact]
        public void NullSystemTextJson()
        {
            foreach (Type type in GetSerializationTypes())
            {
                if (!type.IsClass) continue;
                Assert.Equal("null", SystemTextJsonSerialize(null, type));
                Assert.Null(SystemTextJsonDeserialize("null", type));
                Assert.Null(SystemTextJsonDeserialize("\"\"", type));
            }
        }

        private static string SystemTextJsonSerialize(object? value, Type type)
            => JsonSerializer.Serialize(value, type);
        private static object? SystemTextJsonDeserialize(string text, Type type)
            => JsonSerializer.Deserialize(text, type);

    }
}
