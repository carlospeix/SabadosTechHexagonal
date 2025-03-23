
namespace Model.Ports;

/// <summary>
/// Port de ports & adapters
/// </summary>
public interface INotificator
{
    public void Send(IEnumerable<Recipient> recipients, string message);
}
