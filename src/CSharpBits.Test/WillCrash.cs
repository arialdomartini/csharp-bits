using FluentAssertions;
using IWillCrash;
using Xunit;

namespace CSharpBits.Test
{
    public class WillCrash
    {
        [Fact]
        void does_crash()
        {
            var _ = new Foo();
            true.Should().BeTrue();
        }
    }
}