using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace webapi.Controllers;

[Route("[controller]")]
public class InviteController : BaseController
{
    private readonly InviteRepository inviteRepository;
    private readonly UserRepository userRepository;
    private readonly ILog logger;

    public InviteController(InviteRepository inviteRepository, UserRepository userRepository, ILog logger)
    {
        this.inviteRepository = inviteRepository;
        this.userRepository = userRepository;
        this.logger = logger;
    }
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("verify")]
    public ActionResult Verify(string id)
    {
        var invite = inviteRepository.Get(id);
        if (invite == null) return Content("Verification link already used or not found.");

        if (invite.ExpiresAt < DateTime.UtcNow) return Content("Expired verification link");
        logger.Info("Using verification link", id);
        //delete the verification link
        var user = userRepository.Get(invite.UserId);
        inviteRepository.Delete(invite);

        //mark verified
        user.Verified = true;
        userRepository.Update(user);
        return Redirect($"{Request.Scheme}://{Request.Host}/{invite.Path}");
     }
}