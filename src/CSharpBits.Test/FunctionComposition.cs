using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using A = System.Int32;
using B = System.Int32;
using C = System.Int32;
using D = System.Int32;

namespace CSharpBits.Test
{
    [SuppressMessage("ReSharper", "ConvertToLocalFunction")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    public class FunctionComposition
    {
        [Fact]
        void composing_pure_functions()
        {
            Func<A, B> f1 = i => i;
            Func<B, C> f2 = i => i;
            Func<C, D> f3 = i => i;

            // f3 . (f2 . f1)
            Func<A, C> f21 = a => f2(f1(a));
            Func<A, D> f3_21 = a => f3(f21(a));

            // (f3 . f2) . f1
            Func<B, D> f32 = b => f3(f2(b));
            Func<A, D> f32_1 = a => f32(f1(a));

            var r1 = f3_21(42);
            var r2 = f32_1(42);

            Assert.Equal(r1, r2);
        }

        [Fact]
        void composing_impure_functions()
        {
            var state = 0;

            Func<A, B> f1 = i =>
            {
                state = 90;
                return i + state ;
            };
            Func<B, C> f2 = i =>
            {
                state *= 2;
                return i + state;
            };
            Func<C, D> f3 = i =>
            {
                state /= 2;
                return i * state;
            };

            // f3 . (f2 . f1)
            Func<A, C> f21 = a =>
            {
                var b = f1(a);
                var c = f2(b);
                return c;
            };

            Func<A, D> f3_21 = a =>
            {
                var c = f21(a);
                var d = f3(c);
                return d;
            };

            // (f3 . f2) . f1
            Func<B, D> f32 = b =>
            {
                var c = f2(b);
                var d = f3(c);
                return d;
            };
            Func<A, D> f32_1 = a =>
            {
                var b = f1(a);
                var d = f32(b);
                return d;
            };




            state = 0;
            var r1 = f3_21(42);



            state = 0;
            var r2 = f32_1(42);

            Assert.Equal(r1, r2);
            // Assert.Equal(state, "123123");
        }
    }
}