using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.DTOs;
using Server.Models;
using System.Security.Cryptography;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase {

    private readonly SQLiteContext database;

    public AuthController(SQLiteContext database) {
      this.database = database;
    }

    [HttpGet]
    public IActionResult Authenticate() {

      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"secret\" cookie.");
      } 

      var user = database.Users
        .Where(x => x.Secret == Request.Cookies["secret"]);

      if (!user.Any()) {
        return NotFound("User not found.");
      }

      var userData = user.First();

      return Ok(new SuccesfulAuthenticationDTO(userData.Id, userData.Username));      
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginUserDTO loginUserDto) {

      if (loginUserDto.Username == null || loginUserDto.Username.Trim() == "") {
        return BadRequest("Username field is either null or empty.");
      }

      if (loginUserDto.Password == null || loginUserDto.Password.Trim() == "") {
        return BadRequest("Password field is either null or empty.");
      }

      var user = database.Users
        .Where(x => x.Username == loginUserDto.Username.Trim());

      if (!user.Any()) {
        return Unauthorized("Incorrect username or password.");
      }

      var userData = user.First();

      if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUserDto.Password.Trim(), userData.PasswordHash)) {
        return Unauthorized("Incorrect username or password.");
      }

      Response.Cookies.Append("secret", userData.Secret, new CookieOptions() {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
      });

      return Ok(new SuccesfulLoginDTO(userData.Id, userData.Username));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerUserDto) {
      
      string usernamePattern = "^[a-zA-Z0-9._-]{2,32}$";
      if (registerUserDto.Username == null || !Regex.IsMatch(registerUserDto.Username.Trim(), usernamePattern)) {
        return BadRequest($"Username field is either null, or does not match \"{usernamePattern}\" regular expression.");
      }

      if (registerUserDto.Password == null || registerUserDto.Password.Trim() == "") {
        return BadRequest("Password field is either null or empty.");
      }

      if (database.Users.Where(x => x.Username == registerUserDto.Username.Trim()).Any()) {
        return Conflict("Username already taken.");
      }

      UserModel user = new UserModel();
      user.Username = registerUserDto.Username.Trim();
      user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerUserDto.Password.Trim());
      
      do {
        user.Secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
      } while (database.Users.Where(x => x.Secret == user.Secret).Any());
      // Or make the field UNIQUE?

      try {
        await database.Users.AddAsync(user);
        await database.SaveChangesAsync();
      } catch {
        return StatusCode(500);
      }

      Response.Cookies.Append("secret", user.Secret, new CookieOptions {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
      });

      return StatusCode(201, new SuccesfulRegisterDTO(user.Id, user.Username));
    }
  
    [HttpGet("logout")]
    public IActionResult Logout() {
      Response.Cookies.Delete("secret");
      return Ok();
    }
  }
}

// GET /api/auth -> Authenticate
// POST /api/auth/login -> Login
// POST /api/auth/register -> Register