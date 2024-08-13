using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("xUnit", "xUnit3001", Justification = "Class already has an implicit constructor.")]
    public abstract class Fixture : IXunitSerializable
    {
        public string? Id { get; set; }
        public IFixtureAdapter? Adapter { get; internal set; }
        public int? LineNumber { get; init; }

        public bool IsComplete { get; private set; }
        public bool IsValid { get; private set; }

        protected void MarkAsComplete(bool isValid = true)
        {
            Assert.False(IsComplete, "A fixture cannot be marked complete twice.");
            IsComplete = true;
            IsValid = isValid;
        }

        public override string ToString()
            => $"{Id ?? "Unnamed"}{(LineNumber is null ? null : $" (line {LineNumber})")} {(IsValid ? '\u2705' : '\u274C')}";

        protected TExtender Extend<TExtender>() where TExtender : IFixtureExtender<Fixture>, new()
        {
            TExtender extender = new TExtender();
            extender.SetPrototype(this);
            return extender;
        }

        protected TFixture AddNew<TFixture>(TFixture fixture) where TFixture : Fixture
        {
            Adapter!.Add(fixture);
            return fixture;
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                field.SetValue(this, info.GetValue(field.Name, field.FieldType));
        }
        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                info.AddValue(field.Name, field.GetValue(this), field.FieldType);
        }
    }
}
