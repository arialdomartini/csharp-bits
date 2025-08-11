using System;
using Xunit;

namespace CSharpBits.Test;

internal class Subscriber
{
    private Logger logger;

    public Subscriber(Logger logger)
    {
        this.logger = logger;
    }

    ~Subscriber()
    {
        logger.Log = "Subscriber is being finalized.";
    }

    public void OnServiceChanged(object sender, EventArgs e)
    {

    }
}

internal class Service
{
    private Logger logger;

    public event EventHandler? SomethingChanged;

    public Service(Logger logger)
    {
        this.logger = logger;
    }

    ~Service()
    {
        logger.Log = "Service is being finalized.";
    }

    public void RaiseChange()
    {
        SomethingChanged?.Invoke(this, EventArgs.Empty);
    }
}

internal class Logger
{
    public string Log = string.Empty;
}

public class DelegatesAndGcTest
{
    [Fact]
    public void LogsBeforeBeingGcCollected()
    {
        var loggerSubscriber = new Logger();
        var loggerService = new Logger();

        void CreateAndRelease()
        {
            var mySubscriber = new Subscriber(loggerSubscriber);
            var myService = new Service(loggerService);
            myService.SomethingChanged += mySubscriber.OnServiceChanged;
        }

        CreateAndRelease();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(string.IsNullOrEmpty(loggerSubscriber.Log));
        Assert.False(string.IsNullOrEmpty(loggerService.Log));
    }
}
