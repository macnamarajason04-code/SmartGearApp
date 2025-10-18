using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SmartGearApp.Hubs
{
    public class ProductHub : Hub
    {
        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }
    }
}
