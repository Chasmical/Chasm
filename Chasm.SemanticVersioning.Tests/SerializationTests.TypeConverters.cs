using System;
using System.ComponentModel;
using Xunit;
#pragma warning disable xUnit1045

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SerializationTests
    {
        [Theory, MemberData(nameof(CreateSerializationFixtures))]
        public void TypeConverters(object value)
        {
            // test TypeConverter serialization
            string serialized = TypeConverterSerialize(value, value.GetType());
            Output.WriteLine(serialized);

            // Note: it's just simple ToString and Parse conversions
            string expected = value.ToString()!;
            Assert.Equal(expected, serialized);

            // test TypeConverter deserialization
            object? deserialized = TypeConverterDeserialize(serialized, value.GetType());
            Assert.Equal(value, deserialized);

        }

        [Fact]
        public void NullTypeConverter()
        {
            foreach (Type type in GetSerializationTypes())
            {
                if (!type.IsClass) continue;
                Assert.Equal("", TypeConverterSerialize(null, type));
                Assert.Null(TypeConverterDeserialize("", type));

                TypeConverter converter = TypeDescriptor.GetConverter(type);
                Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(123));
            }
        }

        private static string TypeConverterSerialize(object? value, Type type)
            => TypeDescriptor.GetConverter(type).ConvertToString(value)!;
        private static object? TypeConverterDeserialize(string text, Type type)
            => TypeDescriptor.GetConverter(type).ConvertFromString(text);

    }
}
