using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;
using System.Security.Cryptography;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistrationInviteController : BaseController
{
    private static readonly TimeSpan InviteLifetime = TimeSpan.FromHours(72);

    private readonly RegistrationInviteRepository registrationInviteRepository;
    private readonly UserRepository userRepository;
    private readonly AmazonSimpleEmailServiceClient amazonSes;
    private readonly ILog logger;

    public RegistrationInviteController(
        RegistrationInviteRepository registrationInviteRepository,
        UserRepository userRepository,
        AmazonSimpleEmailServiceClient amazonSes,
        ILog logger)
    {
        this.registrationInviteRepository = registrationInviteRepository;
        this.userRepository = userRepository;
        this.amazonSes = amazonSes;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Create")]
    public async Task<RegistrationInviteCreateResponse> Create([FromBody] RegistrationInviteCreateRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant() ?? "";
        if (string.IsNullOrEmpty(email))
        {
            return new RegistrationInviteCreateResponse { Success = false, Message = "Email is required" };
        }

        if (userRepository.FindByEmail(email) != null)
        {
            return new RegistrationInviteCreateResponse { Success = false, Message = "A user with this email already exists" };
        }

        if (!TryResolveInviteRole(request.Role, out var invitedRole, out var roleError))
        {
            return new RegistrationInviteCreateResponse { Success = false, Message = roleError ?? "Invalid role" };
        }

        registrationInviteRepository.ExpirePendingForEmail(email);

        var token = NewInviteToken();
        var id = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;
        var invite = new RegistrationInvite
        {
            Id = id,
            Token = token,
            Email = email,
            CreatedAt = now,
            ExpiresAt = now.Add(InviteLifetime),
            Status = RegistrationInviteStatus.Pending.ToString(),
            Role = invitedRole.ToString(),
            InvitedByUserId = string.IsNullOrEmpty(UserId) ? null : UserId,
            AcceptedUserId = null
        };

        registrationInviteRepository.Insert(invite);

        var inviteLink = $"{Request.Scheme}://{Request.Host}/#/accept-invite?token={Uri.EscapeDataString(token)}";

        try
        {
            await SendInviteEmail(email, inviteLink, invite.ExpiresAt, invite.Role);
        }
        catch (AmazonSimpleEmailServiceException ex)
        {
            logger.Error("Registration invite email could not be sent (AWS SES). Invite was created.", ex);
        }

        return new RegistrationInviteCreateResponse
        {
            Success = true,
            Message = "Invitation created",
            InviteLink = inviteLink,
            ExpiresAt = invite.ExpiresAt
        };
    }

    [HttpGet]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Validate")]
    public RegistrationInviteValidateResponse Validate([FromQuery] string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new RegistrationInviteValidateResponse
            {
                Valid = false,
                Message = "Invitation token is missing"
            };
        }

        var invite = registrationInviteRepository.GetByToken(token.Trim());
        if (invite == null)
        {
            return new RegistrationInviteValidateResponse
            {
                Valid = false,
                Message = "This invitation link is not valid"
            };
        }

        RefreshExpiryIfNeeded(invite);

        if (string.Equals(invite.Status, RegistrationInviteStatus.Expired.ToString(), StringComparison.Ordinal))
        {
            return new RegistrationInviteValidateResponse
            {
                Valid = false,
                Status = invite.Status,
                Message = "This invitation has expired. Ask an administrator for a new invite."
            };
        }

        if (string.Equals(invite.Status, RegistrationInviteStatus.Accepted.ToString(), StringComparison.Ordinal))
        {
            return new RegistrationInviteValidateResponse
            {
                Valid = false,
                Status = invite.Status,
                Message = "This invitation has already been used"
            };
        }

        return new RegistrationInviteValidateResponse
        {
            Valid = true,
            Email = invite.Email,
            Role = string.IsNullOrEmpty(invite.Role) ? UserType.User.ToString() : invite.Role,
            Status = invite.Status,
            ExpiresAt = invite.ExpiresAt,
            Message = "Invitation is valid"
        };
    }

    [HttpPost]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Accept")]
    public Task<GenericResponse> Accept([FromBody] RegistrationInviteAcceptRequest request)
    {
        var token = request.Token?.Trim() ?? "";
        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(new GenericResponse { Success = false, Message = "Invitation token is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(new GenericResponse { Success = false, Message = "Password is required" });
        }

        if (request.Password.Length < 10)
        {
            return Task.FromResult(new GenericResponse { Success = false, Message = "Password must be at least 10 characters" });
        }

        var invite = registrationInviteRepository.GetByToken(token);
        if (invite == null)
        {
            return Task.FromResult(new GenericResponse { Success = false, Message = "This invitation link is not valid" });
        }

        RefreshExpiryIfNeeded(invite);

        if (!string.Equals(invite.Status, RegistrationInviteStatus.Pending.ToString(), StringComparison.Ordinal))
        {
            var msg = string.Equals(invite.Status, RegistrationInviteStatus.Expired.ToString(), StringComparison.Ordinal)
                ? "This invitation has expired"
                : "This invitation has already been used";
            return Task.FromResult(new GenericResponse { Success = false, Message = msg });
        }

        var email = invite.Email;
        if (userRepository.FindByEmail(email) != null)
        {
            return Task.FromResult(new GenericResponse { Success = false, Message = "A user with this email already exists" });
        }

        var roleForUser = UserType.User;
        if (!string.IsNullOrWhiteSpace(invite.Role) &&
            Enum.TryParse<UserType>(invite.Role.Trim(), ignoreCase: true, out var parsedRole))
        {
            roleForUser = parsedRole;
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            Verified = true,
            RoleEnum = roleForUser
        };
        var passwordHasher = new PasswordHasher<User>();
        user.HashedPassword = passwordHasher.HashPassword(user, request.Password);
        userRepository.Insert(user);

        invite.Status = RegistrationInviteStatus.Accepted.ToString();
        invite.AcceptedUserId = user.Id;
        registrationInviteRepository.Update(invite);

        logger.Info("Registration invite accepted", new { InviteId = invite.Id, UserId = user.Id });

        return Task.FromResult(new GenericResponse { Success = true, Message = "Account created. You can sign in now." });
    }

    private void RefreshExpiryIfNeeded(RegistrationInvite invite)
    {
        if (!string.Equals(invite.Status, RegistrationInviteStatus.Pending.ToString(), StringComparison.Ordinal))
        {
            return;
        }

        if (invite.ExpiresAt >= DateTime.UtcNow)
        {
            return;
        }

        invite.Status = RegistrationInviteStatus.Expired.ToString();
        registrationInviteRepository.Update(invite);
    }

    private static bool TryResolveInviteRole(string? role, out UserType userType, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(role))
        {
            userType = UserType.User;
            return true;
        }

        if (!Enum.TryParse<UserType>(role.Trim(), ignoreCase: true, out userType))
        {
            error = $"Role must be one of: {string.Join(", ", Enum.GetNames(typeof(UserType)))}";
            return false;
        }

        return true;
    }

    private static string NewInviteToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task SendInviteEmail(string toEmail, string inviteLink, DateTime expiresAtUtc, string role)
    {
        var remaining = expiresAtUtc - DateTime.UtcNow;
        var expireDays = Math.Max(1, (int)Math.Ceiling(remaining.TotalDays));
        var dayWord = expireDays == 1 ? "day" : "days";
        var subject = "You've been invited to The Plant Agent Collective";
        logger.Info("Sending registration invite email", new { toEmail, inviteLink });
        await amazonSes.SendEmailAsync(new SendEmailRequest
        {
            Source = "no-reply@plantagents.org",
            Destination = new Destination { ToAddresses = new List<string> { toEmail } },
            Message = new Message(
                new Content(subject),
                new Body
                {
                    Html = new Content(
                        $"Your account role will be: <strong>{System.Net.WebUtility.HtmlEncode(role)}</strong>.<br /><br />" +
                        $"<a href='{inviteLink}'>Click here to accept your invitation.</a><br /><br />" +
                        $"Your invitation expires in {expireDays} {dayWord}.<br /><br />" +
                        $"If the link does not work, copy and paste this URL:<br />{inviteLink}")
                })
        });
    }
}

public class RegistrationInviteCreateRequest
{
    public string Email { get; set; } = "";

    /// <summary>Optional; defaults to User. Must match a <see cref="UserType"/> name.</summary>
    public string? Role { get; set; }
}

public class RegistrationInviteCreateResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? InviteLink { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class RegistrationInviteValidateResponse
{
    public bool Valid { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class RegistrationInviteAcceptRequest
{
    public string Token { get; set; } = "";
    public string Password { get; set; } = "";
}
