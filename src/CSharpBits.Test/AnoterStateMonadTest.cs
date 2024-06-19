using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test;

delegate (TResult, decimal) StatefulComputation<in TSource, TResult>(TSource item, decimal total);
public class ApplicativeParsingTest{}
public class AnotherStateMonadTest
{
    [Fact]
    public void stateful_cart()
    {
        var cart = new StateMonadForCart.StatefulCart();

        cart.Add(2, "zucchine", 1.0m);
        cart.Add(1, "book", 20m);
        cart.Add(2, "wine", 5m);

        cart.Total.Should().Be(2 * 1.0m + 20m + 2 * 5m);
    }

    class Item
    {
        internal string Product { get; set; }
        internal int Quantity { get; set; }
        internal decimal Price { get; set; }

        internal Item(int quantity, string product, decimal price)
        {
            Quantity = quantity;
            Product = product;
            Price = price;
        }
    }

    [Fact]
    public void stateless_cart()
    {
        StatefulComputation<Item, string> cart = (item, total) =>
        (
            $"Adding {item.Product}",
            total + item.Price * item.Quantity
        );

        var (log1, tot1) = cart(new Item(2, "zucchine", 1.0m), 0);
        var (log2, tot2) = cart(new Item(1, "book", 20m), tot1);
        var (log3, tot3) = cart(new Item(2, "wine", 5.0m), tot2);

        log1.Should().Be("Adding zucchine");
        log2.Should().Be("Adding book");
        log3.Should().Be("Adding wine");
        tot3.Should().Be(2 * 1.0m + 20m + 2 * 5m);
    }

    [Fact]
    public void should_transform_generator()
    {
        StatefulComputation<Item, Item> cart = (item, total) =>
        (
            item,
            total + item.Price * item.Quantity
        );

        StatefulComputation<Item, string> cartString = (item, total) =>
        {
            var (result, newTotal) = cart(item, total);
            return ($"Adding {result.Product}", newTotal);
        };

        var (log1, tot1) = cartString(new Item(2, "zucchine", 1.0m), 0);
        var (log2, tot2) = cartString(new Item(1, "book", 20m), tot1);
        var (log3, tot3) = cartString(new Item(2, "wine", 5.0m), tot2);

        log1.Should().Be("Adding zucchine");
        log2.Should().Be("Adding book");
        log3.Should().Be("Adding wine");
        tot3.Should().Be(2 * 1.0m + 20m + 2 * 5m);

    }

    [Fact]
    public void should_transform_generator_using_Map()
    {
        StatefulComputation<Item, Item> cart = (item, total) =>
        (
            item,
            total + item.Price * item.Quantity
        );

        StatefulComputation<Item, string> cartString =
            cart
                .Map(result => $"Adding {result.Product}");


        var (log1, tot1) = cartString(new Item(2, "zucchine", 1.0m), 0);
        var (log2, tot2) = cartString(new Item(1, "book", 20m), tot1);
        var (log3, tot3) = cartString(new Item(2, "wine", 5.0m), tot2);

        log1.Should().Be("Adding zucchine");
        log2.Should().Be("Adding book");
        log3.Should().Be("Adding wine");
        tot3.Should().Be(2 * 1.0m + 20m + 2 * 5m);
    }
}


internal static class StateMonadForCart
{
    internal static StatefulComputation<TSource, TNewResult> Map<TSource, TResult, TNewResult>(
        this StatefulComputation<TSource, TResult> statefulComputation,
        Func<TResult, TNewResult> map)
    {
        return (item, total) =>
        {
            (TResult result, decimal newTotal) = statefulComputation(item, total);

            return (map(result), newTotal);
        };
    }


    internal class StatefulCart
    {
        public decimal Total { get; private set; }

        public void Add(int quantity, string product, decimal price)
        {
            Total += quantity * price;
        }
    }
}
