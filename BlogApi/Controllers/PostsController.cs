using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .ToListAsync();

            return Ok(posts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.ImagePath,
                p.VideoPath,
                p.CreatedAt,
                p.UpdatedAt,
                User = new { p.User.Id, p.User.UserName, p.User.Email }
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.ImagePath,
                post.VideoPath,
                post.CreatedAt,
                post.UpdatedAt,
                User = new { post.User.Id, post.User.UserName, post.User.Email }
            });
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                ImagePath = model.ImagePath,
                VideoPath = model.VideoPath,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.ImagePath,
                post.VideoPath,
                post.CreatedAt,
                User = new { Id = userId }
            });
        }

    
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto model)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (post.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            post.Title = model.Title;
            post.Content = model.Content;
            post.ImagePath = model.ImagePath;
            post.VideoPath = model.VideoPath;
            post.UpdatedAt = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(post);
        }



        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            if (post.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post deleted successfully" });
        }

      
    }
}
