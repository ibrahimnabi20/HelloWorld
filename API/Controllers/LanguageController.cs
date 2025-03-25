using System.Diagnostics;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguageController : ControllerBase
{
    private static readonly ActivitySource Activity = new("API");
    private readonly ILogger<LanguageController> _logger;

    public LanguageController(ILogger<LanguageController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        using var activity = Activity.StartActivity("GetLanguages");
        _logger.LogInformation("Fetching languages");
        var language = LanguageService.Instance.GetLanguages();
        _logger.LogInformation("Fetched languages: {@Languages}", language);
        return Ok(new GetLanguageModel.Response { DefaultLanguage = language.DefaultLanguage, Languages = language.Languages });
    }
}