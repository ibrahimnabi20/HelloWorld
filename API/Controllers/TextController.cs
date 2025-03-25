using API.Models;
using API.Services;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TextController : ControllerBase
{
    private static readonly ActivitySource Activity = new("API");
    private readonly ILogger<TextController> _logger;

    public TextController(ILogger<TextController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get(string languageCode)
    {
        using var activity = Activity.StartActivity("GetGreetingAndPlanet");
        activity?.SetTag("language_code", languageCode);
        _logger.LogInformation("Received request for language: {LanguageCode}", languageCode);

        try
        {
            var greeting = GreetingService.Instance.Greet(new GreetingRequest { LanguageCode = languageCode });
            _logger.LogInformation("Greeting generated: {Greeting}", greeting.Greeting);

            var planet = PlanetService.Instance.GetPlanet();
            _logger.LogInformation("Planet selected: {Planet}", planet.Planet);

            var response = new GetGreetingModel.Response
            {
                Greeting = greeting.Greeting,
                Planet = planet.Planet
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing greeting for {LanguageCode}", languageCode);
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            return StatusCode(500, "An error occurred");
        }
    }
}