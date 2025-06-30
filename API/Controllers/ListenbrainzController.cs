using Application.Services.Interfaces;
using Core.DTO.ListenBrainz;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/v1/listenbrainz")]
public class ListenbrainzController(IListenBrainzService listenBrainzService) : ControllerBase
{
    [HttpGet("validate-token")]
    public async Task<ActionResult<ValidateTokenResponse>> ValidateToken()
    {
        var token = GetToken(HttpContext);

        if (token == null)
        {
            return BadRequest("You need to provide an Authorization token.");
        }

        return Ok(await listenBrainzService.ValidateToken(token));
    }

    [HttpPost("submit-listens")]
    public async Task<ActionResult<SubmitListensResponse>> SubmitListens(
        [FromBody] SubmitListensRequest submitListensRequest)
    {
        var token = GetToken(HttpContext);
        if (token == null)
        {
            return BadRequest("You need to provide an Authorization token.");
        }

        return Ok(await listenBrainzService.SubmitListens(submitListensRequest, token));
    }

    // TODO: Implement as middleware
    private static string? GetToken(HttpContext context)
    {
        return context
            .Request
            .Headers
            .Authorization
            .ToString()
            .Split(" ")
            .ElementAtOrDefault(1);
    }
}