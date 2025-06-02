using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace sigma_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        [Authorize]
        [HttpPost]
        public string GetConversations()
        {
            return "wait";
        }
    }
}
