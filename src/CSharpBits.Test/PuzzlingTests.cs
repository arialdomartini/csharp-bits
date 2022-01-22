using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static CSharpBits.Test.TestHelper;

namespace CSharpBits.Test
{
    public class PuzzlingTest
    {
        class TargetClass
        {
            internal void SomeMethod(IEnumerable<int> numbers)
            {
                // [...]
                numbers.Select(_ => "I'm just accessing the collection of numbers").ToList();
                // [...]
            }
        }

        string testResult = "arbitraryCode() hasn't been run";

        [Fact]
        void function_injection_via_IEnumerable()
        {
            IEnumerable<int> arbitraryCode()
            {
                testResult = "Hey! arbitraryCode() was executed";
                yield return 0;
            }
            IEnumerable<int> looksLikeNumbers = arbitraryCode();
            Assert.Equal("arbitraryCode() hasn't been run", testResult);

            new TargetClass()
                .SomeMethod(looksLikeNumbers);

            testResult.Should().Be("Hey! arbitraryCode() was executed");
        }

        [Fact]
        public void intermittent_test2()
        {
            IEnumerable<string> strings = SomeTexts;
            Assert.Equal(new[]{"foo", "bar", "baz"}, strings.ToArray());

            var toUpper1 = strings.Select(s => s.ToUpper());
            Assert.Equal(new[]{"FOO", "BAR", "BAZ"}, toUpper1.ToArray());


            "".Invoking(_ =>
            {
                strings.Select(s => s.ToUpper() + "")
                    .ToArray();

            }).Should()
                .Throw<PlatformNotSupportedException>();


            var toUpper3 = strings.Select(s => "" + s.ToUpper());
            Assert.Equal(new[]{"FOO", "BAR", "BAZ"}, toUpper3.ToArray());
        }


        [Fact]
        public void intermittent_test()
        {
            IEnumerable<string> strings = SomeStrings;
            Assert.Equal(new[]{"Foo", "Bar", "Baz"}, strings.ToArray());

            var test1 = strings.Select(s => new string(s.ToCharArray()));
            Assert.Equal(new[]{"Foo", "Bar", "Baz"}, test1.ToArray());


            var test2 = strings.Select(s => new string(s.ToCharArray())+ "");
            Assert.Equal(new[] {
                "Argument Foo of type System.String is not assignable to parameter type int32",
                "Argument Bar of type System.String is not assignable to parameter type int32",
                "Constructor 'Baz' has 0 parameter(s) but is invoked with 0 argument(s)"
            }, test2.ToArray());

            var test3 = strings.Select(s => "" + new string(s.ToCharArray()));
            Assert.Equal(new[]{"Foo", "Bar", "Baz"}, test3.ToArray());
        }
    }

    internal static class TestHelper
    {
        private static int count1;
        internal static IEnumerable<string> SomeTexts
        {
            get
            {
                count1++;
                if (count1 == 1 || count1 % 2 == 0)
                {
                    yield return "foo";
                    yield return "bar";
                    yield return "baz";

                    yield break;
                }

                throw new PlatformNotSupportedException();
            }
        }

        private static int count2;
        internal static IEnumerable<string> SomeStrings
        {
            get
            {
                count2++;
                if (count2 == 1 || count2 % 2 == 0)
                {
                    yield return "Foo";
                    yield return "Bar";
                    yield return "Baz";
                }
                else
                {
                    yield return "Argument Foo of type System.String is not assignable to parameter type int32";
                    yield return "Argument Bar of type System.String is not assignable to parameter type int32";
                    yield return "Constructor 'Baz' has 0 parameter(s) but is invoked with 0 argument(s)";
                }


            }
        }
    }


}