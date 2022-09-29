using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Services;
using TeslaUsbKeepCarAwake.Services.Contracts;

namespace TeslaUsbKeepCarAwake.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly HelloService _helloService;
        private readonly IMqttService _mqttService;
        private readonly Internals _internals;

        public HelloController(HelloService helloService, IMqttService mqttService, Internals internals)
        {
            _helloService = helloService;
            _mqttService = mqttService;
            _internals = internals;
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

        [HttpGet]
        public void ReduceApplicationStartupTime()
        {
            _internals.ApplicationStartup = new DateTime(2022, 9, 1);
        }

        [HttpGet]
        public bool IsMqttClientConnected() => _mqttService.IsConnected;

        [HttpGet]
        public Task<DtoValue<int>> DisconnectedMqttServices()
        {
            var value = new DtoValue<int>(_mqttService.IsConnected ? 0 : 1);
            return Task.FromResult(value);
        }
    }
}
