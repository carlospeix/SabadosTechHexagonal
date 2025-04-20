using Model;
using Model.Ports.Driven;
using Model.Ports.Driving;
using Persistence;

namespace Application;

public class Notifications(UnitOfWork unitOfWork, IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider) : INotifications
{
    private readonly UnitOfWork unitOfWork = unitOfWork;
    private readonly IRegistrar registrar = registrar;

    private readonly Secretary secretary = new(registrar, notificator, timeProvider);

    public async Task SendGeneral(string message, DateTime scheduleAt = default)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        await secretary.SendGeneralNotification(message, scheduleAt);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var grade = await registrar.GradeById(gradeId) ??
            throw new ArgumentException("Invalid grade identifier", nameof(gradeId));

        await secretary.SendGradeNotification(grade, message);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendStudent(int studentId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var student = await registrar.StudentById(studentId) ??
            throw new ArgumentException("Invalid student identifier", nameof(studentId));

        await secretary.SendStudentNotification(student, message);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendDisciplinary(int studentId, int teacherId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var student = await registrar.StudentById(studentId) ??
            throw new ArgumentException("Invalid student identifier", nameof(studentId));

        var teacher = await registrar.TeacherById(teacherId) ??
            throw new ArgumentException("Invalid teacher identifier", nameof(teacherId));

        await secretary.SendDisciplinaryNotification(student, teacher, message);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendPendingNotifications(CancellationToken cancellationToken)
    {
        await secretary.SendPendingNotifications(cancellationToken);

        await unitOfWork.SaveChangesAsync();
    }
}
