using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rgnl_server.Models.Entities;

namespace rgnl_server.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        
    }
}