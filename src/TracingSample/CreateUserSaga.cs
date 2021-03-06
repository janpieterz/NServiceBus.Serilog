﻿using System;
using System.Threading.Tasks;
using NServiceBus;

public class CreateUserSaga :
    Saga<MySagaData>,
    IAmStartedByMessages<CreateUser>,
    IHandleTimeouts<SagaTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
        mapper.ConfigureMapping<CreateUser>(m => m.UserName)
            .ToSaga(s => s.UserName);
    }

    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        Data.UserName = message.UserName;
        context.LogInformation("User created. Message: {@Message}", message);
        UserCreated userCreated = new()
        {
            UserName = message.UserName
        };
        MarkAsComplete();
        return Task.WhenAll(
            RequestTimeout<SagaTimeout>(context, TimeSpan.FromSeconds(10)),
            context.SendLocal(userCreated));
    }

    public Task Timeout(SagaTimeout state, IMessageHandlerContext context)
    {
        context.LogInformation("Timeout received");
        return Task.CompletedTask;
    }
}