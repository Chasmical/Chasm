﻿using Chasm.SemanticVersioning.Ranges;
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



            // Complement of primitives
            New(">1.2.3", '~').Returns("<=1.2.3");
            New(">=1.2.3", '~').Returns("<1.2.3");
            New("<1.2.3", '~').Returns(">=1.2.3");
            New("<=1.2.3", '~').Returns(">1.2.3");

            // Note: The complement of <0.0.0-0 is considered to be * instead of >=0.0.0-0,
            //       since <0.0.0-0 and * are essentially used as special symbols.
            New("<0.0.0-0", '~').Returns("*");
            New("*", '~').Returns("<0.0.0-0");

            // Complement of 0.0.0 primitives
            New("<0.0.0", '~').Returns(">=0.0.0");
            New(">=0.0.0", '~').Returns("<0.0.0");

            // Complement of ranges specifying all versions
            New(">=0.0.0-0", '~').Returns("<0.0.0-0");
            New("x.x", '~').Returns("<0.0.0-0");
            New("^x.x.x", '~').Returns("<0.0.0-0");
            New("~x.x.x", '~').Returns("<0.0.0-0");

            // Complement of ranges specifying no versions
            New("3.4.5 - 1.2.3", '~').Returns("*");
            New("<0.0.0", '~').Returns(">=0.0.0");



            // Intersection of primitives, same direction
            New(">1.2.3", '&', ">3.4.5").Returns(">3.4.5", true);
            New("<1.2.3", '&', "<3.4.5").Returns("<1.2.3", true);
            New("<=1.2.3", '&', "<1.2.3").Returns("<1.2.3", true);
            New(">=1.2.3", '&', ">1.2.3").Returns(">1.2.3", true);

            // Intersection of primitives, opposite directions
            New("<1.2.3", '&', ">3.4.5").Returns("<0.0.0-0", true);
            New(">1.2.3", '&', "<3.4.5").Returns(">1.2.3 <3.4.5");
            // TODO: figure out what to do about this case
            //New("<3.4.5", '&', ">1.2.3").Returns("<3.4.5 >1.2.3");

            New("<1.2.3", '&', ">1.2.3").Returns("<0.0.0-0", true);
            New("<=1.2.3", '&', ">1.2.3").Returns("<0.0.0-0", true);
            New(">=1.2.3", '&', "<1.2.3").Returns("<0.0.0-0", true);
            New("<=1.2.3", '&', ">=1.2.3").Returns("=1.2.3", true);

            // Intersection of primitives, with minimum version
            New("<0.0.0-0", '&', "<0.0.0-0").Returns("<0.0.0-0", true);
            New("<0.0.0-0", '&', "<0.0.0").Returns("<0.0.0-0", true);
            New("<0.0.0-0", '&', "<=0.0.0-0").Returns("<0.0.0-0", true);
            New("<0.0.0-0", '&', ">0.0.0-0").Returns("<0.0.0-0", true);
            New("<0.0.0-0", '&', ">=0.0.0-0").Returns("<0.0.0-0", true);



            // Union of primitives, same direction
            New(">1.2.3", '|', ">3.4.5").Returns(">1.2.3", true);
            New("<1.2.3", '|', "<3.4.5").Returns("<3.4.5", true);
            New(">=1.2.3", '|', ">1.2.3").Returns(">=1.2.3", true);
            New("<=1.2.3", '|', "<1.2.3").Returns("<=1.2.3", true);

            // Union of primitives, opposite directions
            New(">1.2.3", '|', "<3.4.5").Returns("*", true);
            New("<1.2.3", '|', ">3.4.5").Returns("<1.2.3 || >3.4.5");
            New(">3.4.5", '|', "<1.2.3").Returns(">3.4.5 || <1.2.3");

            New("<1.2.3", '|', ">1.2.3").Returns("<1.2.3 || >1.2.3");
            New(">1.2.3", '|', "<1.2.3").Returns(">1.2.3 || <1.2.3");
            New("<=1.2.3", '|', ">1.2.3").Returns("*", true);
            New("<1.2.3", '|', ">=1.2.3").Returns("*", true);
            New("<=1.2.3", '|', ">=1.2.3").Returns("*", true);

            // Union of primitives, with minimum version
            New("<0.0.0-0", '|', "<0.0.0-0").Returns("<0.0.0-0", true);
            New("<0.0.0-0", '|', "<0.0.0").Returns("<0.0.0", true);
            New("<0.0.0-0", '|', ">=0.0.0-0").Returns("*", true);
            New("<=0.0.0-0", '|', ">0.0.0-0").Returns("*", true);
            New("<0.0.0-0", '|', ">0.0.0-0").Returns("<0.0.0-0 || >0.0.0-0");
            New("<0.0.0-0", '|', ">=0.0.0").Returns("<0.0.0-0 || >=0.0.0");
            New(">=0.0.0", '|', "<0.0.0-0").Returns(">=0.0.0 || <0.0.0-0");



            // Complement of comparator sets
            New(">=1.2.3 <=1.8.2", '~').Returns("<1.2.3 || >1.8.2");
            New("^1.2.3 <=1.4.0", '~').Returns("<1.2.3 || >1.4.0");



            // Intersection of comparator sets of primitives
            New(">=1.2.3 <=1.6.0", '&', ">=1.5.0 <=1.8.2").Returns(">=1.5.0 <=1.6.0", true);
            New("<=1.6.0 >=1.2.3", '&', "<=1.8.2 >=1.5.0").Returns(">=1.5.0 <=1.6.0", true);
            New(">=1.2.3 <=1.6.0", '&', ">=1.4.0").Returns(">=1.4.0 <=1.6.0", true);
            New(">=1.2.3 <=1.6.0", '&', "<=1.4.0").Returns(">=1.2.3 <=1.4.0", true);

            // Intersection of comparators sets of advanced comparators
            New("^1.2.3 <1.5.0-0", '&', "~1.2 >1.0").Returns(">=1.2.3 <1.3.0-0", true);



            // Union of comparator sets of primitives
            New(">=1.2.3 <=1.6.0", '|', ">=1.5.0 <=1.8.2").Returns(">=1.2.3 <=1.8.2", true);
            New(">=1.2.3 <=1.5.0", '|', ">=1.6.0 <=1.8.2").Returns(">=1.2.3 <=1.5.0 || >=1.6.0 <=1.8.2");
            New(">=1.6.0 <=1.8.2", '|', ">=1.2.3 <=1.5.0").Returns(">=1.6.0 <=1.8.2 || >=1.2.3 <=1.5.0");

            New(">=1.2.3", '|', ">=1.6.0 <=1.8.2").Returns(">=1.2.3", true);
            New("<=2.5.0", '|', ">=1.6.0 <=1.8.2").Returns("<=2.5.0", true);
            New(">=1.2.3", '|', ">=1.1.0 <=1.2.0").Returns(">=1.2.3 || >=1.1.0 <=1.2.0");
            New(">=1.1.0 <=1.2.0", '|', ">=1.2.3").Returns(">=1.1.0 <=1.2.0 || >=1.2.3");

            // Union of comparator sets of advanced comparators
            New("^1.2.3", '|', "~2.5").Returns("^1.2.3 || ~2.5");
            New("~2.5", '|', "^1.2.3").Returns("~2.5 || ^1.2.3");
            New("^1.2.3", '|', "~1.5").Returns("^1.2.3", true);



            // TODO: Complement of version ranges

            // TODO: Intersection of version ranges

            // TODO: Union of version ranges



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
                => $"{base.ToString()} {(Right is null ? $"{Operation}({Left})" : $"{Left} {Operation} {Right}")} ⇒ {Expected}";

        }
    }
}
