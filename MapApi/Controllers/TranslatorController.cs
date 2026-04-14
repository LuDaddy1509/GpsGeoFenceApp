using MapApi.Common;
using MapApi.Services;
using Microsoft.AspNetCore.Mvc;
using MapApi.Dtos.Translator;
using Microsoft.AspNetCore.Authorization;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/translator")]
public sealed class TranslatorController : ApiControllerBase
{
    private readonly TranslatorClient _translator;

    public TranslatorController(TranslatorClient translator)
    {
        _translator = translator;
    }

    [HttpPost("translate")]
    [AllowAnonymous]
    public async Task<ActionResult> Translate([FromBody] TranslateRequest request, CancellationToken ct)
    {
        var result = await _translator.TryTranslateAsync(request.Text, request.ToLang, request.FromLang, ct);
        return Content(result ?? request.Text, "text/plain");
    }
}
