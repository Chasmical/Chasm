using System.Collections;
using System.Collections.Generic;

namespace Chasm.SemanticVersioning.Tests
{
    public class FixtureAdapter<TFixture> : IFixtureAdapter<TFixture> where TFixture : Fixture
    {
        public string? Name { get; }
        private readonly List<TFixture> fixtures = [];

        public FixtureAdapter() { }
        public FixtureAdapter(string? name) => Name = name;

        public TFixture Add(TFixture fixture)
        {
            fixtures.Add(fixture);
            fixture.Adapter = this;
            fixture.Id ??= Name is null ? $"#{fixtures.Count:000}" : $"{Name} #{fixtures.Count:000}";
            return fixture;
        }
        void IFixtureAdapter<TFixture>.Add(TFixture fixture)
            => Add(fixture);
        void IFixtureAdapter.Add(Fixture fixture)
            => Add((TFixture)fixture);

        public IEnumerator<TFixture> GetEnumerator()
            => fixtures.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerator<object?[]> IEnumerable<object?[]>.GetEnumerator()
        {
            foreach (TFixture fixture in fixtures)
                yield return new object?[1] { fixture };
        }

    }
    public interface IFixtureAdapter : IEnumerable<object?[]>
    {
        void Add(Fixture fixture);
    }
    public interface IFixtureAdapter<in TFixture> : IFixtureAdapter
    {
        void Add(TFixture fixture);
    }
}
