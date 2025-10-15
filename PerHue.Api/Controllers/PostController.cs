using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;

namespace PerHue.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public PostController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        // GET: api/<PostController>
        [HttpGet]
        public async Task<PaginatedResult<PostModel>> Get(int? pageIndex = 1, int? pageSize = 30, string? searchTerm = "")
        {
            return await _servicesProvider.PostService.GetAllAsync(pageIndex ?? 1, pageSize ?? 30, searchTerm);
        }

/*        // GET api/<PostController>/all
        [HttpGet("all")]
        public async Task<IEnumerable<PostModel>> GetAll()
        {
            return await _servicesProvider.PostService.GetAllAsync();
        }*/

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostModel>> Get(int id)
        {
            var post = await _servicesProvider.PostService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servicesProvider.PostService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
		[HttpPost]
		public async Task<ActionResult<PostModel>> Post([FromBody] PostModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var created = await _servicesProvider.PostService.CreateAsync(model);

			if (created == null)
			{
				return BadRequest("Could not create post.");
			}

			// Return 201 Created with location header
			return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
		}
	}
}
