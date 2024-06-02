namespace Chasm.SemanticVersioning.Tests
{
    public abstract class FixtureExtender<TFixture> : IFixtureExtender<TFixture> where TFixture : Fixture
    {
        public TFixture Prototype { get; private set; } = null!;
        public IFixtureAdapter Adapter => Prototype.Adapter!;

        Fixture IFixtureExtender.Prototype
            => Prototype;
        void IFixtureExtender.SetPrototype(Fixture prototype)
            => Prototype = (TFixture)prototype;

        protected TExtender Extend<TExtender>() where TExtender : IFixtureExtender<TFixture>, new()
        {
            TExtender extender = new TExtender();
            extender.SetPrototype(Prototype);
            return extender;
        }

        protected TFixture AddNew(TFixture fixture)
        {
            Adapter.Add(fixture);
            return fixture;
        }

    }
    public interface IFixtureExtender
    {
        Fixture Prototype { get; }
        void SetPrototype(Fixture prototype);
    }
    public interface IFixtureExtender<out TFixture> : IFixtureExtender
    {
        new TFixture Prototype { get; }
    }
}
