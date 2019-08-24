using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rgnl_server.Data;
using rgnl_server.Helpers;
using rgnl_server.Models.Entities;

namespace rgnl_server.Controllers.OData
{
    [Authorize(Policy = Constants.Strings.Roles.Producer)]
    [Route("[controller]")]
    public class PostController : ODataController
    {
        private readonly ApplicationDbContext _dbContext;

        public PostController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Post> Get()
        {
            return _dbContext.NoTrackingSet<Post>();
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<Post> Get([FromODataUri] int key)
        {
            return SingleResult.Create(
                _dbContext.NoTrackingSet<Post>()
                .Where(p => p.PostId == key));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            post.CreatedDate = DateTime.UtcNow;
            post.AppUserId = int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value);
            _dbContext.Add(post);

            await _dbContext.SaveChangesAsync();
            return Created(post);
        }

        [HttpPatch]
        [EnableQuery]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<Post> post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbContext.TrackingSet<Post>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }
            if (entity.AppUserId != int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value))
            {
                return Unauthorized();
            }

            post.Patch(entity);
            entity.UpdatedDate = DateTime.UtcNow;

            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();

            return Updated(entity);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (key != int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value))
            {
                return Unauthorized();
            }

            var user = await _dbContext.TrackingSet<Post>().FindAsync(key);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}