using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
        
    }


    [HttpGet("test")]
    public string[] Test()
    {
        string[] responseArray = new string[] { "value1", "value2" };
        return responseArray;
    }
}

