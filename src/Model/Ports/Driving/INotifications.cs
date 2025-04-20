namespace Model.Ports.Driving;

public interface INotifications
{
    public Task SendGeneral(string message, DateTime scheduleAt);
    public Task SendToGrade(int gradeId, string message);
    public Task SendStudent(int studentId, string message);
    public Task SendDisciplinary(int studentId, int teacherId, string message);
    public Task SendPendingNotifications(CancellationToken cancellationToken);
}
