namespace Model.Ports.Driving;

public interface INotifications
{
    public Task SendGeneral(string message, DateTime scheduleAt);
    public Task SendGrade(int gradeId, string message);
    public Task SendStudent(int studentRecordId, string message);
    public Task SendDisciplinary(int studentRecordId, int teacherId, string message);
    public Task SendPendingNotifications(CancellationToken cancellationToken);
}
