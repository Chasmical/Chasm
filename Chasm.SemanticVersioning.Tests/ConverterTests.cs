using System;
using System.ComponentModel;
using System.Text.Json;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;
#pragma warning disable xUnit1045 // 'The type argument object might not be serializable'

namespace Chasm.SemanticVersioning.Tests
{
    public sealed class ConverterTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Pure] public static TheoryData<object> GetSimpleFixtures()
        {
            object[] objects =
            [
                new SemanticVersion(3, 4, 5, [0]),
                new SemanticVersion(1, 2, 3, ["beta", 5], ["BUILD", "07-"]),

                new SemverPreRelease("alpha-beta"),
                new SemverPreRelease(562),

                new PartialVersion('X', 'x'),
                new PartialVersion(4, 5, 'x', ["delta", 4], ["DEV--"]),

                new PartialComponent('x'),
                new PartialComponent('*'),
                PartialComponent.Omitted,
                new PartialComponent(6792),

                // TODO: VersionRange doesn't have equality methods yet
                // VersionRange.Parse("<1.2.3-alpha.4 || ~3.* || ^2.3.x-beta.5"),
            ];
            return new TheoryData<object>(objects);
        }

        [Theory, MemberData(nameof(GetSimpleFixtures))]
        public void TypeConverters(object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());

            // test TypeConverter directly
            string str = converter.ConvertToString(value)!;
            Output.WriteLine(str);

            Assert.Equal(value.ToString(), str);
            Assert.Equal(value, converter.ConvertFromString(str));

            // test null argument handling
            if (value.GetType().IsClass)
            {
                Assert.Equal("", converter.ConvertToString(null));
                Assert.Null(converter.ConvertFromString(""));
                Assert.Throws<NotSupportedException>(() => converter.ConvertFromString(null!));
            }
        }

        [Theory, MemberData(nameof(GetSimpleFixtures))]
        public void JsonNetSerialization(object value)
        {
            // test Json.NET serialization that uses type converters
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            Output.WriteLine(json);

            Assert.Equal($"\"{value}\"", json);
            Assert.Equal(value, Newtonsoft.Json.JsonConvert.DeserializeObject(json, value.GetType()));
        }

        [Theory, MemberData(nameof(GetSimpleFixtures))]
        public void SystemTextJsonSerialization(object value)
        {
            // test System.Text.Json serialization that uses JsonConverters
            string json = JsonSerializer.Serialize(value);
            Output.WriteLine(json);

            // Note: the default encoder encodes '+' as '\u002B'
            string expectedContents = value.ToString()!.Replace("+", "\\u002B");
            Assert.Equal($"\"{expectedContents}\"", json);
            Assert.Equal(value, JsonSerializer.Deserialize(json, value.GetType()));

            // test null argument handling
            if (value.GetType().IsClass)
            {
                Assert.Equal("null", JsonSerializer.Serialize((object?)null, value.GetType()));
                Assert.Null(JsonSerializer.Deserialize("null", value.GetType()));
                Assert.Null(JsonSerializer.Deserialize("\"\"", value.GetType()));
            }
        }

    }
}
