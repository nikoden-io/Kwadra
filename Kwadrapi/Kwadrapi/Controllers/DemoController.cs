using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Kwadrapi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/demo")]
public class DemoController : ControllerBase
{

    public DemoController()
    {
      
    }

    [HttpGet("get-demo")]
    public IActionResult GetDemo()
    {
        
        return Ok("DEMO IS WORKING");
    }
}