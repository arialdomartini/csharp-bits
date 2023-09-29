using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test;

public class TokenizerTest
{
    [Fact]
    void should_extract_unities_and_quantities()
    {
        var inventory = "x;y;5:2;10:99;20:33";

        var result = inventory.ExtractUnityQuantity();

        result.Should().BeEquivalentTo(new List<UnityQuantity>
        {
            new UnityQuantity { Unity = 5, Quantity = 2 },
            new UnityQuantity { Unity = 10, Quantity = 99 },
            new UnityQuantity { Unity = 20, Quantity = 33 }
        });
    }

    [Fact]
    void should_distribute_cashes()
    {
        var unityQuantities = new List<UnityQuantity>
        {
            new UnityQuantity { Unity = 5, Quantity = 2 },
            new UnityQuantity { Unity = 10, Quantity = 99 },
            new UnityQuantity { Unity = 100, Quantity = 33 }
        };

        var cashTypes = new List<CashType>
        {
            new CashType{ Unity = 5, IsCoin = true},
            new CashType{ Unity = 10, IsCoin = true},
            new CashType{ Unity = 20, IsCoin = true},
            new CashType{ Unity = 50, IsCoin = false},
            new CashType{ Unity = 100, IsCoin = false}
        };

        var result = unityQuantities.Distribute(cashTypes);

        result.Should().BeEquivalentTo(new List<Cash>
        {
            new Cash{ Unity = 5, Quantity = 2, IsCoin = true},
            new Cash{ Unity = 10, Quantity = 99, IsCoin = true},
            new Cash{ Unity = 20, Quantity = 0, IsCoin = true},
            new Cash{ Unity = 50, Quantity = 0, IsCoin = false},
            new Cash{ Unity = 100, Quantity = 33, IsCoin = false}
        });
    }

    [Fact]
    void acceptance_test()
    {
        var inventory = "x;y;10:101;20:499;100:29";

        var cashTypes = new List<CashType>
        {
            new CashType{ Unity = 5, IsCoin = true},
            new CashType{ Unity = 10, IsCoin = true},
            new CashType{ Unity = 20, IsCoin = true},
            new CashType{ Unity = 50, IsCoin = false},
            new CashType{ Unity = 100, IsCoin = false}
        };

        var result = inventory.ExtractUnityQuantity().Distribute(cashTypes);

        result.Should().BeEquivalentTo(new List<Cash>
        {
            new Cash{ Unity = 5, Quantity = 0, IsCoin = true},
            new Cash{ Unity = 10, Quantity = 101, IsCoin = true},
            new Cash{ Unity = 20, Quantity = 499, IsCoin = true},
            new Cash{ Unity = 50, Quantity = 0, IsCoin = false},
            new Cash{ Unity = 100, Quantity = 29, IsCoin = false}
        });
    }
}

class CashType
{
    internal int Unity { get; set; }
    internal bool IsCoin { get; set; }

    internal Cash ToCash(IEnumerable<UnityQuantity> unityQuantities)
    {
        return new Cash
        {
            Unity = Unity,
            IsCoin = IsCoin,
            Quantity = unityQuantities.GetQuantityOrDefault(Unity)
        };
    }
}

class UnityQuantity
{
    public int Unity { get; set; }
    public int Quantity { get; set; }
}

class Cash : UnityQuantity
{
    public bool IsCoin { get; set; }
}

internal static class InventoryExtensions
{
    internal static IEnumerable<Cash> Distribute(this IEnumerable<UnityQuantity> unityQuantities, IEnumerable<CashType> cashTypes) =>
        cashTypes.Select(c => c.ToCash(unityQuantities));

    internal static int GetQuantityOrDefault(this IEnumerable<UnityQuantity> unityQuantities, int unity) =>
        unityQuantities.FirstOrDefault(u => u.Unity == unity)?.Quantity ?? 0;

    internal static IEnumerable<UnityQuantity> ExtractUnityQuantity(this string inventory) =>
        inventory.ExtractUnityQuantityStrings()
            .Select(SplitUnityQuantity);

    private static  IEnumerable<string> ExtractUnityQuantityStrings(this string inventory) =>
        inventory
            .Split(";")
            .Skip(2);

    private static UnityQuantity SplitUnityQuantity(string s)
    {
        var unityAndQuantity = s.Split(":");
        return new UnityQuantity
        {
            Unity = Convert.ToInt32(unityAndQuantity[0]),
            Quantity = Convert.ToInt32(unityAndQuantity[1])
        };
    }
}