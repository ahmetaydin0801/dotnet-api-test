using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
             
         [HttpPost("login")]
         public IActionResult Login()
         {
             return Ok();
         }
    }
}