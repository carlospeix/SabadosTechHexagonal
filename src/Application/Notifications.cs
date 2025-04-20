using Model;
using Model.Ports.Driven;
using Model.Ports.Driving;
using Persistence;

namespace Application;

public class Notifications(UnitOfWork unitOfWork, IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider) : INotifications
{
    private readonly UnitOfWork unitOfWork = unitOfWork;
    private readonly IRegistrar registrar = registrar;
    private readonly ITimeProvider timeProvider = timeProvider;
    private readonly Secretary secretary = new(registrar, notificator, timeProvider);

    public async Task SendGeneral(string message, DateTime scheduleAt = default)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var builder = new GeneralNotificationBuilder(registrar, message, BringToNowPastScheduleDates(scheduleAt));
        await secretary.SendNotification(builder.Build());

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendToGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var grade = await registrar.GradeById(gradeId) ??
            throw new ArgumentException("Invalid grade identifier", nameof(gradeId));

        var builder = new GradeNotificationBuilder(grade, message);
        await secretary.SendNotification(builder.Build());

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

        var builder = new StudentNotificationBuilder(student, message);
        await secretary.SendNotification(builder.Build());

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

        var builder = new DisciplinaryNotificationBuilder(registrar, student, teacher, message);
        await secretary.SendNotification(builder.Build());

        await unitOfWork.SaveChangesAsync();
    }

    public async Task SendPendingNotifications(CancellationToken cancellationToken)
    {
        await secretary.SendPendingNotifications(cancellationToken);

        await unitOfWork.SaveChangesAsync();
    }

    private DateTime BringToNowPastScheduleDates(DateTime scheduleAt)
    {
        return scheduleAt == default ? timeProvider.UtcNow : scheduleAt;
    }
}
