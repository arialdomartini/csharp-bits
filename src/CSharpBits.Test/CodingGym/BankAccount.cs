using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using static CSharpBits.Test.CodingGym.WithdrawalResult;

namespace CSharpBits.Test.CodingGym;

public class BankAccountTest
{
    private readonly Arbitrary<int> PositiveNumber =
        Arb.Generate<int>()
            .Select(Math.Abs)
            .Where(n => n > 0)
            .ToArbitrary();

    private BankAccount _bankAccount;

    [Property]
    void withdrawal_not_possible_with_empty_account(PositiveInt forAnyAmount)
    {
        WithdrawalResult withdrawalResult = BankAccount.Withdraw(Amount.Of(forAnyAmount.Item));

        Assert.Equal(NoBalance, withdrawalResult);
    }

    [Fact]
    void withdrawing_0_returns_0()
    {
        var withdrawalResult = BankAccount.Withdraw(Amount.Of(0));

        Assert.Equal(SomeBalance(new Amount(0)), withdrawalResult);
    }

    // [Property]
    // void withdrawn_amount_is_possible_as_long_as_deposit_is_equal_or_larger(PositiveInt a, PositiveInt b)
    // {
    //     var deposited = new Amount(a.Item + b.Item);
    //     var withdrawn = new Amount(b.Item);
    //     
    //     WithdrawalResult withdrawalResult = BankAccount.Withdraw(new Amount(forAnyAmount.Item));
    //
    //     Assert.Equal(NoBalance, withdrawalResult);
    // }

    [Property]
    void withdrawn_amount_is_possible_as_long_as_deposit_is_equal_or_larger(PositiveInt valueToDeposit)
    {
        _bankAccount = new BankAccount();
        var toDeposit = new Amount(valueToDeposit.Item);

        var currentBalance = _bankAccount.Deposit(toDeposit);

        Assert.Equal(SomeBalance(toDeposit), currentBalance);
    }

    [Property]
    void withdraws_amounts_is_possible_as_long_as_deposit_is_equal_or_larger(
        PositiveInt a,
        PositiveInt b)
    {

        _bankAccount = new BankAccount();
        _bankAccount.Deposit(new Amount(a.Item));

        var currentBalance = _bankAccount.Deposit(new Amount(b.Item));

        Assert.Equal(SomeBalance(new Amount(a.Item + b.Item)), currentBalance);
    }
}

internal record Amount(int Value)
{
    internal bool IsEqualTo(Amount other) => other.Value == Value;

    internal bool IsGreaterThan(Amount other) => Value > other.Value;

    internal static Amount Of(int value) => new(value);

    public Amount Add(Amount toAdd) => Amount.Of(Value + toAdd.Value);
};

internal class BankAccount
{
    private Amount _balance = Amount.Of(0);

    internal static WithdrawalResult Withdraw(Amount amount) =>
        amount switch
        {
            var a when a.IsGreaterThan(new Amount(0)) => NoBalance,
            var a when a.IsEqualTo(new Amount(0)) => SomeBalance(new Amount(0)),
        };

    public WithdrawalResult Deposit(Amount toDeposit)
    {
        _balance = _balance.Add(toDeposit);
        return SomeBalance(_balance);
    }
}

internal record SomeBalanceResult(Amount Amount) : WithdrawalResult;

internal abstract record WithdrawalResult
{
    internal static WithdrawalResult NoBalance => new NoBalanceResult();
    internal static WithdrawalResult SomeBalance(Amount amount) => new SomeBalanceResult(amount);
}

record NoBalanceResult : WithdrawalResult;
