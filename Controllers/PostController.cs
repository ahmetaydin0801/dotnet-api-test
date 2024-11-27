using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration dapper)
        {
            _dapper = new DataContextDapper(dapper);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM TutorialAppSchema.Posts";
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("Post/{postId}")]
        public Post GetPost(int postId)
        {
            string sql = @"
                SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated 
                FROM TutorialAppSchema.Posts 
                WHERE PostId = @PostId";

            return _dapper.LoadDataSingle<Post>(sql, new { PostId = postId });
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            // Retrieve the UserId from the authenticated user's claims
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            int userId = int.Parse(userIdClaim.Value);

            string sql = @"
                SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated 
                FROM TutorialAppSchema.Posts 
                WHERE UserId = @UserId";

            return _dapper.LoadData<Post>(sql, new { UserId = userId });
        }
        
        [HttpPost("CreatePost")]
        public IActionResult CreatePost(CreatePostDto postDto)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            int userId = int.Parse(userIdClaim.Value);

            string sql = @"
                INSERT INTO TutorialAppSchema.Posts (UserId, PostTitle, PostContent, PostCreated, PostUpdated) 
                VALUES (@UserId, @PostTitle, @PostContent, @PostCreated, @PostUpdated)";

            bool success = _dapper.Execute(sql, new
            {
                UserId = userId,
                PostTitle = postDto.PostTitle,
                PostContent = postDto.PostContent,
                PostCreated = DateTime.UtcNow,
                PostUpdated = DateTime.UtcNow
            });

            if (success)
            {
                return Ok("Post created successfully.");
            }

            return BadRequest("Failed to create post.");
        }

        [HttpPut("EditPost")]
        public IActionResult EditPost(EditPostDto postDto)
        {
            string sql = @"
        UPDATE TutorialAppSchema.Posts 
        SET PostTitle = @PostTitle, 
            PostContent = @PostContent, 
            PostUpdated = @PostUpdated 
        WHERE PostId = @PostId";

            bool success = _dapper.Execute(sql, new
            {
                PostId = postDto.PostId,
                PostTitle = postDto.PostTitle,
                PostContent = postDto.PostContent,
                PostUpdated = DateTime.UtcNow
            });

            if (success)
            {
                return Ok("Post updated successfully.");
            }

            return BadRequest("Failed to update post.");
        }
        
        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            // Retrieve the UserId from the authenticated user's claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid user ID claim.");
            }


            string ownerSql = "SELECT UserId FROM TutorialAppSchema.Posts WHERE PostId = @PostId";
            int? postOwnerId = _dapper.LoadDataSingle<int?>(ownerSql, new { PostId = postId });

            if (postOwnerId == null)
            {
                return NotFound("Post not found.");
            }

            if (postOwnerId != userId)
            {
                return Forbid("You are not authorized to delete this post.");
            }


            string sql = "DELETE FROM TutorialAppSchema.Posts WHERE PostId = @PostId";
            bool success = _dapper.Execute(sql, new { PostId = postId });

            if (success)
            {
                return Ok("Post deleted successfully.");
            }

            return BadRequest("Failed to delete post.");
        }

    }
}