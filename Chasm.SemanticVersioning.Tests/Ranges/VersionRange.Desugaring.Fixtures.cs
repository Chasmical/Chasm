using System;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<DesugaringFixture> CreateDesugaringFixtures()
        {
            FixtureAdapter<DesugaringFixture> adapter = [];

            DesugaringFixture New(string source)
                => adapter.Add(new DesugaringFixture(source));

            const int max = int.MaxValue;



            // Examples from node-semver README

            New("1.2.3 - 2.3.4").DesugarsTo(">=1.2.3 <=2.3.4");
            New("1.2 - 2.3.4").DesugarsTo(">=1.2.0 <=2.3.4");
            New("1.2.3 - 2.3").DesugarsTo(">=1.2.3 <2.4.0-0");
            New("1.2.3 - 2").DesugarsTo(">=1.2.3 <3.0.0-0");

            New("*").DesugarsTo(">=0.0.0");
            New("1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");

            New("1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.x.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");

            New("~1.2.3").DesugarsTo(">=1.2.3 <1.3.0-0");
            New("~1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("~1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("~0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("~0.2").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("~0").DesugarsTo(">=0.0.0 <1.0.0-0");
            New("~1.2.3-beta.2").DesugarsTo(">=1.2.3-beta.2 <1.3.0-0");

            New("^1.2.3").DesugarsTo(">=1.2.3 <2.0.0-0");
            New("^0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("^0.0.3").DesugarsTo(">=0.0.3 <0.0.4-0");
            New("^1.2.3-beta.2").DesugarsTo(">=1.2.3-beta.2 <2.0.0-0");
            New("^0.0.3-beta").DesugarsTo(">=0.0.3-beta <0.0.4-0");

            New("^1.2.x").DesugarsTo(">=1.2.0 <2.0.0-0");
            New("^0.0.x").DesugarsTo(">=0.0.0 <0.1.0-0");
            New("^0.0").DesugarsTo(">=0.0.0 <0.1.0-0");
            New("^1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("^0.x").DesugarsTo(">=0.0.0 <1.0.0-0");



            // X-Range implicit equality comparator desugaring
            New("1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("x").DesugarsTo(">=0.0.0");

            // X-Range '=' comparator desugaring
            New("=1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("=1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("=1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("=1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("=x").DesugarsTo(">=0.0.0");

            // X-Range '>' comparator desugaring
            New(">1.2.x").DesugarsTo(">=1.3.0");
            New(">1.2").DesugarsTo(">=1.3.0");
            New(">1.x").DesugarsTo(">=2.0.0");
            New(">1").DesugarsTo(">=2.0.0");
            New(">x").DesugarsTo("<0.0.0-0");

            // X-Range '>=' comparator desugaring
            New(">=1.2.x").DesugarsTo(">=1.2.0");
            New(">=1.2").DesugarsTo(">=1.2.0");
            New(">=1.x").DesugarsTo(">=1.0.0");
            New(">=1").DesugarsTo(">=1.0.0");
            New(">=x").DesugarsTo(">=0.0.0");

            // X-Range '<' comparator desugaring
            New("<1.2.x").DesugarsTo("<1.2.0-0");
            New("<1.2").DesugarsTo("<1.2.0-0");
            New("<1.x").DesugarsTo("<1.0.0-0");
            New("<1").DesugarsTo("<1.0.0-0");
            New("<x").DesugarsTo("<0.0.0-0");

            // X-Range '<=' comparator desugaring
            New("<=1.2.x").DesugarsTo("<1.3.0-0");
            New("<=1.2").DesugarsTo("<1.3.0-0");
            New("<=1.x").DesugarsTo("<2.0.0-0");
            New("<=1").DesugarsTo("<2.0.0-0");
            New("<=x").DesugarsTo(">=0.0.0");



            // X-Range '=' comparator overflow exceptions
            New($"=1.{max - 1}.x").DesugarsTo($">=1.{max - 1}.0 <1.{max}.0-0");
            New($"=1.{max - 1}").DesugarsTo($">=1.{max - 1}.0 <1.{max}.0-0");
            New($"=1.{max}.x").Throws(Exceptions.MinorTooBig);
            New($"=1.{max}").Throws(Exceptions.MinorTooBig);
            New($"={max - 1}.x").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"={max - 1}").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"={max}.x").Throws(Exceptions.MajorTooBig);
            New($"={max}").Throws(Exceptions.MajorTooBig);

            // X-Range '>' comparator overflow exceptions
            New($">1.{max - 1}.x").DesugarsTo($">=1.{max}.0");
            New($">1.{max - 1}").DesugarsTo($">=1.{max}.0");
            New($">1.{max}.x").Throws(Exceptions.MinorTooBig);
            New($">1.{max}").Throws(Exceptions.MinorTooBig);

            New($">{max - 1}.x").DesugarsTo($">={max}.0.0");
            New($">{max - 1}").DesugarsTo($">={max}.0.0");
            New($">{max}.x").Throws(Exceptions.MajorTooBig);
            New($">{max}").Throws(Exceptions.MajorTooBig);

            // X-Range '>=' comparator overflow exceptions
            New($">=1.{max}.x").DesugarsTo($">=1.{max}.0");
            New($">=1.{max}").DesugarsTo($">=1.{max}.0");
            New($">={max}.x").DesugarsTo($">={max}.0.0");
            New($">={max}").DesugarsTo($">={max}.0.0");

            // X-Range '<' comparator overflow exceptions
            New($"<1.{max}.x").DesugarsTo($"<1.{max}.0-0");
            New($"<1.{max}").DesugarsTo($"<1.{max}.0-0");
            New($"<{max}.x").DesugarsTo($"<{max}.0.0-0");
            New($"<{max}").DesugarsTo($"<{max}.0.0-0");

            // X-Range '<=' comparator overflow exceptions
            New($"<=1.{max - 1}.x").DesugarsTo($"<1.{max}.0-0");
            New($"<=1.{max - 1}").DesugarsTo($"<1.{max}.0-0");
            New($"<=1.{max}.x").Throws(Exceptions.MinorTooBig);
            New($"<=1.{max}").Throws(Exceptions.MinorTooBig);

            New($"<={max - 1}.x").DesugarsTo($"<{max}.0.0-0");
            New($"<={max - 1}").DesugarsTo($"<{max}.0.0-0");
            New($"<={max}.x").Throws(Exceptions.MajorTooBig);
            New($"<={max}").Throws(Exceptions.MajorTooBig);



            // Hyphen Range comparator desugaring
            New("1.2.3 - 3.4.5").DesugarsTo(">=1.2.3 <=3.4.5");
            New("1.2.x - 3.4.5").DesugarsTo(">=1.2.0 <=3.4.5");
            New("1.2 - 3.4.5").DesugarsTo(">=1.2.0 <=3.4.5");
            New("1.x - 3.4.5").DesugarsTo(">=1.0.0 <=3.4.5");
            New("1 - 3.4.5").DesugarsTo(">=1.0.0 <=3.4.5");
            New("x - 3.4.5").DesugarsTo("<=3.4.5");

            New("1.2.3 - 3.4.x").DesugarsTo(">=1.2.3 <3.5.0-0");
            New("1.2.3 - 3.4").DesugarsTo(">=1.2.3 <3.5.0-0");
            New("1.2.3 - 3.x").DesugarsTo(">=1.2.3 <4.0.0-0");
            New("1.2.3 - 3").DesugarsTo(">=1.2.3 <4.0.0-0");
            New("1.2.3 - x").DesugarsTo(">=1.2.3");

            // Hyphen Range comparator overflow exceptions
            New($"1.2.{max} - 3.4.5").DesugarsTo($">=1.2.{max} <=3.4.5");
            New($"1.{max}.3 - 3.4.5").DesugarsTo($">=1.{max}.3 <=3.4.5");
            New($"{max}.2.3 - 3.4.5").DesugarsTo($">={max}.2.3 <=3.4.5");
            New($"1.{max} - 3.4.5").DesugarsTo($">=1.{max}.0 <=3.4.5");
            New($"{max}.2 - 3.4.5").DesugarsTo($">={max}.2.0 <=3.4.5");
            New($"{max} - 3.4.5").DesugarsTo($">={max}.0.0 <=3.4.5");

            New($"1.2.3 - 3.4.{max}").DesugarsTo($">=1.2.3 <=3.4.{max}");
            New($"1.2.3 - 3.{max}.5").DesugarsTo($">=1.2.3 <=3.{max}.5");
            New($"1.2.3 - {max}.4.5").DesugarsTo($">=1.2.3 <={max}.4.5");
            New($"1.2.3 - 3.{max - 1}").DesugarsTo($">=1.2.3 <3.{max}.0-0");
            New($"1.2.3 - 3.{max}").Throws(Exceptions.MinorTooBig);
            New($"1.2.3 - {max - 1}").DesugarsTo($">=1.2.3 <{max}.0.0-0");
            New($"1.2.3 - {max}").Throws(Exceptions.MajorTooBig);



            // Caret comparator desugaring
            New("^1.2.3").DesugarsTo(">=1.2.3 <2.0.0-0");
            New("^1.2.x").DesugarsTo(">=1.2.0 <2.0.0-0");
            New("^1.2").DesugarsTo(">=1.2.0 <2.0.0-0");
            New("^1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("^1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("^x").DesugarsTo(">=0.0.0");

            New("^0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("^0.2.x").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("^0.2").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("^0.x").DesugarsTo(">=0.0.0 <1.0.0-0");
            New("^0").DesugarsTo(">=0.0.0 <1.0.0-0");

            New("^0.0.3").DesugarsTo(">=0.0.3 <0.0.4-0");
            New("^0.0.x").DesugarsTo(">=0.0.0 <0.1.0-0");

            // Caret comparator overflow exceptions (patch)
            New($"^1.2.{max}").DesugarsTo($">=1.2.{max} <2.0.0-0");
            New($"^0.2.{max}").DesugarsTo($">=0.2.{max} <0.3.0-0");
            New($"^0.0.{max - 1}").DesugarsTo($">=0.0.{max - 1} <0.0.{max}-0");
            New($"^0.0.{max}").Throws(Exceptions.PatchTooBig);

            // Caret comparator overflow exceptions (minor)
            New($"^1.{max}.3").DesugarsTo($">=1.{max}.3 <2.0.0-0");
            New($"^1.{max}.x").DesugarsTo($">=1.{max}.0 <2.0.0-0");
            New($"^1.{max}").DesugarsTo($">=1.{max}.0 <2.0.0-0");
            New($"^0.{max - 1}.3").DesugarsTo($">=0.{max - 1}.3 <0.{max}.0-0");
            New($"^0.{max - 1}.x").DesugarsTo($">=0.{max - 1}.0 <0.{max}.0-0");
            New($"^0.{max - 1}").DesugarsTo($">=0.{max - 1}.0 <0.{max}.0-0");
            New($"^0.{max}.3").Throws(Exceptions.MinorTooBig);
            New($"^0.{max}.x").Throws(Exceptions.MinorTooBig);
            New($"^0.{max}").Throws(Exceptions.MinorTooBig);

            // Caret comparator overflow exceptions (major)
            New($"^{max - 1}.2.3").DesugarsTo($">={max - 1}.2.3 <{max}.0.0-0");
            New($"^{max - 1}.x.x").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"^{max - 1}").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"^{max}.2.3").Throws(Exceptions.MajorTooBig);
            New($"^{max}.x.x").Throws(Exceptions.MajorTooBig);
            New($"^{max}").Throws(Exceptions.MajorTooBig);



            // Tilde comparator desugaring
            New("~1.2.3").DesugarsTo(">=1.2.3 <1.3.0-0");
            New("~1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("~1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("~1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("~1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("~x").DesugarsTo(">=0.0.0");

            New("~0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("~0.2.x").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("~0.2").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("~0.x").DesugarsTo(">=0.0.0 <1.0.0-0");
            New("~0").DesugarsTo(">=0.0.0 <1.0.0-0");

            New("~0.0.3").DesugarsTo(">=0.0.3 <0.1.0-0");
            New("~0.0.x").DesugarsTo(">=0.0.0 <0.1.0-0");
            New("~0.0").DesugarsTo(">=0.0.0 <0.1.0-0");

            // Tilde comparator overflow exceptions (patch)
            New($"~1.2.{max}").DesugarsTo($">=1.2.{max} <1.3.0-0");
            New($"~0.2.{max}").DesugarsTo($">=0.2.{max} <0.3.0-0");
            New($"~0.0.{max}").DesugarsTo($">=0.0.{max} <0.1.0-0");

            // Tilde comparator overflow exceptions (minor)
            New($"~1.{max - 1}.3").DesugarsTo($">=1.{max - 1}.3 <1.{max}.0-0");
            New($"~1.{max - 1}.x").DesugarsTo($">=1.{max - 1}.0 <1.{max}.0-0");
            New($"~1.{max - 1}").DesugarsTo($">=1.{max - 1}.0 <1.{max}.0-0");
            New($"~1.{max}.3").Throws(Exceptions.MinorTooBig);
            New($"~1.{max}.x").Throws(Exceptions.MinorTooBig);
            New($"~1.{max}").Throws(Exceptions.MinorTooBig);

            New($"~0.{max - 1}.3").DesugarsTo($">=0.{max - 1}.3 <0.{max}.0-0");
            New($"~0.{max - 1}.x").DesugarsTo($">=0.{max - 1}.0 <0.{max}.0-0");
            New($"~0.{max - 1}").DesugarsTo($">=0.{max - 1}.0 <0.{max}.0-0");
            New($"~0.{max}.3").Throws(Exceptions.MinorTooBig);
            New($"~0.{max}.x").Throws(Exceptions.MinorTooBig);
            New($"~0.{max}").Throws(Exceptions.MinorTooBig);

            // Tilde comparator overflow exceptions (major)
            New($"~{max}.2.3").DesugarsTo($">={max}.2.3 <{max}.3.0-0");
            New($"~{max}.2.x").DesugarsTo($">={max}.2.0 <{max}.3.0-0");
            New($"~{max}.2").DesugarsTo($">={max}.2.0 <{max}.3.0-0");
            New($"~{max - 1}.x").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"~{max - 1}").DesugarsTo($">={max - 1}.0.0 <{max}.0.0-0");
            New($"~{max}.x").Throws(Exceptions.MajorTooBig);
            New($"~{max}").Throws(Exceptions.MajorTooBig);



            return adapter;
        }

        public class DesugaringFixture(string source) : FuncFixture<VersionRange>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public DesugaringFixture() : this(null!) { }

            public string Source { get; } = source;
            public string? Expected { get; private set; }
            protected override Type DefaultExceptionType => typeof(InvalidOperationException);

            public void DesugarsTo(string expectedRange)
            {
                MarkAsComplete();
                Expected = expectedRange;
            }

            public override void AssertResult(VersionRange? result)
            {
                Assert.NotNull(result);
                Assert.Equal(VersionRange.Parse(Expected!), result);
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\"";
        }
    }
}
