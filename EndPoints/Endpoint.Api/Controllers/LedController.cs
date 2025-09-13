using Auto.Application._Shared;
using Auto.Domain.ViewModels.LED;
using Auto.Facade.LED;
using Auto.MQTT;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Endpoint.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedController : ControllerBase
    {
        private readonly ILedFacade _facade;

        public LedController(ILedFacade facade)
        {
            _facade = facade;
        }

        // GET: api/<LedController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LedController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LedController>
        [HttpPost]
        public async Task<IActionResult> SendLedCommand([FromBody] LEDViewModel model)
        {
            var result = await _facade.SendLedStatus(model.Command, Topics.LedStatus);

            if(result.Status == OperationResultStatus.Success)
            {
                return Ok(new {message = result.Message });
            }

            return BadRequest(new { message = result.Message });
        }

        // PUT api/<LedController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LedController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
