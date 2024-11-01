using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.DTOs;
using Server.Models;
using BCrypt.Net;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase {

    SQLiteContext database;

    public AuthController(SQLiteContext database) {
      this.database = database;
    }

    [HttpGet]
    public IActionResult Authenticate() {

      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"Secret\" cookie");
      } 

      var accountData = database.Users
        .Where(x => x.Secret == Request.Cookies["secret"])
        .Select(x => x);

      if (!accountData.Any()) {
        return NotFound("User not found");
      }

      return Ok(new SuccesfulAuthenticationDTO(accountData.First().Id, accountData.First().Username));      
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginUserDTO loginUserDto) {

      if (loginUserDto.Username == null || loginUserDto.Username.Trim() == "") {
        return BadRequest("Username field is empty.");
      }

      if (loginUserDto.Password == null || loginUserDto.Password.Trim() == "") {
        return BadRequest("Password field is empty");
      }

      var accountData = database.Users
        .Where(x => x.Username == loginUserDto.Username.Trim())
        .Select(x => x);

      if (!accountData.Any()) {
        return Unauthorized("Incorrect username or password.");
      }

      if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUserDto.Password.Trim(), accountData.First().PasswordHash)) {
        return Unauthorized("Incorrect username or password.");
      }

      Response.Cookies.Append("secret", accountData.First().Secret, new CookieOptions() {
        HttpOnly = true,
        Secure = true,
        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
      });

      return Ok(new SuccesfulLoginDTO(accountData.First().Id, accountData.First().Username));
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

      if (database.Users.Where(x => x.Username == registerUserDto.Username).Any()) {
        return Conflict("Username already taken.");
      }

      UserModel user = new UserModel();
      user.Username = registerUserDto.Username;
      user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerUserDto.Password.Trim());
      
      do {
        user.Secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
      } while (database.Users.Where(x => x.Secret == user.Secret).Any());

      try {
        await database.Users.AddAsync(user);
        await database.SaveChangesAsync();
      } catch {
        return StatusCode(500);
      }

      Response.Cookies.Append("secret", user.Secret, new CookieOptions() {
        HttpOnly = true,
        Secure = true,
        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
      });

      return StatusCode(201, new SuccesfulRegisterDTO(user.Id, user.Username));
    }
  }
}