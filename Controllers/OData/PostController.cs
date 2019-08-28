using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using rgnl_server.Data;
using rgnl_server.Helpers;
using rgnl_server.Hubs;
using rgnl_server.Models.Entities;

namespace rgnl_server.Controllers.OData
{
    [Authorize(Policy = Constants.Strings.Roles.Producer)]
    [Route("[controller]")]
    public class PostController : ODataController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<PostHub> _hubContext;

        public PostController(ApplicationDbContext dbContext, IHubContext<PostHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Post> Get()
        {
            return _dbContext.NonTrackingSet<Post>();
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<Post> Get([FromODataUri] int key)
        {
            return SingleResult.Create(
                _dbContext.NonTrackingSet<Post>()
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
            await _hubContext.Clients.Group(post.AppUserId.ToString()).SendAsync("NewPostCreated", post);
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
            await _hubContext.Clients.Group(entity.AppUserId.ToString()).SendAsync("PostEdited", entity);

            return Updated(entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var post = await _dbContext.TrackingSet<Post>().FindAsync(key);

            if (post == null)
            {
                return NotFound();
            }
            if (post.AppUserId != int.Parse(this.User.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id).Value))
            {
                return Unauthorized();
            }

            _dbContext.Remove(post);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Group(post.AppUserId.ToString()).SendAsync("PostDeleted", post);

            return NoContent();
        }
    }
}