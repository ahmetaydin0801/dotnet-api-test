using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    private DataContextEF _entityFramework;

    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();
        if (user != null)
        {
            return user;
        }

        throw new Exception("Failed to get user");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault();
        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to update user");
        }

        throw new Exception("Failed to get user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = new User();


        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Gender = user.Gender;

        _entityFramework.Add(userDb);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to update user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();
        if (userDb != null)
        {
            _entityFramework.Users.Remove(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to update user");
        }

        throw new Exception("Failed to get user");
    }
    
    [HttpGet("GetUserSalary/{userId}")]
    
public IActionResult GetUserSalary(int userId)
{
    var userSalary = _entityFramework.UserSalary.FirstOrDefault(s => s.UserId == userId);
    if (userSalary != null)
    {
        return Ok(userSalary);
    }
    return NotFound(new { Message = "Salary record not found for the specified user." });
}

[HttpPost("AddUserSalary")]
public IActionResult AddUserSalary(UserSalaryToAddDto userSalary)
{
    var userExists = _entityFramework.Users.Any(u => u.UserId == userSalary.UserId);
    if (!userExists)
    {
        return BadRequest(new { Message = "User does not exist." });
    }

    var salary = new UserSalary
    {
        UserId = userSalary.UserId,
        Salary = userSalary.Salary
    };

    _entityFramework.UserSalary.Add(salary);

    if (_entityFramework.SaveChanges() > 0)
    {
        return Ok(new { Message = "Salary added successfully." });
    }

    return StatusCode(500, new { Message = "An error occurred while adding salary." });
}

[HttpPut("EditUserSalary")]
public IActionResult EditUserSalary(UserSalaryToUpdateDto userSalary)
{
    var salaryDb = _entityFramework.UserSalary.FirstOrDefault(s => s.UserId == userSalary.UserId);
    if (salaryDb != null)
    {
        salaryDb.Salary = userSalary.Salary;

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok(new { Message = "Salary updated successfully." });
        }
        return StatusCode(500, new { Message = "An error occurred while updating salary." });
    }
    return NotFound(new { Message = "No salary record found for the specified user." });
}

[HttpDelete("DeleteUserSalary/{userId}")]
public IActionResult DeleteUserSalary(int userId)
{
    var salaryDb = _entityFramework.UserSalary.FirstOrDefault(s => s.UserId == userId);
    if (salaryDb != null)
    {
        _entityFramework.UserSalary.Remove(salaryDb);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok(new { Message = "Salary deleted successfully." });
        }
        return StatusCode(500, new { Message = "An error occurred while deleting salary." });
    }
    return NotFound(new { Message = "No salary record found for the specified user." });
}

}