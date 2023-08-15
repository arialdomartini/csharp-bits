using System;
using System.ComponentModel.DataAnnotations;
using static CSharpBits.Test.PropertyBasedTesting.Country;

namespace CSharpBits.Test.PropertyBasedTesting;

using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

enum Category
{
    Vegetables,
    Drinks,
    Books,
    BoardGames,
    Meat,
    Pastries,
    Electronics
}

record Promotion(string Name, DateTime ValidFrom, DateTime ValidTo);

record Product(string Name, decimal price, Category category);

enum Country
{
    Italy,
    France,
    Belgium,
    Mexico,
    UK,
    USA
}

public class ProductPropertyTests
{
    private readonly Gen<DateTime> AnyDate = Arb.Generate<DateTime>();
    private readonly Gen<int> PositiveNumber = Arb.Generate<int>().Select(Math.Abs);

    Gen<Promotion> ValidPromotion(DateTime today) =>
        from name in Arb.Generate<string>()
        from daysBefore in PositiveNumber
        from daysAfter in PositiveNumber
        select new Promotion(
            Name: name,
            ValidFrom: today.AddDays(-daysBefore),
            ValidTo: today.AddDays(daysAfter));

    private static readonly Gen<Country> LocalCountry = Arb.Generate<Country>();
    
    Gen<Country> InternationalCountries =
        from country in Arb.Generate<Country>()
        from local in LocalCountry
        where country != local
        select country;

    Gen<Product> FoodProducts =
        from name in Arb.Generate<string>()
        from price in Arb.Generate<decimal>()
        from category in Arb.Generate<Category>().Where(isFood)
        select new Product(name, price, category);

    
    bool Ship(Product product, Country country, Promotion promotions)
    {
        throw new NotImplementedException();
    }

    record UseCase(DateTime Today, Country Country, Product Product, Promotion Promotion);

    [Property]
    Property food_products_are_restricted_from_international_shipping_due_to_regulatory_compliance_unless_there_is_an_active_promotion()
    {
        Arbitrary<UseCase> useCases = Arb.From(
            from today in AnyDate
            from country in InternationalCountries
            from product in FoodProducts
            from promotion in ValidPromotion(today)
            select new UseCase(Today: today, Country: country, Product: product, Promotion: promotion));

        bool internationalShippingIsAllowed(UseCase useCase) =>
            Ship(useCase.Product, useCase.Country, useCase.Promotion) == true;

        return Prop.ForAll(useCases, internationalShippingIsAllowed);
    }

    private static bool isFood(Category category)
    {
        return true;
    }
}
