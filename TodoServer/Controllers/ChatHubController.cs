using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TodoServer.Hubs;

namespace TodoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatHubController : ControllerBase
    {

        private IHubContext<ChatHub> _hubContext;

        public ChatHubController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpGet(Name = "GetHubStatus")]
        public string Get()
        {
            return JsonConvert.SerializeObject(_hubContext.Clients.All);
        }
    }
}
