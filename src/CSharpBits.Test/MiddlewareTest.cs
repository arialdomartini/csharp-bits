using System;
using Xunit;

namespace CSharpBits.Test;

delegate R Middleware<out T, R>(Func<T, R> f);

record ConnectionString(String Value);

public class MiddlewareTest
{
    [Fact]
    void works()
    {
        Middleware<ConnectionString, int> ConnOK = f =>
        {
            var connectionString = new ConnectionString(Value: "Hey");

            int r = f(connectionString);
            return r;
        };

        Middleware<ConnectionString, R> ConnGeneric<R>() => f =>
        {
            var connectionString = new ConnectionString(Value: "Hey");

            R r = f(connectionString);
            return r;
        };
        
        Middleware<ConnectionString, int> connOk = ConnOK;
        Middleware<ConnectionString, int> connOkGeneric = ConnGeneric<int>();

    }
}
