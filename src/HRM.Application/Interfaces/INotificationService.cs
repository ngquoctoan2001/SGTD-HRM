using System.Threading.Tasks;

namespace HRM.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string title, string message, string type, string link = "");
}
