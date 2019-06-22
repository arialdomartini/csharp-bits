using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    public class AnoterStateMonadTest
    {
        [Fact]
        public void stateful_cart()
        {
            var cart = new StatefulCart();

            cart.Add(2, "zucchine", 1.0m);
            cart.Add(1, "book", 20m);
            cart.Add(2, "wine", 5m);

            cart.Total.Should().Be(2 * 1.0m + 20m + 2 * 5m);
        }

        class Item
        {
            public string Product { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }

            public Item(int quantity, string product, decimal price)
            {
                Quantity = quantity;
                Product = product;
                Price = price;
            }
        }

        delegate (decimal, string) StatefulComputation(Item item, decimal total);

        [Fact]
        public void stateless_cart()
        {
            StatefulComputation cart = (item, total) => (
                total + item.Price * item.Quantity,
                $"Adding {item.Product}");

            var (tot1, log1) = cart(new Item(2, "zucchine", 1.0m), 0);
            var (tot2, log2) = cart(new Item(1, "book", 20m), tot1);
            var (tot3, log3) = cart(new Item(2, "wine", 5.0m), tot2);

            log1.Should().Be("Adding zucchine");
            log2.Should().Be("Adding book");
            log3.Should().Be("Adding wine");
            tot3.Should().Be(2 * 1.0m + 20m + 2 * 5m);
        }
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