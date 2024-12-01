using Microsoft.AspNetCore.Mvc;

namespace AutoLayer.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(MyService myService) : ControllerBase
    {
        private readonly MyService _myService = myService;

        [HttpGet("add")]
        public IActionResult Add()
        {
            _myService.Add();
            return Ok();
        }

        [HttpGet("update")]
        public IActionResult Update()
        {
            _myService.Update();
            return Ok();
        }

        [HttpGet("remove")]
        public IActionResult Remove()
        {
            _myService.Remove();
            return Ok();
        }
    }
}
