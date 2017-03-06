﻿using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class CreateUserHandler : IHandleMessages<CreateUser>
{
    static ILog log = LogManager.GetLogger(typeof(CreateUserHandler));

    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        log.InfoFormat("Hello from {@Handler}. Message: {@Message}", nameof(CreateUserHandler), message);
        return Task.FromResult(0);
    }
}