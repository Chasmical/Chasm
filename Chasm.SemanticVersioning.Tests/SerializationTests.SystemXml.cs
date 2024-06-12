using System;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using Xunit;
#pragma warning disable xUnit1045

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SerializationTests
    {
        [Theory, MemberData(nameof(CreateSerializationFixtures))]
        public void SystemXml(object value)
        {
            // At the moment schemas are not implemented
            Assert.Null(((IXmlSerializable)value).GetSchema());

            // test Xml serialization
            Type type = value.GetType();
            string serialized = SystemXmlSerialize(value, type);
            Output.WriteLine(serialized);

            // Note: System.Xml turns '<' and '>' into '&lt;' and '&gt;'
            string expected = $"<{type.Name}>{SecurityElement.Escape(value.ToString())}</{type.Name}>";
            Assert.Equal(expected, serialized);

            // test Xml deserialization
            object? deserialized = SystemXmlDeserialize(serialized, type);
            Assert.Equal(value, deserialized);

        }

        [Fact]
        public void NullSystemXml()
        {
            foreach (Type type in GetSerializationTypes())
            {
                if (!type.IsClass) continue;

                Type containerType = typeof(Container<>).MakeGenericType(type);
                object container = Activator.CreateInstance(containerType, true)!;

                const string xsiNs = "http://www.w3.org/2001/XMLSchema-instance";
                const string xsdNs = "http://www.w3.org/2001/XMLSchema";
                string expected = $"""<ContainerOf{type.Name} xmlns:xsi="{xsiNs}" xmlns:xsd="{xsdNs}"><One>1</One></ContainerOf{type.Name}>""";
                Output.WriteLine(expected);

                Assert.Equal(expected, SystemXmlSerialize(container, containerType));
                Assert.Null(((IContainer)SystemXmlDeserialize(expected, containerType)!).Value);
            }
        }

        [Theory, MemberData(nameof(CreateSerializationFixtures))]
        public void DoubleDeserializationSystemXml(object value)
        {
            Type type = value.GetType();
            // assuming previous tests have succeeded, and serialization works correctly
            string expected = SystemXmlSerialize(value, type);

            object? deserialized;

            // try manually using the IXmlSerializable.ReadXml method
            using (StringReader reader = new StringReader(expected))
            using (XmlReader xmlReader = XmlReader.Create(reader))
            {
                xmlReader.MoveToContent();
                deserialized = Activator.CreateInstance(type, true)!;
                ((IXmlSerializable)deserialized).ReadXml(xmlReader);
                Assert.Equal(value, deserialized);
            }

            // make sure that ReadXml can run only once (for classes)
            using (StringReader reader = new StringReader(expected))
            using (XmlReader xmlReader = XmlReader.Create(reader))
            {
                xmlReader.MoveToContent();
                if (type.IsClass)
                {
                    // classes can only be deserialized once
                    Assert.Throws<InvalidOperationException>(() => ((IXmlSerializable)deserialized).ReadXml(xmlReader));
                }
                else
                {
                    // structs are boxed, so it should be fine to deserialize boxed values several times
                    ((IXmlSerializable)deserialized).ReadXml(xmlReader);
                    Assert.Equal(value, deserialized);
                }
            }
        }

        private static string SystemXmlSerialize(object? value, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            XmlWriterSettings settings = new() { OmitXmlDeclaration = true };

            using (StringWriter writer = new StringWriter())
            using (XmlWriter xml = XmlWriter.Create(writer, settings))
            {
                ser.Serialize(xml, value);
                return writer.ToString();
            }
        }
        private static object? SystemXmlDeserialize(string text, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);

            using (StringReader reader = new StringReader(text))
            using (XmlReader xml = XmlReader.Create(reader))
                return ser.Deserialize(xml);
        }

        public class Container<T> : IContainer
        {
            public int One = 1;
            //[XmlElement("Value", IsNullable = true)]
            public T? Value { get; set; }
            object? IContainer.Value => Value;
        }
        private interface IContainer
        {
            object? Value { get; }
        }

    }
}
