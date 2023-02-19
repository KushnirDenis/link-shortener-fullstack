using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/v{version:apiVersion}/l/")]
public class LinkHandlerController : ControllerBase
{
    
}