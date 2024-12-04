using Microsoft.AspNetCore.Mvc;

namespace AutoLayer.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(MyService myService) : ControllerBase
    {
        private readonly MyService _myService = myService;

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _myService.GetByIdAsync());
        }

        [HttpGet("first")]
        public async Task<IActionResult> First()
        {
            return Ok(await _myService.GetFirstAsync());
        }

        [HttpGet("getwhere")]
        public async Task<IActionResult> GetWhere()
        {
            return Ok(await _myService.GetWhereAsync());
        }

        [HttpGet("getordered")]
        public async Task<IActionResult> GetOrdered()
        {
            return Ok(await _myService.GetOrderedAsync());
        }

        [HttpGet("getpage")]
        public async Task<IActionResult> GetPage()
        {
            return Ok(await _myService.GetPagedAsync());
        }

        [HttpGet("exists")]
        public async Task<IActionResult> Exists()
        {
            return Ok(await _myService.ExistsAsync());
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            return Ok(await _myService.CountAsync());
        }

        [HttpGet("countwhere")]
        public async Task<IActionResult> CountWhere()
        {
            return Ok(await _myService.CountWhereAsync());
        }

        [HttpGet("max")]
        public async Task<IActionResult> Max()
        {
            return Ok(await _myService.MaxAsync());
        }

        [HttpGet("maxwhere")]
        public async Task<IActionResult> MaxWhere()
        {
            return Ok(await _myService.MaxWhereAsync());
        }

        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            await _myService.AddAsync();
            return Ok();
        }

        [HttpGet("addrange")]
        public async Task<IActionResult> AddRange()
        {
            await _myService.AddRangeAsync();
            return Ok();
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update()
        {
            await _myService.UpdateAsync();
            return Ok();
        }

        [HttpGet("updatebyid")]
        public async Task<IActionResult> UpdateById()
        {
            await _myService.UpdateByIdAsync();
            return Ok();
        }

        [HttpGet("updaterange")]
        public async Task<IActionResult> UpdateRange()
        {
            await _myService.UpdateRangeAsync();
            return Ok();
        }

        [HttpGet("updatewhere")]
        public async Task<IActionResult> UpdateWhere()
        {
            await _myService.UpdateWhereAsync();
            return Ok();
        }

        [HttpGet("remove")]
        public async Task<IActionResult> Remove()
        {
            await _myService.RemoveAsync();
            return Ok();
        }

        [HttpGet("removebyid")]
        public async Task<IActionResult> RemoveById()
        {
            await _myService.RemoveByIdAsync();
            return Ok();
        }

        [HttpGet("removerange")]
        public async Task<IActionResult> RemoveRange()
        {
            await _myService.RemoveRangeAsync();
            return Ok();
        }

        [HttpGet("removewhere")]
        public async Task<IActionResult> RemoveWhere()
        {
            await _myService.RemoveWhereAsync();
            return Ok();
        }

        [HttpGet("trans")]
        public async Task<IActionResult> Transaction()
        {
            await _myService.ExecuteTransactionAsync();
            return Ok();
        }

        [HttpGet("sql")]
        public async Task<IActionResult> Sql()
        {
            return Ok(await _myService.ExecuteSqlRawAsync());
        }

        [HttpGet("sqlcommand")]
        public async Task<IActionResult> SqlCommand()
        {
            return Ok(await _myService.ExecuteSqlRawCommandAsync());
        }
    }
}
