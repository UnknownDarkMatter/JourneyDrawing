using Microsoft.AspNetCore.Mvc;


namespace JourneyDrawing.Front.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CalculationController : ControllerBase
{
    [HttpGet]
    [Route("/api/v1/[controller]/HelloWorld")]
    public async Task<ActionResult> HelloWorld(CancellationToken cancellationToken)
    {
        return Ok(new {Message="Hello World from API !"});
    }




}
