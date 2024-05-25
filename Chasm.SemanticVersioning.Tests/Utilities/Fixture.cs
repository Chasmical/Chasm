using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public abstract class Fixture
    {
        public string? Id { get; set; }
        public IFixtureAdapter? Adapter { get; internal set; }

        public bool IsComplete { get; private set; }
        public bool IsValid { get; private set; }

        protected void MarkAsComplete(bool isValid = true)
        {
            Assert.False(IsComplete, "A fixture cannot be marked complete twice.");
            IsComplete = true;
            IsValid = isValid;
        }

        public override string ToString()
            => $"{Id ?? "Unnamed"} {(IsValid ? '\u2705' : '\u274C')}";

        protected TExtender Extend<TExtender>() where TExtender : IFixtureExtender<Fixture>, new()
        {
            TExtender extender = new TExtender();
            extender.SetPrototype(this);
            return extender;
        }

    }
}
