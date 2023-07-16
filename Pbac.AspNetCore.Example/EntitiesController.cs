using Microsoft.AspNetCore.Mvc;

namespace Pbac.AspNetCore.Example;

[ApiController]
[Route("/entity")]
public class EntitiesController : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(Permissions.ReadEntity)]
    public IActionResult Get()
        => NoContent();

    [HttpDelete]
    [AuthorizePermission(Permissions.DeleteEntity)]
    public IActionResult Delete()
        => NoContent();
}
