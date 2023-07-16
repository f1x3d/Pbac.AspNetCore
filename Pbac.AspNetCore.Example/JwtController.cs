using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Pbac.AspNetCore.Example;

[ApiController]
[Route("/jwt")]
public class JwtController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] Permissions[] permissions)
    {
        var permissionString = new PermissionSet<Permissions>(permissions).ToCompactString();

        return Ok(DummyJwtGenerator.Generate(new[] { new Claim(ClaimNames.Permissions, permissionString) }));
    }
}
