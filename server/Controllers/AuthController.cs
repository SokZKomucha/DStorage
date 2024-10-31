using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase {

    SQLiteContext database;

    public AuthController(SQLiteContext database) {
      this.database = database;
    }
  }
}