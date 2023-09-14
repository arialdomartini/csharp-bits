using System;
using System.Collections.Generic;
using Hedgehog.Xunit;
using Hedgehog;
using Hedgehog.Linq;
using Xunit;
using static CSharpBits.Test.PropertyBasedTesting.WithHedgehog.Part1.Category;
using static Hedgehog.Linq.Property;
using Gen = Hedgehog.Linq.Gen;
using Property = Hedgehog.Property;
using Range = Hedgehog.Linq.Range;

namespace CSharpBits.Test.PropertyBasedTesting.WithHedgehog;

public class Part1
{
    private interface IRepository
    {
        void Save(Product product);
        Product LoadById(Guid productId);
    }

    internal enum Category
    {
        Books
    }

    internal record Product(Guid Id, string Name, Category Category, decimal Price);

    private IRepository _repository;


    [Fact]
    void products_can_be_persisted()
    {
        var product = new Product(
            Id: Guid.NewGuid(),
            Name: "The Little Schemer",
            Category: Books,
            Price: 16.50M);

        _repository.Save(product);

        var found = _repository.LoadById(product.Id);

        Assert.Equal(found, product);
    }

    

    private string fizzbuzz(int n)
    {
        if (n % 15 == 0) return "fizzbuzz";
        return "yeeee";
    }
}
