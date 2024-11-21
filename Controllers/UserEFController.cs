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
    IUserRepository _userRepository;

    public UserEFController(IConfiguration config, IUserRepository userRepository )
    {
        _entityFramework = new DataContextEF(config);
        _userRepository = userRepository; 
    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);
        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;

            if (_userRepository.SaveChanges())
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


        _userRepository.AddEntity<User>(userDb);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to update user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _userRepository.GetSingleUser(userId);
        if (userDb != null)
        {

            _userRepository.RemoveEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to update user");
        }

        throw new Exception("Failed to get user");
    }

    
    [HttpGet("GetUserSalary/{userId}")]
    
public UserSalary GetUserSalary(int userId)
{
    var userSalary = _userRepository.GetUserSalary(userId);
    if (userSalary != null)
    {
        return userSalary;
    }
    throw new Exception("Failed to get usersalary");
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


    _userRepository.AddEntity<UserSalary>(salary);

    if (_userRepository.SaveChanges())
    {
        return Ok(new { Message = "Salary added successfully." });
    }

    return StatusCode(500, new { Message = "An error occurred while adding salary." });
}

[HttpPut("EditUserSalary")]
public IActionResult EditUserSalary(UserSalaryToUpdateDto userSalary)
{
    var salaryDb = _userRepository.GetUserSalary(userSalary.UserId);
    if (salaryDb != null)
    {
        salaryDb.Salary = userSalary.Salary;

        if (_userRepository.SaveChanges())
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
    var salaryDb = _userRepository.GetUserSalary(userId);
    if (salaryDb != null)
    {

        _userRepository.RemoveEntity<UserSalary>(salaryDb);
        

        if (_userRepository.SaveChanges())
        {
            return Ok(new { Message = "Salary deleted successfully." });
        }
        return StatusCode(500, new { Message = "An error occurred while deleting salary." });
    }
    return NotFound(new { Message = "No salary record found for the specified user." });
}

[HttpGet("GetUserJobInfo/{userId}")]
public IActionResult GetUserJobInfo(int userId)
{
    var jobInfo = _userRepository.GetUserJobInfo(userId);
    if (jobInfo != null)
    {
        return Ok(jobInfo);
    }
    return NotFound(new { Message = "Job information not found for the specified user." });
}

[HttpPost("AddUserJobInfo")]
public IActionResult AddUserJobInfo(UserJobInfoToAddDto userJobInfo)
{
    var userExists = _entityFramework.Users.Any(u => u.UserId == userJobInfo.UserId);
    if (!userExists)
    {
        return BadRequest(new { Message = "User does not exist." });
    }

    var newJobInfo = new UserJobInfo
    {
        UserId = userJobInfo.UserId,
        JobTitle = userJobInfo.JobTitle,
        Department = userJobInfo.Department
    };

    _userRepository.AddEntity<UserJobInfo>(newJobInfo);

    if (_userRepository.SaveChanges())
    {
        return Ok(new { Message = "Job information added successfully." });
    }
    return StatusCode(500, new { Message = "An error occurred while adding job information." });
}

[HttpPut("EditUserJobInfo")]
public IActionResult EditUserJobInfo(UserJobInfoToUpdateDto userJobInfo)
{
    var jobInfoDb = _userRepository.GetUserJobInfo(userJobInfo.UserId);
    if (jobInfoDb != null)
    {
        jobInfoDb.JobTitle = userJobInfo.JobTitle;
        jobInfoDb.Department = userJobInfo.Department;

        if (_userRepository.SaveChanges())
        {
            return Ok(new { Message = "Job information updated successfully." });
        }
        return StatusCode(500, new { Message = "An error occurred while updating job information." });
    }
    return NotFound(new { Message = "Job information not found for the specified user." });
}

[HttpDelete("DeleteUserJobInfo/{userId}")]
public IActionResult DeleteUserJobInfo(int userId)
{
    var jobInfoDb = _userRepository.GetUserJobInfo(userId);
    if (jobInfoDb != null)
    {
        _userRepository.RemoveEntity<UserJobInfo>(jobInfoDb);

        if (_userRepository.SaveChanges())
        {
            return Ok(new { Message = "Job information deleted successfully." });
        }
        return StatusCode(500, new { Message = "An error occurred while deleting job information." });
    }
    return NotFound(new { Message = "Job information not found for the specified user." });
}

}