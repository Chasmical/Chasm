using System;
using Newtonsoft.Json;
using Xunit;
#pragma warning disable xUnit1045

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SerializationTests
    {
        [Theory, MemberData(nameof(CreateSerializationFixtures))]
        public void JsonNet(object value)
        {
            // test Json.NET serialization
            string serialized = JsonNetSerialize(value, value.GetType());
            Output.WriteLine(serialized);

            // Note: Json.NET doesn't escape unicode characters by default
            string expected = $"\"{value}\"";
            Assert.Equal(expected, serialized);

            // test Json.NET deserialization
            object? deserialized = JsonNetDeserialize(serialized, value.GetType());
            Assert.Equal(value, deserialized);

        }

        [Fact]
        public void NullJsonNet()
        {
            foreach (Type type in GetSerializationTypes())
            {
                if (!type.IsClass) continue;
                Assert.Equal("null", JsonNetSerialize(null, type));
                Assert.Null(JsonNetDeserialize("null", type));
                Assert.Null(JsonNetDeserialize("\"\"", type));
            }
        }

        private static string JsonNetSerialize(object? value, Type type)
            => JsonConvert.SerializeObject(value, type, null);
        private static object? JsonNetDeserialize(string text, Type type)
            => JsonConvert.DeserializeObject(text, type);

    }
}
