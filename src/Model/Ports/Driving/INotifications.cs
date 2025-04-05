namespace Model.Ports.Driving;

public interface INotifications
{
    void SendGeneralNotification(string message);
    void SendToGrade(int gradeId, string message);
}
