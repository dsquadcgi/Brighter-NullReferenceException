using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using WebApplication.Events;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("/")]
    public class StupidController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<StupidController> _logger;

        public StupidController(
            IAmACommandProcessor commandProcessor,
            ILogger<StupidController> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                _commandProcessor.Post(new StupidEvent
                {
                    CreationDate = DateTime.UtcNow
                });

                _logger.LogInformation("event sent succesfully");

                return Ok();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "NullReference reproduced: send failed");
                return StatusCode(Settings.StatusCodeWhenReproduced);
            }
            // TODO comment if needed as 3 different exceptions can be raised
            catch (Exception e)
            {
                _logger.LogError(e, "send failed but ignored, we don't care about this exception");
                return Ok();
            }
        }
    }
}