using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;

namespace webapi.Controllers;

/// <summary>
/// Lookup endpoint for partner directories (Xerces, Jupiter, ...). Used by API
/// consumers and the MCP server to enumerate valid partner ids before issuing
/// partner-filtered exports or partner-aware vendor creates.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Policy = "ExportPolicy")]
public class PartnerController : BaseController
{
    private readonly PartnerRepository repo;

    public PartnerController(PartnerRepository repo)
    {
        this.repo = repo;
    }

    /// <summary>Returns all partner directories registered in PAC.</summary>
    [HttpGet("List")]
    public ActionResult<IEnumerable<Partner>> List()
    {
        return Ok(repo.List());
    }
}
