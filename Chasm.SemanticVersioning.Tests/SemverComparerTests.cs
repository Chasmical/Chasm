using Chasm.Collections;
using Chasm.SemanticVersioning.Ranges;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverComparerTests
    {
        public static SemanticVersion[][] FixturesSemanticVersionDefault()
            => SemanticVersionTests.CreateSortingFixtures().ConvertAll(static v => new[] { v });

        public static SemanticVersion[][] FixturesSemanticVersionExact()
        {
            string[] versions = [
                "0.0.0-0+-",
                "0.0.0-0+-0",
                "0.0.0-0+0",
                "0.0.0-0+00",
                "0.0.0-0+01",
                "0.0.0-0+010",
                "0.0.0-0+10",
                "0.0.0-0",
                "0.0.0+1000",
                "0.0.0+999",
                "0.0.0+test.DEV.99",
                "0.0.0+test.build",
                "0.0.0+test.dev",
                "0.0.0+test.dev.99",
                "1.2.3-0+A",
                "1.2.3-1+A",
                "1.2.3-1",
                "1.2.3",
            ];
            return versions.ConvertAll(static v => new[] { SemanticVersion.Parse(v) });
        }



        public static PartialComponent[][] FixturesPartialComponentDefault()
            => PartialComponentTests.CreateSortingFixtures();

        public static PartialComponent[][] FixturesPartialComponentExact()
        {
            return [
                // TODO
            ];
        }



        public static PartialVersion[][] FixturesPartialVersionDefault()
            => PartialVersionTests.CreateSortingFixtures();

        public static PartialVersion[][] FixturesPartialVersionIncludeBuild()
        {
            string[][] partials = [
                ["x.x.x-0+-", "X.*.x-0+-"],
                ["x.x.x-0+010"],
                ["x.x.x-0+1"],
                ["x.x.x-0", "X.*.x-0"],
                ["x.x.x", "*.*", "X.*.x"],
                ["0.0.0-0"],
                ["0.0.0"],
                ["1.2.x+09"],
                ["1.2.x+5"],
                ["1.2.3-0+B"],
                ["1.2.3-1+A"],
                ["1.2.3-1"],
                ["1.2.3"],
                ["9.x.*+3", "9.*.X+3"],
            ];
            return partials.ConvertAll(static a => a.ConvertAll(PartialVersion.Parse));
        }
        public static PartialVersion[][] FixturesPartialVersionDiffWildcards()
        {
            string[][] partials = [
                // TODO
            ];
            return partials.ConvertAll(static a => a.ConvertAll(PartialVersion.Parse));
        }
        public static PartialVersion[][] FixturesPartialVersionExact()
        {
            string[][] partials = [
                // TODO
            ];
            return partials.ConvertAll(static a => a.ConvertAll(PartialVersion.Parse));
        }

    }
}
