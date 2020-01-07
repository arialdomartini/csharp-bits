using System.Collections.Generic;
using System.Linq;

namespace CSharpBits.CascadeOfIfs
{
    internal static class OperationsExtensions
    {
        internal static IRing ChainTogether(this IEnumerable<IOperation> operations, Check check) =>
            operations.Reverse()
                .Select(o => new OperationRing(check, o))
                .Aggregate((IRing) new PassAll(), (previous, ring) =>
                {
                    ring.Next = previous;
                    return ring;
                });
    }
}