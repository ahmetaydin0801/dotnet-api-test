using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
        SELECT [UserId],
               [FirstName],
               [LastName],
               [Email],
               [Gender],
               [Active]
        FROM TutorialAppSchema.Users
        ";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
        SELECT [UserId],
               [FirstName],
               [LastName],
               [Email],
               [Gender],
               [Active]
        FROM TutorialAppSchema.Users
        WHERE UserId = @UserId
        ";
        User user = _dapper.LoadDataSingle<User>(sql, new { UserId = userId });
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
    UPDATE TutorialAppSchema.Users
        SET [FirstName] = @FirstName,
            [LastName] = @LastName,
            [Email] = @Email,
            [Gender] = @Gender,
            [Active] = @Active
        WHERE [UserId] = @UserId
    ";

        if (_dapper.Execute(sql, new
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Active = user.Active
            }))
        {
            return Ok();
        }

        throw new Exception("Failed to update user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
    INSERT INTO TutorialAppSchema.Users(
        [FirstName],
        [LastName],
        [Email],
        [Gender],
        [Active]
    )
    VALUES (
        @FirstName,
        @LastName,
        @Email,
        @Gender,
        @Active
    )
    ";

        if (_dapper.Execute(sql, new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Active = user.Active
            }))
        {
            return Ok("User added successfully");
        }

        throw new Exception("Failed to add user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = @UserId";

        if (_dapper.Execute(sql, new { UserId = userId }))
        {
            return Ok("User deleted successfully");
        }
        throw new Exception("Failed to delete user");
    }
}