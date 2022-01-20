using System;
using Xunit;

namespace CSharpBits.Test.ReaderMonad.Approach1.ConnectionStringExample
{
    public class ConnectionStringTest
    {
        [Fact]
        void monadic_connection_string()
        {
            Reader<E, T> Reader<E, T>(Func<E, T> f) =>
                f.ToReader();

            Func<string, Reader<String, int>> readAge = name =>
                Reader<String, int>(conn => 40);

            Func<int, Reader<String, string>> readDrink = age =>
                Reader<String, string>(age => "beer");

            Func<string, string> give = drink =>
                $"Hey! Here's your {drink}!";

            Func<string, Reader<String, string>> serve = name =>
                readAge(name)
                    .Bind(age => readDrink(age))
                    .Map(drink => give(drink));

            Func<string, Reader<String, string>> serveLinq = name =>
                from age in readAge(name)
                from drink in readDrink(age)
                select give(drink);

            serve("Mario")
                .Run("sqlite:some.db");
        }

        [Fact]
        void non_monadic_connection_string()
        {
            Func<string, String, int> readAge = (name, conn) =>
                40;

            Func<int, String, string> readDrink = (age, conn) =>
                "beer";

            Func<string, string> give = drink =>
                $"Hey! Here's your {drink}!";

            Func<string, String, string> serve = (name, conn) =>
            {
                var age = readAge(name, conn);
                var drink = readDrink(age, conn);
                var greeting = give(drink);
                return greeting;
            };

            Func<string, String, string> fluentServe = (name, conn) =>
                give(
                    readDrink(
                        readAge(name, conn), conn));

            serve("Mario", "sqlite:some.db");
        }

        class AgeRepository
        {
            private readonly String _conn;

            internal AgeRepository(String conn)
            {
                _conn = conn;
            }

            internal int ReadAge(string name) =>
                40;
        }

        class DrinkRepository
        {
            private readonly string _conn;

            internal DrinkRepository(String conn)
            {
                _conn = conn;
            }

            internal string ReadDrink(int age) =>
                "beer";
        }

        [Fact]
        void with_dependency_injection_connection_string()
        {
            String conn = "sqlite:some.db";
            var ageRepo = new AgeRepository(conn);
            var drinkRepo = new DrinkRepository(conn);

            Func<string, string> give = drink =>
                $"Hey! Here's your {drink}!";

            Func<string, string> serve = name =>
            {
                var age = ageRepo.ReadAge(name);
                var drink = drinkRepo.ReadDrink(age);
                var greeting = give(drink);
                return greeting;
            };

            serve("Mario");
        }

        [Fact]
        void made_fluent_with_dependency_injection_connection_string()
        {
            String conn = "sqlite:some.db";
            var ageRepo = new AgeRepository(conn);
            var drinkRepo = new DrinkRepository(conn);

            Func<string, int> getAge = ageRepo.ReadAge;
            Func<int, string> getDrink = drinkRepo.ReadDrink;

            Func<string, string> give = drink =>
                $"Hey! Here's your {drink}!";

            Func<string, string> serve = name =>
            {
                var process =
                    getAge.Then(age =>
                            getDrink(age))
                        .Then(drink =>
                            give(drink));

                return process(name);
            };

            Func<string, string> servePointFree = name =>
            {
                var process =
                    getAge
                        .Then(getDrink)
                        .Then(give);

                return process(name);
            };

            serve("Mario");
            servePointFree("Mario");
        }
    }
}