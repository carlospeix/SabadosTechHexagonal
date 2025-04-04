﻿using Model.Ports.Driven;
using Model.Ports.Driving;

namespace Model.UseCases;

public class Notifications : INotifications
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Notifications(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendGlobal(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null or empty");
        }
        var secretary = new Secretary(registrar, notificator);
        secretary.SendNotification(message);
    }
}
