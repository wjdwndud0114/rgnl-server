using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rgnl_server.Helpers;
using rgnl_server.Models.Entities;

namespace rgnl_server.Controllers.OData
{
    [Authorize(Policy = Constants.Strings.Roles.Admin)]
    [Route("[controller]")]
    public class AppUserController : ODataController
    {
        private readonly UserManager<AppUser> _userManager;

        public AppUserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<AppUser> Get()
        {
            return _userManager.Users;
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<AppUser> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_userManager.Users.Where(u => u.Id == key));
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<AppUser> user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == key);
            if (entity == null)
            {
                return NotFound();
            }

            user.Patch(entity);
            await _userManager.UpdateAsync(entity);

            return Updated(entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == key);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
    }
}