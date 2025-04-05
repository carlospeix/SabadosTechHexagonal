namespace Model.Ports.Driving;

public interface INotifications
{
    void SendGlobal(string message);
    void SendToGrade(int gradeId, string message);
}
