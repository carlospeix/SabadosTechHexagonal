namespace Model.Ports.Driving;

public interface INotifications
{
    void SendGeneral(string message, DateTime scheduledAt);
    void SendToGrade(int gradeId, string message);
    void SendStudent(int studentId, string message);
    void SendDisciplinary(int studentId, int teacherId, string message);
}
