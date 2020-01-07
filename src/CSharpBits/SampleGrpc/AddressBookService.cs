using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CSharpBits.SampleGrpc
{
    public class AddressBookService : AddressBook.AddressBookBase
    {
        public override Task<AddressBookItems> GetAll(AddressRequest request, ServerCallContext context)
        {
            return Task.FromResult(new AddressBookItems
            {
                People = { new List<AddressBookItems.Types.Person>
                {
                    new AddressBookItems.Types.Person
                    {
                        Email = "some email",
                        Name = "Mario",
                        Phones = { new AddressBookItems.Types.Person.Types.PhoneNumber
                        {
                            Number = "349 45 33 666",
                            Type = AddressBookItems.Types.Person.Types.PhoneNumber.Types.PhoneType.Mobile
                        }}
                    }
                }}
            });
        }
    }
}