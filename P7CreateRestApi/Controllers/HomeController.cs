using Microsoft.AspNetCore.Mvc;

namespace Dot.Net.WebApi.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    //public class HomeController : ControllerBase
    //{
    //    [HttpGet]
    //    public IActionResult Get()
    //    {
    //        return Ok();
    //    }

    //    [HttpGet]
    //    [Route("Admin")]
    //    public IActionResult Admin()
    //    {
    //        return Ok();
    //    }
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check() => Ok("API is running");
    }
}