using Chasm.SemanticVersioning.Ranges;
using System;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<OperationFixture> CreateOperationFixtures()
        {
            FixtureAdapter<OperationFixture> adapter = [];

            OperationFixture New(string left, char op, string? right = null)
                => adapter.Add(new OperationFixture(op, left, right));

            #region Operations with primitives

            // Complement: test with comparison operators
            New(">1.2.3", '~').Returns("<=1.2.3");
            New("<1.2.3", '~').Returns(">=1.2.3");
            New(">=1.2.3", '~').Returns("<1.2.3");
            New("<=1.2.3", '~').Returns(">1.2.3");
            // Complement: test with equality operators
            New("=1.2.3", '~').Returns("<1.2.3 || >1.2.3");
            New("1.2.3", '~').Returns("<1.2.3 || >1.2.3");
            // Complement: test with special versions
            New("*", '~').Returns("<0.0.0-0");
            New("<0.0.0-0", '~').Returns("*");

            // Union: test basic precedence
            New(">1.2.3", '|', ">3.4.5").Returns(">1.2.3", true);
            New("<1.2.3", '|', "<3.4.5").Returns("<3.4.5", true);
            New(">=1.2.3", '|', ">=3.4.5").Returns(">=1.2.3", true);
            New("<=1.2.3", '|', "<=3.4.5").Returns("<=3.4.5", true);
            // Union: test precedence with inclusive/exclusive
            New(">1.2.3", '|', ">=1.2.3").Returns(">=1.2.3", true);
            New("<1.2.3", '|', "<=1.2.3").Returns("<=1.2.3", true);
            New(">1.2.3", '|', ">=3.4.5").Returns(">1.2.3", true);
            New(">=1.2.3", '|', ">3.4.5").Returns(">=1.2.3", true);
            New("<3.4.5", '|', "<=1.2.3").Returns("<3.4.5", true);
            New("<=3.4.5", '|', "<1.2.3").Returns("<=3.4.5", true);
            // Union: test with equality operators
            // TODO: New("=1.2.3", '|', ">1.2.3").Returns(">=1.2.3", true);
            // TODO: New("=1.2.3", '|', "<1.2.3").Returns("<=1.2.3", true);
            New("=1.2.3", '|', ">=1.2.3").Returns(">=1.2.3", true);
            New("=1.2.3", '|', "<=1.2.3").Returns("<=1.2.3", true);
            // Union: test with special versions
            New(">1.2.3", '|', "*").Returns("*", true);
            New("<1.2.3", '|', "*").Returns("*", true);
            New(">=1.2.3", '|', "*").Returns("*", true);
            New("<=1.2.3", '|', "*").Returns("*", true);
            New(">1.2.3", '|', "<0.0.0-0").Returns(">1.2.3", true);
            New("<1.2.3", '|', "<0.0.0-0").Returns("<1.2.3", true);
            New(">=1.2.3", '|', "<0.0.0-0").Returns(">=1.2.3", true);
            New("<=1.2.3", '|', "<0.0.0-0").Returns("<=1.2.3", true);

            // Intersection: test basic precedence
            New(">1.2.3", '&', ">3.4.5").Returns(">3.4.5", true);
            New("<1.2.3", '&', "<3.4.5").Returns("<1.2.3", true);
            New(">=1.2.3", '&', ">=3.4.5").Returns(">=3.4.5", true);
            New("<=1.2.3", '&', "<=3.4.5").Returns("<=1.2.3", true);
            // Union: test precedence with inclusive/exclusive
            New(">1.2.3", '&', ">=1.2.3").Returns(">1.2.3", true);
            New("<1.2.3", '&', "<=1.2.3").Returns("<1.2.3", true);
            New(">1.2.3", '&', ">=3.4.5").Returns(">=3.4.5", true);
            New(">=1.2.3", '&', ">3.4.5").Returns(">3.4.5", true);
            New("<3.4.5", '&', "<=1.2.3").Returns("<=1.2.3", true);
            New("<=3.4.5", '&', "<1.2.3").Returns("<1.2.3", true);
            // Intersection: test with equality operators
            New("=1.2.3", '&', ">1.2.3").Returns("<0.0.0-0", true);
            New("=1.2.3", '&', "<1.2.3").Returns("<0.0.0-0", true);
            New("=1.2.3", '&', ">=1.2.3").Returns("=1.2.3", true);
            New("=1.2.3", '&', "<=1.2.3").Returns("=1.2.3", true);
            // Intersection: test with special versions
            New(">1.2.3", '&', "*").Returns(">1.2.3", true);
            New("<1.2.3", '&', "*").Returns("<1.2.3", true);
            New(">=1.2.3", '&', "*").Returns(">=1.2.3", true);
            New("<=1.2.3", '&', "*").Returns("<=1.2.3", true);
            New(">1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0", true);
            New("<1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0", true);
            New(">=1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0", true);
            New("<=1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0", true);

            #endregion

            #region Operations with comparator sets

            // Complement: test with comparator sets
            New(">1.2.3 <3.4.5", '~').Returns("<=1.2.3 || >=3.4.5");
            New(">1.2.3 <=3.4.5", '~').Returns("<=1.2.3 || >3.4.5");
            New(">=1.2.3 <3.4.5", '~').Returns("<1.2.3 || >=3.4.5");
            New(">=1.2.3 <=3.4.5", '~').Returns("<1.2.3 || >3.4.5");

            // Union: expanding from the right
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <3.4.5").Returns(">=1.2.3 <3.4.5", true);
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <=3.4.5").Returns(">=1.2.3 <=3.4.5", true);
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <2.0.0").Returns(">=1.2.3 <=2.0.0", true);
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <=2.0.0").Returns(">=1.2.3 <=2.0.0", true);
            // Union: expanding from the left
            New(">=1.2.3 <=2.0.0", '|', ">1.0.0 <=1.5.0").Returns(">1.0.0 <=2.0.0", true);
            New(">=1.2.3 <=2.0.0", '|', ">=1.0.0 <=1.5.0").Returns(">=1.0.0 <=2.0.0", true);
            New(">=1.2.3 <=2.0.0", '|', ">1.2.3 <=1.5.0").Returns(">=1.2.3 <=2.0.0", true);
            New(">=1.2.3 <=2.0.0", '|', ">=1.2.3 <=1.5.0").Returns(">=1.2.3 <=2.0.0", true);
            // Union: expanding from both ends
            New(">=1.2.3 <=2.0.0", '|', ">=1.0.0 <=3.4.5").Returns(">=1.0.0 <=3.4.5", true);

            // Intersection: contracting from the right
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <2.0.0").Returns(">=1.0.0 <2.0.0", true);
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <=2.0.0").Returns(">=1.0.0 <=2.0.0", true);
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <3.4.5").Returns(">=1.0.0 <3.4.5", true);
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <=3.4.5").Returns(">=1.0.0 <=3.4.5", true);
            // Intersection: contracting from the left
            New(">=1.0.0 <=3.4.5", '&', ">1.2.3 <=5.0.0").Returns(">1.2.3 <=3.4.5", true);
            New(">=1.0.0 <=3.4.5", '&', ">=1.2.3 <=5.0.0").Returns(">=1.2.3 <=3.4.5", true);
            New(">=1.0.0 <=3.4.5", '&', ">1.0.0 <=5.0.0").Returns(">1.0.0 <=3.4.5", true);
            New(">=1.0.0 <=3.4.5", '&', ">=1.0.0 <=5.0.0").Returns(">=1.0.0 <=3.4.5", true);
            // Intersection: contracting from both ends
            New(">=1.2.3 <=2.0.0", '&', ">=1.0.0 <=3.4.5").Returns(">=1.2.3 <=2.0.0", true);

            #endregion

            #region Operations with advanced comparators

            // Caret: complement
            New("^1.2.3", '~').Returns("<1.2.3 || >=2.0.0-0");
            New("^0.2.3", '~').Returns("<0.2.3 || >=0.3.0-0");
            New("^0.0.3", '~').Returns("<0.0.3 || >=0.0.4-0");
            New("^0.0.3-rc", '~').Returns("<0.0.3-rc || >=0.0.4-0");
            // Caret: union
            New("^1.2.3", '|', ">=1.3.0 <1.4.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '|', ">=1.2.3 <1.4.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '|', ">=1.3.0 <2.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '|', ">=1.2.3 <2.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '|', ">=1.1.0 <=1.4.0").Returns(">=1.1.0 <2.0.0-0", true); // TODO: resugar as ^1.1.0
            New("^1.2.3", '|', ">1.1.0 <=1.4.0").Returns(">1.1.0 <2.0.0-0", true);
            New("^1.2.3", '|', ">=1.5.0 <3.0.0-0").Returns(">=1.2.3 <3.0.0-0", true);
            // Caret: intersection
            New("^1.2.3", '&', ">=1.0.0 <3.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '&', ">=1.2.3 <3.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '&', ">=1.0.0 <2.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '&', ">=1.2.3 <2.0.0-0").Returns("^1.2.3", true);
            New("^1.2.3", '&', ">=1.5.0 <2.0.0-0").Returns(">=1.5.0 <2.0.0-0"); // TODO: resugar as ^1.5.0
            New("^0.2.3", '&', ">=0.2.4 <1.0.0-0").Returns(">=0.2.4 <0.3.0-0"); // TODO: resugar as ^0.2.4
            New("^0.0.3", '&', ">=0.0.4-rc <0.1.0-0").Returns("<0.0.0-0");
            New("^0.0.3-a", '&', ">=0.0.3-b <0.1.0-0").Returns(">=0.0.3-b <0.0.4-0"); // TODO: resugar as ^0.0.3-b

            #endregion

            #region Operations with version ranges



            #endregion

            return adapter;
        }

        public sealed class OperationFixture(char operation, string left, string? right = null) : FuncFixture<VersionRange>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public OperationFixture() : this(default, null!) { }

            public char Operation { get; } = operation;
            public string Left { get; } = left;
            public string? Right { get; } = right;

            public string? Expected { get; private set; }

            public void Returns(string expected, bool isCommutative = false)
            {
                MarkAsComplete();
                Expected = expected;

                if (isCommutative) AddNew(new OperationFixture(Operation, Right!, Left)).Returns(expected);
            }

            public override void AssertResult(VersionRange? result)
                => Assert.Equal(Expected, result?.ToString());
            public override string ToString()
                => $"{base.ToString()} {(Right is null ? $"{Operation}({Left})" : $"({Left}) {Operation} ({Right})")} ⇒ {Expected}";

        }
    }
}
