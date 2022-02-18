using System;
using Xunit;

namespace CSharpBits.Test
{
    struct NotAFunctor<A>
    {
        internal A V { get; }
        internal int Count { get; }

        private NotAFunctor(A a, int count)
        {
            V = a;
            Count = count;
        }

        internal static NotAFunctor<A> Return(A t) =>
            new NotAFunctor<A>(t, 0);

        internal static Func<NotAFunctor<A>, NotAFunctor<B>> Apply<B>(Func<A, B> f)
        {
            return functorA =>
                new NotAFunctor<B>(f(functorA.V), functorA.Count + 1);
        }
    }


    public class NotAFunctorTest
    {
        private Func<int, int> id = i => i;
        private readonly Func<int, int> twice =  i => i * 2;

        [Fact]
        void violation_of_identity()
        {
            var idM = NotAFunctor<int>.Apply(id);

            Assert.Equal(100, id(100));

            Assert.Equal(NotAFunctor<int>.Return(100).Count, idM(NotAFunctor<int>.Return(100)).Count);
            // Assert.Equal(NotAFunctor<int>.Return(100), idM(NotAFunctor<int>.Return(100)));
        }

        [Fact]
        void violation_of_associativity()
        {

        }
    }
}