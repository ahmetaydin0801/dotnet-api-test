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

    [HttpGet("GetUserSalary/{userId}")]
    public IActionResult GetUserSalary(int userId)
    {
        string sql = @"
    SELECT [UserId], 
           [Salary]
    FROM TutorialAppSchema.UserSalary
    WHERE UserId = @UserId
    ";

        try
        {
            UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql, new { UserId = userId });
            return Ok(userSalary);
        }
        catch (InvalidOperationException)
        {
            // This exception occurs if no rows are returned
            return NotFound(new { Message = "User salary not found" });
        }
        catch (Exception ex)
        {
            // General exception handling
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalaryToAddDto userSalary)
    {
        string sql = @"
    INSERT INTO TutorialAppSchema.UserSalary (
        [UserId],
        [Salary]
    )
    VALUES (
        @UserId,
        @Salary
    )";

        try
        {
            if (_dapper.Execute(sql, new
                {
                    UserId = userSalary.UserId,
                    Salary = userSalary.Salary
                }))
            {
                return Ok("Salary added successfully");
            }

            return BadRequest("Failed to add salary");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = @UserId";

        try
        {
            if (_dapper.Execute(sql, new { UserId = userId }))
            {
                return Ok("User salary deleted successfully");
            }

            return NotFound(new { Message = "No salary record found for the user" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalaryToUpdateDto userSalary)
    {
        string sql = @"
    UPDATE TutorialAppSchema.UserSalaries
    SET Salary = @Salary
    WHERE UserId = @UserId";

        try
        {
            // Validate if the user exists
            string checkUserSql = "SELECT COUNT(1) FROM TutorialAppSchema.Users WHERE UserId = @UserId";
            int userExists = _dapper.LoadDataSingle<int>(checkUserSql, new { UserId = userSalary.UserId });

            if (userExists == 0)
            {
                return BadRequest("Invalid user ID. User does not exist.");
            }

            // Perform the update
            if (_dapper.Execute(sql, new { UserId = userSalary.UserId, Salary = userSalary.Salary }))
            {
                return Ok("User salary updated successfully");
            }

            return NotFound(new { Message = "No salary record found for the user" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpGet("GetUserJobInfo/{userId}")]
    public IActionResult GetUserJobInfo(int userId)
    {
        string sql = @"
    SELECT [UserId], 
           [JobTitle], 
           [Department]
    FROM TutorialAppSchema.UserJobInfo
    WHERE UserId = @UserId";

        try
        {
            UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql, new { UserId = userId });
            return Ok(userJobInfo);
        }
        catch (InvalidOperationException)
        {
            // This exception occurs if no rows are returned
            return NotFound(new { Message = "User job information not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
    INSERT INTO TutorialAppSchema.UserJobInfo (
        [UserId],
        [JobTitle],
        [Department]
    )
    VALUES (
        @UserId,
        @JobTitle,
        @Department
    )";

        try
        {
            if (_dapper.Execute(sql, new
                {
                    UserId = userJobInfo.UserId,
                    JobTitle = userJobInfo.JobTitle,
                    Department = userJobInfo.Department
                }))
            {
                return Ok("Job information added successfully");
            }

            return BadRequest("Failed to add job information");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
    UPDATE TutorialAppSchema.UserJobInfo
    SET JobTitle = @JobTitle,
        Department = @Department
    WHERE UserId = @UserId";

        try
        {
            // Validate if the user exists
            string checkUserSql = "SELECT COUNT(1) FROM TutorialAppSchema.Users WHERE UserId = @UserId";
            int userExists = _dapper.LoadDataSingle<int>(checkUserSql, new { UserId = userJobInfo.UserId });

            if (userExists == 0)
            {
                return BadRequest("Invalid user ID. User does not exist.");
            }

            // Perform the update
            if (_dapper.Execute(sql, new
                {
                    UserId = userJobInfo.UserId,
                    JobTitle = userJobInfo.JobTitle,
                    Department = userJobInfo.Department
                }))
            {
                return Ok("User job information updated successfully");
            }

            return NotFound(new { Message = "No job information found for the user" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = @UserId";

        try
        {
            if (_dapper.Execute(sql, new { UserId = userId }))
            {
                return Ok("User job information deleted successfully");
            }

            return NotFound(new { Message = "No job information record found for the user" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }
}