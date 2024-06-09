using System;
using System.Collections.Generic;
using System.Linq;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SerializationTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Pure] public static TheoryData<object> CreateSerializationFixtures()
        {
            object[] objects =
            [
                SemanticVersion.MinValue,
                SemanticVersion.MaxValue,
                new SemanticVersion(3, 0, 0),
                new SemanticVersion(1, 2, 3, ["beta", 5], ["BUILD", "07-"]),

                SemverPreRelease.Zero,
                new SemverPreRelease(562),
                new SemverPreRelease("alpha-beta"),

                PartialVersion.OneStar,
                new PartialVersion('X', 'x', 5),
                new PartialVersion(4, 5, 'x', ["delta", 4], ["DEV--"]),

                PartialComponent.LowerX,
                PartialComponent.Star,
                PartialComponent.Omitted,
                new PartialComponent(6792),

                VersionRange.All,
                VersionRange.None,
                VersionRange.Parse(">1.2.3-alpha.4 <2.0.0-0 || ~3.* || ^2.3.x-beta.5"),
            ];
            return new TheoryData<object>(objects);
        }

        [Pure] public static IEnumerable<Type> GetSerializationTypes()
            => CreateSerializationFixtures().Select(static f => f[0].GetType()).Distinct();

    }
}
