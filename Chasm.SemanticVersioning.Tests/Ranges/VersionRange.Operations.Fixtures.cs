using Chasm.SemanticVersioning.Ranges;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<OperationFixture> CreateOperationFixtures()
        {
            FixtureAdapter<OperationFixture> adapter = [];

            OperationFixture New(string left, char op, string? right = null, [CallerLineNumber] int line = -1)
                => adapter.Add(new OperationFixture(op, left, right) { LineNumber = line });

            #region Operations with primitives

            // Complement: test comparison operators
            New(">1.2.3", '~').Returns("<=1.2.3");
            New("<1.2.3", '~').Returns(">=1.2.3");
            New(">=1.2.3", '~').Returns("<1.2.3");
            New("<=1.2.3", '~').Returns(">1.2.3");
            // Complement: test equality operators
            New("=1.2.3", '~').Returns("<1.2.3 || >1.2.3");
            New("1.2.3", '~').Returns("<1.2.3 || >1.2.3");
            // Complement: test special versions
            New("*", '~').Returns("<0.0.0-0");
            New("<0.0.0-0", '~').Returns("*");

            // Union: test basic precedence
            New(">1.2.3", '|', ">3.4.5").Returns(">1.2.3");
            New("<1.2.3", '|', "<3.4.5").Returns("<3.4.5");
            New(">=1.2.3", '|', ">=3.4.5").Returns(">=1.2.3");
            New("<=1.2.3", '|', "<=3.4.5").Returns("<=3.4.5");
            // Union: test same direction comparison operators
            New(">1.2.3", '|', ">=1.2.3").Returns(">=1.2.3");
            New("<1.2.3", '|', "<=1.2.3").Returns("<=1.2.3");
            New(">1.2.3", '|', ">=3.4.5").Returns(">1.2.3");
            New(">=1.2.3", '|', ">3.4.5").Returns(">=1.2.3");
            New("<3.4.5", '|', "<=1.2.3").Returns("<3.4.5");
            New("<=3.4.5", '|', "<1.2.3").Returns("<=3.4.5");
            // Union: test opposite direction comparison operators
            New(">1.2.3", '|', "<1.2.3").Returns(">1.2.3 || <1.2.3", false);
            New("<1.2.3", '|', ">1.2.3").Returns("<1.2.3 || >1.2.3", false);
            New(">1.2.3", '|', "<=1.2.3").Returns("*");
            New("<1.2.3", '|', ">=1.2.3").Returns("*");
            New(">=1.2.3", '|', "<=1.2.3").Returns("*");
            New("<1.2.3", '|', ">3.4.5").Returns("<1.2.3 || >3.4.5", false);
            New(">3.4.5", '|', "<1.2.3").Returns(">3.4.5 || <1.2.3", false);
            New(">1.2.3", '|', "<3.4.5").Returns("*");
            New(">=1.2.3", '|', "<3.4.5").Returns("*");
            New(">1.2.3", '|', "<=3.4.5").Returns("*");
            New(">=1.2.3", '|', "<=3.4.5").Returns("*");
            // Union: test equality operators
            New(">1.2.3", '|', "=3.4.5").Returns(">1.2.3");
            New("<1.2.3", '|', "=0.7.8").Returns("<1.2.3");
            // TODO: New(">1.2.3", '|', "=1.2.3").Returns(">=1.2.3");
            // TODO: New("<1.2.3", '|', "=1.2.3").Returns("<=1.2.3");
            New(">=1.2.3", '|', "=1.2.3").Returns(">=1.2.3");
            New("<=1.2.3", '|', "=1.2.3").Returns("<=1.2.3");
            New(">=1.2.3", '|', "=0.7.8").Returns(">=1.2.3 || =0.7.8", false);
            New("<=1.2.3", '|', "=3.4.5").Returns("<=1.2.3 || =3.4.5", false);
            // Union: test special versions
            New(">1.2.3", '|', "*").Returns("*");
            New("<1.2.3", '|', "*").Returns("*");
            New(">=1.2.3", '|', "*").Returns("*");
            New("<=1.2.3", '|', "*").Returns("*");
            New(">1.2.3", '|', "<0.0.0-0").Returns(">1.2.3");
            New("<1.2.3", '|', "<0.0.0-0").Returns("<1.2.3");
            New(">=1.2.3", '|', "<0.0.0-0").Returns(">=1.2.3");
            New("<=1.2.3", '|', "<0.0.0-0").Returns("<=1.2.3");

            // Intersection: test basic precedence
            New(">1.2.3", '&', ">3.4.5").Returns(">3.4.5");
            New("<1.2.3", '&', "<3.4.5").Returns("<1.2.3");
            New(">=1.2.3", '&', ">=3.4.5").Returns(">=3.4.5");
            New("<=1.2.3", '&', "<=3.4.5").Returns("<=1.2.3");
            // Intersection: test same direction comparison operators
            New(">1.2.3", '&', ">=1.2.3").Returns(">1.2.3");
            New("<1.2.3", '&', "<=1.2.3").Returns("<1.2.3");
            New(">1.2.3", '&', ">=3.4.5").Returns(">=3.4.5");
            New(">=1.2.3", '&', ">3.4.5").Returns(">3.4.5");
            New("<3.4.5", '&', "<=1.2.3").Returns("<=1.2.3");
            New("<=3.4.5", '&', "<1.2.3").Returns("<1.2.3");
            // Intersection: test opposite direction comparison operators
            New(">1.2.3", '&', "<1.2.3").Returns("<0.0.0-0");
            New(">1.2.3", '&', "<=1.2.3").Returns("<0.0.0-0");
            New(">=1.2.3", '&', "<1.2.3").Returns("<0.0.0-0");
            New(">=1.2.3", '&', "<=1.2.3").Returns("=1.2.3");
            New(">1.2.3", '&', "<3.4.5").Returns(">1.2.3 <3.4.5", false);
            New(">=1.2.3", '&', "<3.4.5").Returns(">=1.2.3 <3.4.5", false);
            New(">1.2.3", '&', "<=3.4.5").Returns(">1.2.3 <=3.4.5", false);
            New(">=1.2.3", '&', "<=3.4.5").Returns(">=1.2.3 <=3.4.5", false);
            New("<1.2.3", '&', ">3.4.5").Returns("<0.0.0-0");
            New("<=1.2.3", '&', ">3.4.5").Returns("<0.0.0-0");
            New("<1.2.3", '&', ">=3.4.5").Returns("<0.0.0-0");
            New("<=1.2.3", '&', ">=3.4.5").Returns("<0.0.0-0");
            // Intersection: test equality operators
            New("=1.2.3", '&', ">1.2.3").Returns("<0.0.0-0");
            New("=1.2.3", '&', ">3.4.5").Returns("<0.0.0-0");
            New("=1.2.3", '&', "<0.7.8").Returns("<0.0.0-0");
            New("=1.2.3", '&', ">=1.2.3").Returns("=1.2.3");
            New("=1.2.3", '&', "<=1.2.3").Returns("=1.2.3");
            // Intersection: test special versions
            New(">1.2.3", '&', "*").Returns(">1.2.3");
            New("<1.2.3", '&', "*").Returns("<1.2.3");
            New(">=1.2.3", '&', "*").Returns(">=1.2.3");
            New("<=1.2.3", '&', "*").Returns("<=1.2.3");
            New(">1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0");
            New("<1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0");
            New(">=1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0");
            New("<=1.2.3", '&', "<0.0.0-0").Returns("<0.0.0-0");

            #endregion

            #region Operations with comparator sets

            // Complement: test with comparator sets
            New(">1.2.3 <3.4.5", '~').Returns("<=1.2.3 || >=3.4.5");
            New(">1.2.3 <=3.4.5", '~').Returns("<=1.2.3 || >3.4.5");
            New(">=1.2.3 <3.4.5", '~').Returns("<1.2.3 || >=3.4.5");
            New(">=1.2.3 <=3.4.5", '~').Returns("<1.2.3 || >3.4.5");

            // Union: expanding from the right
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <3.4.5").Returns(">=1.2.3 <3.4.5");
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <=3.4.5").Returns(">=1.2.3 <=3.4.5");
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <2.0.0").Returns(">=1.2.3 <=2.0.0");
            New(">=1.2.3 <=2.0.0", '|', ">=1.5.0 <=2.0.0").Returns(">=1.2.3 <=2.0.0");
            // Union: expanding from the left
            New(">=1.2.3 <=2.0.0", '|', ">1.0.0 <=1.5.0").Returns(">1.0.0 <=2.0.0");
            New(">=1.2.3 <=2.0.0", '|', ">=1.0.0 <=1.5.0").Returns(">=1.0.0 <=2.0.0");
            New(">=1.2.3 <=2.0.0", '|', ">1.2.3 <=1.5.0").Returns(">=1.2.3 <=2.0.0");
            New(">=1.2.3 <=2.0.0", '|', ">=1.2.3 <=1.5.0").Returns(">=1.2.3 <=2.0.0");
            // Union: expanding from both ends
            New(">=1.2.3 <=2.0.0", '|', ">=1.0.0 <=3.4.5").Returns(">=1.0.0 <=3.4.5");

            // Intersection: contracting from the right
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <2.0.0").Returns(">=1.0.0 <2.0.0");
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <=2.0.0").Returns(">=1.0.0 <=2.0.0");
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <3.4.5").Returns(">=1.0.0 <3.4.5");
            New(">=1.0.0 <=3.4.5", '&', ">=0.5.0 <=3.4.5").Returns(">=1.0.0 <=3.4.5");
            // Intersection: contracting from the left
            New(">=1.0.0 <=3.4.5", '&', ">1.2.3 <=5.0.0").Returns(">1.2.3 <=3.4.5");
            New(">=1.0.0 <=3.4.5", '&', ">=1.2.3 <=5.0.0").Returns(">=1.2.3 <=3.4.5");
            New(">=1.0.0 <=3.4.5", '&', ">1.0.0 <=5.0.0").Returns(">1.0.0 <=3.4.5");
            New(">=1.0.0 <=3.4.5", '&', ">=1.0.0 <=5.0.0").Returns(">=1.0.0 <=3.4.5");
            // Intersection: contracting from both ends
            New(">=1.2.3 <=2.0.0", '&', ">=1.0.0 <=3.4.5").Returns(">=1.2.3 <=2.0.0");

            #endregion

            #region Operations with advanced comparators (caret)

            // Caret: complement
            New("^1.2.3", '~').Returns("<1.2.3 || >=2.0.0-0");
            New("^0.2.3", '~').Returns("<0.2.3 || >=0.3.0-0");
            New("^0.0.3", '~').Returns("<0.0.3 || >=0.0.4-0");
            New("^0.0.3-rc", '~').Returns("<0.0.3-rc || >=0.0.4-0");

            // Caret: union, no changes
            New("^1.2.3", '|', ">=1.3.0 <1.4.0-0").Returns("^1.2.3");
            New("^1.2.3", '|', ">=1.2.3 <1.4.0-0").Returns("^1.2.3");
            New("^1.2.3", '|', ">=1.3.0 <2.0.0-0").Returns("^1.2.3");
            New("^1.2.3", '|', ">=1.2.3 <2.0.0-0").Returns("^1.2.3");
            // Caret: union, expanding
            New("^1.2.3", '|', ">1.1.0 <=1.4.0").Returns(">1.1.0 <2.0.0-0");
            New("^1.2.3", '|', ">=1.1.0 <=1.4.0").Returns(">=1.1.0 <2.0.0-0"); // TODO: resugar as ^1.1.0
            New("^1.2.3", '|', ">=0.2.3 <=1.4.0").Returns(">=0.2.3 <2.0.0-0");
            New("^1.2.3", '|', ">=0.0.3 <=1.4.0").Returns(">=0.0.3 <2.0.0-0");
            New("^1.2.3", '|', ">=0.0.3-rc <=1.4.0").Returns(">=0.0.3-rc <2.0.0-0");
            New("^1.2.3", '|', ">=1.5.0 <=2.0.0-0").Returns(">=1.2.3 <=2.0.0-0");
            New("^1.2.3", '|', ">=1.5.0 <3.0.0-0").Returns(">=1.2.3 <3.0.0-0");

            // Caret: intersection, no changes
            New("^1.2.3", '&', ">=1.0.0 <3.0.0-0").Returns("^1.2.3");
            New("^1.2.3", '&', ">=1.2.3 <3.0.0-0").Returns("^1.2.3");
            New("^1.2.3", '&', ">=1.0.0 <2.0.0-0").Returns("^1.2.3");
            New("^1.2.3", '&', ">=1.2.3 <2.0.0-0").Returns("^1.2.3");
            // Caret: intersection, contracting
            New("^1.2.3", '&', ">1.5.0 <3.0.0-0").Returns(">1.5.0 <2.0.0-0");
            New("^1.2.3", '&', ">=1.1.0 <1.5.0-0").Returns(">=1.2.3 <1.5.0-0");
            New("^0.0.3", '&', ">=0.0.4-rc <0.1.0-0").Returns("<0.0.0-0");
            New("^1.2.3", '&', ">=1.5.0 <2.0.0-0").Returns(">=1.5.0 <2.0.0-0"); // TODO: resugar as ^1.5.0
            New("^0.2.3", '&', ">=0.2.4 <1.0.0-0").Returns(">=0.2.4 <0.3.0-0"); // TODO: resugar as ^0.2.4
            New("^0.0.3-a", '&', ">=0.0.3-b <0.1.0-0").Returns(">=0.0.3-b <0.0.4-0"); // TODO: resugar as ^0.0.3-b

            #endregion

            #region #region Operations with advanced comparators (hyphen)

            // Hyphen: complement
            New("1.2.3 - 3.4.5", '~').Returns("<1.2.3 || >3.4.5");
            New("1.2.x - 3.4", '~').Returns("<1.2.0 || >=3.5.0-0");
            New("1.x - 3.x.x", '~').Returns("<1.0.0 || >=4.0.0-0");
            New("1.2 - 3.4.5-rc", '~').Returns("<1.2.0 || >3.4.5-rc");
            New("* - 3.4.x", '~').Returns(">=3.5.0-0");
            New("1.2 - *", '~').Returns("<1.2.0");

            // Hyphen: union, no changes
            New("1.2.3 - 3.4.5", '|', ">=2.0.0 <=3.0.0").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '|', ">=1.2.3 <=3.0.0").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '|', ">=2.0.0 <=3.4.5").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '|', ">=1.2.3 <=3.4.5").Returns("1.2.3 - 3.4.5");
            New("1.x - 3.4", '|', ">=2.0.0 <=3.0.0").Returns("1.x - 3.4");
            New("1.x - 3.4", '|', ">=1.0.0 <=3.0.0").Returns("1.x - 3.4");
            New("1.x - 3.4", '|', ">=2.0.0 <3.5.0-0").Returns("1.x - 3.4");
            New("1.x - 3.4", '|', ">=1.0.0 <3.5.0-0").Returns("1.x - 3.4");
            // Hyphen: union, expanding from the left
            New("1.2 - 3.4.5", '|', ">0.7.8 <=2.0.0").Returns(">0.7.8 <=3.4.5");
            New("2.x - 3.4.x", '|', ">=1.5.3 <3.0.0-0").Returns(">=1.5.3 <3.5.0-0"); // TODO: resugar as 1.5.3 - 3.4.x
            New("2.x - 3.4.x", '|', ">=1.5.0 <3.0.0-0").Returns(">=1.5.0 <3.5.0-0"); // TODO: resugar as 1.5 - 3.4.x
            New("2.x - 3.4.x", '|', ">=1.0.3 <3.0.0-0").Returns(">=1.0.3 <3.5.0-0"); // TODO: resugar as 1.0.3 - 3.4.x
            New("2.x - 3.4.x", '|', ">=1.0.0 <3.0.0-0").Returns(">=1.0.0 <3.5.0-0"); // TODO: resugar as 1.x - 3.4.x
            New("2.x - 3.4.x", '|', ">=0.0.0 <3.0.0-0").Returns(">=0.0.0 <3.5.0-0"); // TODO: resugar as 0.x - 3.4.x
            // Hyphen: union, expanding from the right
            New("1.2 - 3.4.5", '|', ">=1.5.0 <3.6.7").Returns(">=1.2.0 <3.6.7");
            New("1.x - 3.4.x", '|', ">=1.5.0 <=3.6.7").Returns(">=1.0.0 <=3.6.7"); // TODO: resugar as 1.0 - 3.6.7
            New("1.x - 3.4.x", '|', ">=1.5.0 <3.7.0-0").Returns(">=1.0.0 <3.7.0-0"); // TODO: resugar as 1.0 - 3.6.x
            New("1.x - 3.4.x", '|', ">=1.5.0 <=3.7.0-0").Returns(">=1.0.0 <=3.7.0-0"); // TODO: resugar as 1.0 - 3.7.0-0
            New("1.x - 3.4.x", '|', ">=1.5.0 <4.0.0-0").Returns(">=1.0.0 <4.0.0-0"); // TODO: resugar as 1.0 - 3.x.x
            New("1.x - 3.4.x", '|', ">=1.5.0 <=4.0.0-0").Returns(">=1.0.0 <=4.0.0-0"); // TODO: resugar as 1.0 - 4.0.0-0

            // Hyphen: intersection, no changes
            New("1.2.3 - 3.4.5", '&', ">=1.0.0 <=5.0.0").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '&', ">=1.2.3 <=5.0.0").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '&', ">=1.0.0 <=3.4.5").Returns("1.2.3 - 3.4.5");
            New("1.2.3 - 3.4.5", '&', ">=1.2.3 <=3.4.5").Returns("1.2.3 - 3.4.5");
            New("1.x - 3.4", '&', ">=0.5.0 <5.0.0-0").Returns("1.x - 3.4");
            New("1.x - 3.4", '&', ">=1.0.0 <5.0.0-0").Returns("1.x - 3.4");
            New("1.x - 3.4", '&', ">=0.5.0 <3.5.0-0").Returns("1.x - 3.4");
            New("1.x - 3.4", '&', ">=1.0.0 <3.5.0-0").Returns("1.x - 3.4");
            // Hyphen: intersection, contracting from the left
            New("1.2 - 3.4.5", '&', ">1.5.2 <5.0.0-0").Returns(">1.5.2 <=3.4.5");
            New("1.2 - 3.4.5", '&', ">=1.5.2 <5.0.0-0").Returns(">=1.5.2 <=3.4.5"); // TODO: resugar as 1.5.2 - 3.4.5
            New("1.x - 3.4", '&', ">=2.5.3 <5.0.0-0").Returns(">=2.5.3 <3.5.0-0"); // TODO: resugar as 2.5.3 - 3.4
            New("1.x - 3.4", '&', ">=2.5.0 <5.0.0-0").Returns(">=2.5.0 <3.5.0-0"); // TODO: resugar as 2.5 - 3.4
            New("1.x - 3.4", '&', ">=2.0.3 <5.0.0-0").Returns(">=2.0.3 <3.5.0-0"); // TODO: resugar as 2.0.3 - 3.4
            New("1.x - 3.4", '&', ">=2.0.0 <5.0.0-0").Returns(">=2.0.0 <3.5.0-0"); // TODO: resugar as 2.x - 3.4
            // Hyphen: intersection, contracting from the right
            New("1.2 - 3.4.5", '&', ">=0.5.0 <3.2.3").Returns(">=1.2.0 <3.2.3");
            New("1.2 - 3.4.5", '&', ">=0.5.0 <=3.2.3").Returns(">=1.2.0 <=3.2.3"); // TODO: resugar as 1.2 - 3.2.3
            New("1.x - 3.4", '&', ">=1.0.0 <=3.4.5").Returns(">=1.0.0 <=3.4.5"); // TODO: resugar as 1.x - 3.4.5
            New("1.x - 3.4", '&', ">=1.0.0 <=3.4.0").Returns(">=1.0.0 <=3.4.0"); // TODO: resugar as 1.x - 3.4.0
            New("1.x - 3.4", '&', ">=1.0.0 <3.2.0-0").Returns(">=1.0.0 <3.2.0-0"); // TODO: resugar as 1.x - 3.1
            New("1.x - 3.4", '&', ">=1.0.0 <=3.0.3").Returns(">=1.0.0 <=3.0.3"); // TODO: resugar as 1.x - 3.0.3
            New("1.x - 3.4", '&', ">=1.0.0 <3.0.0-0").Returns(">=1.0.0 <3.0.0-0"); // TODO: resugar as 1.x - 2.x

            #endregion

            #region #region Operations with advanced comparators (tilde)

            // Tilde: complement
            New("~1.2.3-rc", '~').Returns("<1.2.3-rc || >=1.3.0-0");
            New("~1.2.3", '~').Returns("<1.2.3 || >=1.3.0-0");
            New("~1.2", '~').Returns("<1.2.0 || >=1.3.0-0");
            New("~1.x", '~').Returns("<1.0.0 || >=2.0.0-0");
            New("~1", '~').Returns("<1.0.0 || >=2.0.0-0");

            // Tilde: union, no changes
            New("~1.2.3", '|', ">=1.2.5 <=1.2.8").Returns("~1.2.3");
            New("~1.2.3", '|', ">=1.2.3 <=1.2.8").Returns("~1.2.3");
            New("~1.2.3", '|', ">=1.2.5 <1.3.0-0").Returns("~1.2.3");
            New("~1.2.3", '|', ">=1.2.5 <1.3.0-0").Returns("~1.2.3");
            // Tilde: union, expanding
            New("~1.2.3", '|', ">1.2.1 <=1.2.5").Returns(">1.2.1 <1.3.0-0");
            New("~1.2.3", '|', ">=1.2.1 <=1.2.5").Returns(">=1.2.1 <1.3.0-0"); // TODO: resugar as ~1.2.1
            New("~1.2.3", '|', ">=1.1.0 <=1.2.5").Returns(">=1.1.0 <1.3.0-0");
            New("~1.2.3", '|', ">=0.1.0 <=1.2.5").Returns(">=0.1.0 <1.3.0-0");
            New("~1.2.3", '|', ">=1.2.5 <=1.4.0").Returns(">=1.2.3 <=1.4.0");
            New("~1.2.3", '|', ">=1.2.5 <1.4.0-0").Returns(">=1.2.3 <1.4.0-0");
            New("~1.2.3", '|', ">=1.0.0 <2.0.0-0").Returns(">=1.0.0 <2.0.0-0"); // TODO: resugar as ~1.x.x
            New("~1.x", '|', ">=1.2.5 <3.0.0-0").Returns(">=1.0.0 <3.0.0-0");

            // Tilde: intersection, no changes
            New("~1.2.3", '&', ">=1.0.0 <2.0.0-0").Returns("~1.2.3");
            New("~1.2.3", '&', ">=1.2.3 <2.0.0-0").Returns("~1.2.3");
            New("~1.2.3", '&', ">=1.0.0 <1.3.0-0").Returns("~1.2.3");
            New("~1.2.3", '&', ">=1.2.3 <1.3.0-0").Returns("~1.2.3");
            // Tilde: intersection, contracting
            New("~1.2.3", '&', ">1.2.7 <1.5.0-0").Returns(">1.2.7 <1.3.0-0");
            New("~1.2.3", '&', ">=1.2.7 <1.5.0-0").Returns(">=1.2.7 <1.3.0-0"); // TODO: resugar as ~1.2.7
            New("~1.2.3", '&', ">=1.2.7-rc <1.5.0-0").Returns(">=1.2.7-rc <1.3.0-0"); // TODO: resugar as ~1.2.7-rc
            New("~1.x.x", '&', ">1.4.5 <1.5.0-0").Returns(">1.4.5 <1.5.0-0");
            New("~1.x", '&', ">=1.4.0 <1.5.0-0").Returns(">=1.4.0 <1.5.0-0"); // TODO: resugar as ~1.4
            New("~1.x", '&', ">=1.4.5 <1.5.0-0").Returns(">=1.4.5 <1.5.0-0"); // TODO: resugar as ~1.4.5
            New("~1.x", '&', ">=1.4.5-rc <1.5.0-0").Returns(">=1.4.5-rc <1.5.0-0"); // TODO: resugar as ~1.4.5-rc

            #endregion

            #region #region Operations with advanced comparators (X-Range)

            // X-Range: complement
            New("=1.2.x", '~').Returns("<1.2.0 || >=1.3.0-0");
            New("=1.x.x", '~').Returns("<1.0.0 || >=2.0.0-0");
            New("=x.x.x", '~').Returns("<0.0.0-0");

            New(">1.2.x", '~').Returns("<1.3.0");
            New(">1.x.x", '~').Returns("<2.0.0");
            New(">x.x.x", '~').Returns("*");
            New("<1.2.x", '~').Returns(">=1.2.0-0");
            New("<1.x.x", '~').Returns(">=1.0.0-0");
            New("<x.x.x", '~').Returns("*");

            New(">=1.2.x", '~').Returns("<1.2.0");
            New(">=1.x.x", '~').Returns("<1.0.0");
            New(">=x.x.x", '~').Returns("<0.0.0-0");
            New("<=1.2.x", '~').Returns(">=1.3.0-0");
            New("<=1.x.x", '~').Returns(">=2.0.0-0");
            New("<=x.x.x", '~').Returns("<0.0.0-0");

            // X-Range: union, no changes
            New("=1.2.x", '|', ">=1.2.3 <1.2.8-0").Returns("=1.2.x");
            New("=1.2.x", '|', ">=1.2.0 <1.2.8-0").Returns("=1.2.x");
            New("=1.2.x", '|', ">=1.2.3 <1.3.0-0").Returns("=1.2.x");
            New("=1.2.x", '|', ">=1.2.0 <1.3.0-0").Returns("=1.2.x");

            New(">1.2.x", '|', ">=1.3.0").Returns(">1.2.x");
            New(">1.2.x", '|', ">=1.3.0 <2.0.0-0").Returns(">1.2.x");
            New("<1.2.x", '|', "<1.2.0-0").Returns("<1.2.x");
            New("<1.2.x", '|', ">=1.0.0 <1.2.0-0").Returns("<1.2.x");

            New(">=1.2.x", '|', ">=1.2.0").Returns(">=1.2.x");
            New(">=1.2.x", '|', ">=1.2.0 <1.3.0-0").Returns(">=1.2.x");
            New("<=1.2.x", '|', "<1.3.0-0").Returns("<=1.2.x");
            New("<=1.2.x", '|', ">=1.0.0 <1.3.0-0").Returns("<=1.2.x");

            // X-Range: union, expanding and resugaring
            New("=1.2.x", '|', ">1.0.0 <2.0.0-0").Returns(">1.0.0 <2.0.0-0");
            New("=1.2.x", '|', ">=1.0.0 <2.0.0-0").Returns(">=1.0.0 <2.0.0-0"); // TODO: resugar as =1.x.x
            New("=1.2.x", '|', ">=1.0.0 <=2.0.0-0").Returns(">=1.0.0 <=2.0.0-0");

            New(">1.2.x", '|', ">1.1.3").Returns(">1.1.3");
            New(">1.2.x", '|', ">=1.1.3").Returns(">=1.1.3");
            New(">1.2.x", '|', ">1.1.0").Returns(">1.1.0");
            New(">1.2.x", '|', ">=1.1.0").Returns(">=1.1.0"); // TODO: resugar as >1.0.x
            New(">1.2.x", '|', ">=1.0.0").Returns(">=1.0.0"); // TODO: resugar as >0.x.x

            New("<1.2.x", '|', "<1.5.3").Returns("<1.5.3");
            New("<1.2.x", '|', "<=1.5.3").Returns("<=1.5.3");
            New("<1.2.x", '|', "<=1.5.0").Returns("<=1.5.0");
            New("<1.2.x", '|', "<1.5.0-0").Returns("<1.5.0-0"); // TODO: resugar as <1.5.x
            New("<1.2.x", '|', "<2.0.0-0").Returns("<2.0.0-0"); // TODO: resugar as <2.0.x

            New(">=1.2.x", '|', ">1.1.3").Returns(">1.1.3");
            New(">=1.2.x", '|', ">=1.1.3").Returns(">=1.1.3");
            New(">=1.2.x", '|', ">=1.1.0-0").Returns(">=1.1.0-0");
            New(">=1.2.x", '|', ">=1.1.0").Returns(">=1.1.0"); // TODO: resugar as >=1.1.x
            New(">=1.2.x", '|', ">=1.0.0").Returns(">=1.0.0"); // TODO: resugar as >=1.0.x

            New("<=1.2.x", '|', "<1.5.3").Returns("<1.5.3");
            New("<=1.2.x", '|', "<=1.5.3").Returns("<=1.5.3");
            New("<=1.2.x", '|', "<=1.5.0").Returns("<=1.5.0");
            New("<=1.2.x", '|', "<1.5.0-0").Returns("<1.5.0-0"); // TODO: resugar as <=1.4.x
            New("<=1.2.x", '|', "<2.0.0-0").Returns("<2.0.0-0"); // TODO: resugar as <=1.x.x

            // X-Range: intersection, no changes
            New("=1.2.x", '&', ">=1.0.0 <1.5.0-0").Returns("=1.2.x");
            New("=1.2.x", '&', ">=1.2.0 <1.5.0-0").Returns("=1.2.x");
            New("=1.2.x", '&', ">=1.0.0 <1.3.0-0").Returns("=1.2.x");
            New("=1.2.x", '&', ">=1.2.0 <1.3.0-0").Returns("=1.2.x");

            // X-Range: intersection, contracting and resugaring
            New("=1.2.x", '&', ">1.2.3").Returns(">1.2.3 <1.3.0-0");
            New("=1.2.x", '&', ">=1.2.3").Returns(">=1.2.3 <1.3.0-0");
            New("=1.2.x", '&', "<=1.2.7").Returns(">=1.2.0 <=1.2.7");
            New("=1.2.x", '&', "<1.2.7").Returns(">=1.2.0 <1.2.7");
            New("=1.2.x", '&', "<1.2.7-0").Returns(">=1.2.0 <1.2.7-0");
            New("=1.2.x", '&', ">=1.2.3 <1.2.8-0").Returns(">=1.2.3 <1.2.8-0");
            New("=1.x.x", '&', ">1.2.0").Returns(">1.2.0 <2.0.0-0");
            New("=1.x.x", '&', ">=1.2.0").Returns(">=1.2.0 <2.0.0-0");
            New("=1.x.x", '&', "<=1.8.0").Returns(">=1.0.0 <=1.8.0");
            New("=1.x.x", '&', "<1.8.0").Returns(">=1.0.0 <1.8.0");
            New("=1.x.x", '&', "<1.8.0-0").Returns(">=1.0.0 <1.8.0-0");
            New("=1.x.x", '&', ">=1.2.0 <1.5.0-0").Returns(">=1.2.0 <1.5.0-0");
            New("=1.x.x", '&', ">=1.2.3 <1.3.0-0").Returns(">=1.2.3 <1.3.0-0");
            New("=1.x.x", '&', ">=1.2.0 <1.3.0-0").Returns(">=1.2.0 <1.3.0-0"); // TODO: resugar as =1.2.x

            New(">1.2.x", '&', ">1.5.0").Returns(">1.5.0");
            New(">1.2.x", '&', ">=1.5.0").Returns(">=1.5.0"); // TODO: resugar as >1.4.x
            New(">1.2.x", '&', ">=2.3.0").Returns(">=2.3.0"); // TODO: resugar as >2.2.x
            New(">1.2.x", '&', ">=1.5.3").Returns(">=1.5.3");
            New(">1.2.x", '&', ">=2.0.0").Returns(">=2.0.0"); // TODO: resugar as >1.x.x
            New(">1.2.x", '&', ">=2.0.3").Returns(">=2.0.3");
            New(">1.2.x", '&', "<1.4.0-0").Returns(">=1.3.0 <1.4.0-0"); // TODO: resugar as =1.3.x
            New(">1.2.x", '&', "<1.4.3-0").Returns(">=1.3.0 <1.4.3-0");

            New(">1.x.x", '&', ">=2.5.0").Returns(">=2.5.0"); // TODO: resugar as >2.4.x
            New(">1.x.x", '&', ">=2.1.0").Returns(">=2.1.0"); // TODO: resugar as >2.0.x
            New(">1.x.x", '&', "<3.0.0-0").Returns(">=2.0.0 <3.0.0-0"); // TODO: resugar as =2.x.x
            New(">1.x.x", '&', ">=2.5.0 <3.0.0-0").Returns(">=2.5.0 <3.0.0-0");
            New(">1.x.x", '&', ">=2.5.0 <2.6.0-0").Returns(">=2.5.0 <2.6.0-0"); // TODO: resugar as =2.5.x

            New("<1.2.x", '&', "<1.1.0").Returns("<1.1.0");
            New("<1.2.x", '&', "<1.1.0-0").Returns("<1.1.0-0"); // TODO: resugar as <1.1.x
            New("<1.2.x", '&', "<0.5.0-0").Returns("<0.5.0-0"); // TODO: resugar as <0.5.x
            New("<1.2.x", '&', "<0.5.6-0").Returns("<0.5.6-0");
            New("<1.2.x", '&', "<1.0.0-0").Returns("<1.0.0-0"); // TODO: resugar as <1.0.x (<1.x.x is equivalent)
            New("<1.2.x", '&', "<1.1.3-0").Returns("<1.1.3-0");
            New("<1.2.x", '&', ">=1.1.0").Returns(">=1.1.0 <1.2.0-0"); // TODO: resugar as =1.1.x
            New("<1.2.x", '&', ">=1.1.3").Returns(">=1.1.3 <1.2.0-0");

            New("<2.x.x", '&', "<1.2.0-0").Returns("<1.2.0-0"); // TODO: resugar as <1.2.x
            New("<2.x.x", '&', "<1.0.0-0").Returns("<1.0.0-0"); // TODO: resugar as <1.x.x
            New("<2.x.x", '&', ">=1.0.0").Returns(">=1.0.0 <2.0.0-0"); // TODO: resugar as =1.x.x
            New("<2.x.x", '&', ">=1.5.0").Returns(">=1.5.0 <2.0.0-0");
            New("<2.x.x", '&', ">=1.5.0 <1.6.0-0").Returns(">=1.5.0 <1.6.0-0"); // TODO: resugar as =1.5.x

            New(">=1.2.x", '&', ">1.4.0").Returns(">1.4.0");
            New(">=1.2.x", '&', ">=1.4.0").Returns(">=1.4.0"); // TODO: resugar as >=1.4.x
            New(">=1.2.x", '&', ">=2.3.0").Returns(">=2.3.0"); // TODO: resugar as >=2.3.x
            New(">=1.2.x", '&', ">=1.4.3").Returns(">=1.4.3");
            New(">=1.2.x", '&', ">=2.0.0").Returns(">=2.0.0"); // TODO: resugar as >=2.0.x (>=2.x.x is equivalent)
            New(">=1.2.x", '&', ">=2.0.3").Returns(">=2.0.3");
            New(">=1.2.x", '&', "<1.3.0-0").Returns(">=1.2.0 <1.3.0-0"); // TODO: resugar as =1.2.x
            New(">=1.2.x", '&', "<1.3.4-0").Returns(">=1.2.0 <1.3.4-0");

            New(">=1.x.x", '&', ">=1.2.0").Returns(">=1.2.0"); // TODO: resugar as >=1.2.x
            New(">=1.x.x", '&', ">=2.0.0").Returns(">=2.0.0"); // TODO: resugar as >=2.x.x
            New(">=1.x.x", '&', "<2.0.0-0").Returns(">=1.0.0 <2.0.0-0"); // TODO: resugar as =1.x.x
            New(">=1.x.x", '&', "<2.4.0-0").Returns(">=1.0.0 <2.4.0-0");
            New(">=1.x.x", '&', ">=2.3.0 <2.4.0-0").Returns(">=2.3.0 <2.4.0-0"); // TODO: resugar as =2.3.x

            New("<=1.2.x", '&', "<1.2.0").Returns("<1.2.0");
            New("<=1.2.x", '&', "<1.2.0-0").Returns("<1.2.0-0"); // TODO: resugar as <=1.1.x
            New("<=1.2.x", '&', "<0.6.0-0").Returns("<0.6.0-0"); // TODO: resugar as <=0.5.x
            New("<=1.2.x", '&', "<0.6.3-0").Returns("<0.6.3-0");
            New("<=1.2.x", '&', "<1.0.0-0").Returns("<1.0.0-0"); // TODO: resugar as <=0.x.x
            New("<=1.2.x", '&', "<1.0.3-0").Returns("<1.0.3-0");
            New("<=1.2.x", '&', ">=1.2.0").Returns(">=1.2.0 <1.3.0-0"); // TODO: resugar as =1.2.x
            New("<=1.2.x", '&', ">=1.2.3").Returns(">=1.2.3 <1.3.0-0");

            New("<=2.x.x", '&', "<1.6.0-0").Returns("<1.6.0-0"); // TODO: resugar as <=1.5.x
            New("<=2.x.x", '&', "<2.0.0-0").Returns("<2.0.0-0"); // TODO: resugar as <=1.x.x
            New("<=1.x.x", '&', ">=1.0.0").Returns(">=1.0.0 <2.0.0-0"); // TODO: resugar as =1.x.x
            New("<=1.x.x", '&', ">=1.3.0").Returns(">=1.3.0 <2.0.0-0");
            New("<=1.x.x", '&', ">=1.7.0 <1.8.0-0").Returns(">=1.7.0 <1.8.0-0"); // TODO: resugar as =1.7.x

            #endregion

            #region Operations with version ranges

            // Ranges: complement, simple
            New("<1.2.3 || >=3.4.5", '~').Returns(">=1.2.3 <3.4.5");
            New("<=1.2.3 || >3.4.5", '~').Returns(">1.2.3 <=3.4.5");
            // Ranges: special ranges
            New("<1.2.3 || >1.2.3", '~').Returns("=1.2.3");
            New(">=1.2.3 || <=2.5.0", '~').Returns("<0.0.0-0");
            New("<1.2.3 || >=1.2.3", '~').Returns("<0.0.0-0");
            New("<=1.2.3 || >1.2.3", '~').Returns("<0.0.0-0");
            New(">=5.0.0 <2.3.0", '~').Returns("*");
            New("5.0 - 2.3 || >=5 <7 <=3", '~').Returns("*");
            // Ranges: complement, more sets
            New(">=1.2.3 <2.3.4 || >5.6.0", '~').Returns("<1.2.3 || >=2.3.4 <=5.6.0");
            New("<=2.3.4 || >=4.5.0 <5.0.3", '~').Returns(">2.3.4 <4.5.0 || >=5.0.3");
            New("<1.2.0 || <=2.3.4 || >=4.5.0 <5.0.3", '~').Returns(">2.3.4 <4.5.0 || >=5.0.3");
            New("<2.7.0 || <=2.3.4 || >=4.5.0 <5.0.3", '~').Returns(">=2.7.0 <4.5.0 || >=5.0.3");

            // Ranges: union, simple
            New("^1.2.3", '|', ">3.4.5").Returns("^1.2.3 || >3.4.5", false);
            New(">=1.2.0 <2.0.0", '|', ">2.0.0").Returns(">=1.2.0 <2.0.0 || >2.0.0", false);
            // Ranges: union, special ranges
            New("^1.2.3 || >=3.4.5", '|', "<0.0.0-0").Returns("^1.2.3 || >=3.4.5");
            New("<0.0.0-0 || ^1.2.3 || <0.0.0-0", '|', "<0.0.0-0 || <0.0.0-0").Returns("^1.2.3");
            New("^1.2.3 || * || >=3.4.5", '|', "*").Returns("*");
            // Ranges: union, more sets
            New("<0.2.3 || ~1.2 || ^2.4", '|', "<0.1.0 || ~1.4 || <0.0.0-0")
                .Returns("<0.2.3 || ~1.2 || ^2.4 || ~1.4", false);
            New("<0.2.3 || ~1.2 || ^2.4", '|', "<0.1.0 || ~1 || <0.0.0-0")
                .Returns("<0.2.3 || ~1 || ^2.4");
            New(">=1.0.0 <1.3.0-0 || >=2.0.0 <2.4.0-0 || >=2.6.0 <2.9.0-0", '|', ">=1.2.0 <2.1.0-0 || >=2.3.0 <2.7.0-0")
                .Returns(">=1.0.0 <2.9.0-0");

            // Ranges: intersection, simple
            New("<0.9.0-0 || >=1.2.0", '&', ">=0.5.0 <2.0.0-0").Returns(">=0.5.0 <0.9.0-0 || >=1.2.0 <2.0.0-0");
            // Ranges: intersection, special ranges
            New("<0.0.0-0 || ^1.2.3 || <0.0.0-0", '&', "<0.0.0-0 || ~1.3").Returns("~1.3");
            New("^1.2.3 || * || ~2.4", '&', ">=1.4.0 <2.8.0-0 || *").Returns("*");
            // Ranges: intersection, more sets
            New(">=1.2.0 <1.4.0-0 || >=1.5.0 <1.9.0-0", '&', ">=1.3.0 <1.8.5-0")
                .Returns(">=1.3.0 <1.4.0-0 || >=1.5.0 <1.8.5-0");
            New(">=1.0.0 <1.3.0-0 || >=2.0.0 <3.0.0-0", '&', ">=1.2.0 <2.4.0-0 || >=2.8.0 <2.9.0-0")
                .Returns(">=1.2.0 <1.3.0-0 || >=2.0.0 <2.4.0-0 || >=2.8.0 <2.9.0-0");

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
            public bool IsCommutative { get; private set; }

            public void Returns(string expected, bool isCommutative = true)
            {
                MarkAsComplete();
                Expected = expected;
                IsCommutative = isCommutative;
            }

            public override void AssertResult(VersionRange? result)
                => Assert.Equal(Expected, result?.ToString());
            public override string ToString()
                => $"{base.ToString()} {(Right is null ? $"{Operation}({Left})" : $"({Left}) {Operation} ({Right})")} ⇒ {Expected}";

        }
    }
}
