﻿using Model.Ports.Driven;

namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Secretary(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public bool SendNotification(string message)
    {
        notificator.Send(Recipients(), message);
        return true;
    }

    private IEnumerable<Recipient> Recipients()
    {
        foreach (var grade in registrar.Grades)
        {
            foreach (var subject in grade.Subjects)
            {
                yield return new Recipient(subject.Teacher.Name, subject.Teacher.Email, subject.Teacher.Phone);
            }
        }
        foreach (var parent in registrar.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
