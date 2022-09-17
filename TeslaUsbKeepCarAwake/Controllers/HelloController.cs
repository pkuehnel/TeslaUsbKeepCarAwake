using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Services;

namespace TeslaUsbKeepCarAwake.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly HelloService _helloService;

        public HelloController(HelloService helloService)
        {
            _helloService = helloService;
        }

        [HttpPost]
        public async Task UpdateSettings([FromBody] Settings settings)
        {
            await _helloService.UpdateSettings(settings);
        }

        [HttpGet]
        public bool IsAlive()
        {
            return true;
        }
    }
}
