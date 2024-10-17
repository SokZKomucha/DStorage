using Microsoft.AspNetCore.Mvc;

[Route("/api/[controller]")]
[ApiController]
public class TestController : ControllerBase {
  [HttpGet]
  public IActionResult Get() {
    return Ok("Hello, World!");
  }
}