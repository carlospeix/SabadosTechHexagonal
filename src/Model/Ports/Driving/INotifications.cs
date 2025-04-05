namespace Model.Ports.Driving;

public interface INotifications
{
    void SendGeneral(string message);
    void SendToGrade(int gradeId, string message);
}
