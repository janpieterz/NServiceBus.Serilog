﻿using System;
using System.Threading;
using System.Threading.Tasks;
using VerifyXunit;
using NServiceBus;
using Xunit;
using Xunit.Abstractions;

public class WithNoTracingTests :
    VerifyBase
{
    [Fact]
    public async Task Handler()
    {
        Exception? exception = null;
        var resetEvent = new ManualResetEvent(false);
        var configuration = ConfigBuilder.BuildDefaultConfig("WithNoTracingTests");
        configuration.DisableRetries();
        configuration.RegisterComponents(components => components.RegisterSingleton(resetEvent));

        var recoverability = configuration.Recoverability();
        recoverability.Failed(settings => settings
            .OnMessageSentToErrorQueue(message =>
            {
                exception = message.Exception;
                resetEvent.Set();
                return Task.CompletedTask;
            }));

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(new StartHandler());
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(2)))
        {
            throw new Exception("No Set received.");
        }

        await endpoint.Stop();
        await Verify(exception!.Message);
    }

    public WithNoTracingTests(ITestOutputHelper output) :
        base(output)
    {
    }
}