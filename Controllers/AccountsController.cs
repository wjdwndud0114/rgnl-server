using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rgnl_server.Data;
using rgnl_server.Helpers;
using rgnl_server.Models.Entities;
using rgnl_server.Models.Resources;

namespace rgnl_server.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<AppUser> userManager,
            IMapper mapper,
            ApplicationDbContext appDbContext
        ) {
            _userManager = userManager;
            _mapper = mapper;
        }

        // POST api/accounts
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] RegistrationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(resource);

            var result = await _userManager.CreateAsync(userIdentity, resource.Password);

            if (!result.Succeeded) return BadRequest(Errors.AddErrorsToModelState(result, ModelState));

            await _userManager.AddToRoleAsync(userIdentity, Constants.Strings.Roles.Consumer);

            // await _appDbContext.AddAsync<Profile>(new Profile {  });
            // await _appDbContext.SaveChangesAsync();

            return Ok("Account created");
        }

        [HttpPost("gov")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProducer([FromBody] RegistrationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(resource);

            var result = await _userManager.CreateAsync(userIdentity, resource.Password);

            if (!result.Succeeded) return BadRequest(Errors.AddErrorsToModelState(result, ModelState));

            await _userManager.AddToRoleAsync(userIdentity, Constants.Strings.Roles.Producer);

            // await _appDbContext.AddAsync<Profile>(new Profile {  });
            // await _appDbContext.SaveChangesAsync();

            return Ok("Account created");
        }
    }
}