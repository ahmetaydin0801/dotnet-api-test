using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;
        
        // ASK manually instantiated

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _authHelper = new AuthHelper(config);
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = @"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = @Email";
                IEnumerable<string> existingUsers =
                    _dapper.LoadData<string>(sqlCheckUserExists, new { Email = userForRegistration.Email });
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth =
                        @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES (@Email, @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    SqlParameter emailParam = new SqlParameter("@Email", SqlDbType.NVarChar);
                    emailParam.Value = userForRegistration.Email; // Assuming userForRegistration.Email is the input email
                    SqlParameter passwordSaltParam = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParam.Value = passwordSalt;
                    SqlParameter passwordHashParam = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParam.Value = passwordHash;

                    sqlParams.Add(emailParam);
                    sqlParams.Add(passwordSaltParam);
                    sqlParams.Add(passwordHashParam);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParams))
                    {

                                            string sqlAddUser = @"
                        INSERT INTO TutorialAppSchema.Users (
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender]
                        )
                        VALUES (
                            @FirstName,
                            @LastName,
                            @Email,
                            @Gender
                        );
                    ";

                        if (_dapper.Execute(sqlAddUser, new
                        {
                            FirstName = userForRegistration.FirstName,
                            LastName = userForRegistration.LastName,
                            Email = userForRegistration.Email,
                            Gender = userForRegistration.Gender
                        }))
                        {
                            return Ok("User added successfully");
                        }

                        throw new Exception("Failed to add user");



                    }

                    throw new Exception("Failed to register user");
                }

                throw new Exception("Email already exists");
            }

            throw new Exception("Passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        // ASK do you use DTO
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt =
                @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = @Email";
            UserForLoginConfirmationDto userForConfirmation =
                _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt,
                    new { Email = userForLogin.Email });

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Passwords do not match");
                }
            }

            string userIdSql = @"SELECT [UserId] FROM TutorialAppSchema.Users WHERE Email = @Email";

            int userId = _dapper.LoadDataSingle<int>(userIdSql, new { Email = userForLogin.Email });
            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("UserId")?.Value + "";
            
            string userIdSql = @"SELECT [UserId] FROM TutorialAppSchema.Users WHERE [UserId] = @UserId";
            
            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql, new { UserId = userId });
            

             
            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelper.CreateToken(userIdFromDB)}
            });
        }

    }

}