using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rgnl_server.Helpers;
using rgnl_server.Interfaces.Repositories;

namespace rgnl_server.Controllers
{
    [Authorize(Policy = Constants.Strings.Roles.Consumer)]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public DashboardController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetSelf()
        {
            var userId = int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value);

            return Ok(await _userRepository.GetUser(userId));
        }

        [HttpGet("govs")]
        public IActionResult GetGovernmentUsers()
        {
            return Ok(_userRepository.GetGovernmentUsers());
        }

        [HttpGet("followed")]
        public async Task<IActionResult> FollowedByUser()
        {
            var userId = int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value);

            return Ok(await _userRepository.FollowedByUser(userId));
        }

        [HttpGet("following")]
        public async Task<IActionResult> FollowingTheUser()
        {
            var userId = int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value);

            return Ok(await _userRepository.FollowingTheUser(userId));
        }
    }
}