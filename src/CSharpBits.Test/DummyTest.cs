using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    public class DummyTest
    {
        [Fact]
        void dummy_test()
        {
            "friends".Should().Be("friends");
        }
    }
}