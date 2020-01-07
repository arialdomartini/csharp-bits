using System.Threading.Tasks;
using CSharpBits.SampleGrpc;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.SampleGrpc
{
    public class AddressBookTest : GrcpTest<Startup>
    {
        [Fact]
        async Task invokes_AddressBookService()
        {
            using (var channel = GetChannel())
            {
                var client = new AddressBook.AddressBookClient(channel);

                var response = await client.GetAllAsync(new AddressRequest());

                response.People.Count.Should().Be(1);
            }
        }
    }
}