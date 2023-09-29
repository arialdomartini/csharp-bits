using System.Linq;
using FluentAssertions;
using Pie.Monads;
using Xunit;

namespace CSharpBits.Test.FunctionalParser;

public class FunctionalParserTest
{
    private const string Message = "message";
    private static Either<string, StateResult> Pure(State state) =>
        StateResult.Success(state, new Result());

    [Fact]
    void single_state_fails()
    {
        var state2 = State
            .Empty("2");
        var state1 = State.From("1", (Message, state2));

        var result = state1.Eval("unknown", new Result());

        result.Left().Should().Be("error");
    }

    [Fact]
    void single_state_move_to_next_state()
    {
        var state2 = State.Empty("2");
        var state1 = State.From("1", (Message, state2));

        var result = state1.Eval(Message, new Result()).Right();

        result.State.Should().Be(state2);
        result.Result.Tracks.Should().BeEquivalentTo("1");
    }

    [Fact]
    void support_to_multiple_destinations()
    {
        var state2 = State.Empty("2");
        var state1 = State.From("1", ("one", state2), ("two", state2));

        {
            var result = state1.Eval("one", new Result()).Right();
            result.Result.Tracks.Should().BeEquivalentTo("1");
        }
        {
            var result = state1.Eval("two", new Result()).Right();
            result.Result.Tracks.Should().BeEquivalentTo("1");
        }
    }

    [Fact]
    void long_chain()
    {
        var state3 = State.Empty("3");
        var state2 = State.From("2", ("goto3", state3));
        var state1 = State.From("1", ("goto2", state2));

        var result =
            Pure(state1)
                .Bind(s => s.State.Eval("goto2", s.Result))
                .Bind(s => s.State.Eval("goto3", s.Result))
                .Right();

        result.Result.Tracks.Should().BeEquivalentTo("1", "2");
    }

    [Fact]
    void failing_long_chain()
    {
        var state3 = State.Empty("3");
        var state2 = State.From("2", ("message 2", state3));
        var state1 = State.From("1", ("message 1", state2));

        var result =
            Pure(state1)
                .Bind(s => s.State.Eval("message 1", s.Result))
                .Bind(s => s.State.Eval("message 3", s.Result))
                .Left();

        result.Should().Be("error");
    }

    [Fact]
    void with_foreach()
    {
        var state3 = State.Empty("3");
        var state2 = State.From("2", ("message 2", state3));
        var state1 = State.From("1", ("message 1", state2));

        var current = Pure(state1);


        var messages = new[] {"message 1", "message 2"};
        foreach (var message in messages)
        {
            current = current.Bind(s => s.State.Eval(message, s.Result));
        }

        current.Right().Result.Tracks.Should().BeEquivalentTo("1", "2");
    }

    [Fact]
    void with_aggregate()
    {
        var state3 = State.Empty("3");
        var state2 = State.From("2", ("goto3", state3));
        var state1 = State.From("1", ("goto2", state2));

        var messages = new[] {"goto2", "goto3"};

        var result = messages.Aggregate(
                Pure(state1),
                (state, message) =>
                    state.Bind(s => s.State.Eval(message, s.Result)))
            .Right();

        result.Result.Tracks.Should().BeEquivalentTo("1", "2");
        result.State.Name.Should().Be("3");
    }

    [Fact]
    void multiple_choices()
    {
        var state4 = State.Empty("4");
        var state1 = State.From("1",
            ("goto2",
                State.From("2", ("goto4", state4))),
            ("goto3",
                State.From("3", ("goto4", state4))));

        {
            var messages = new[] {"goto2", "goto4"};

            var result = messages.Aggregate(
                Pure(state1),
                (state, message) =>
                    state.Bind(s => s.State.Eval(message, s.Result)));

            result.Right().Result.Tracks.Should().BeEquivalentTo("1", "2");
            result.Right().State.Name.Should().Be("4");
        }

        {
            var messages = new[] {"goto3", "goto4"};

            var result = messages.Aggregate(
                Pure(state1),
                (state, message) =>
                    state.Bind(s => s.State.Eval(message, s.Result)));

            result.Right().Result.Tracks.Should().BeEquivalentTo("1", "3");
            result.Right().State.Name.Should().Be("4");
        }
    }
}