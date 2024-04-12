using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Repositories;
using Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using FluentLogger.Interfaces;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : BaseController
{
    private readonly UserRepository userRepository;
    private readonly InviteRepository inviteRepository;
    private readonly ApiInfoRepository apiInfoRepository;
    private readonly AmazonSimpleEmailServiceClient amazonSes;
    private readonly ILog logger;

    public UserController(
        UserRepository userRepository,
        InviteRepository inviteRepository,
        ApiInfoRepository apiInfoRepository,
        AmazonSimpleEmailServiceClient amazonSes,
        ILog logger)
    {
        this.userRepository = userRepository;
        this.inviteRepository = inviteRepository;
        this.apiInfoRepository = apiInfoRepository;
        this.amazonSes = amazonSes;
        this.logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("List")]
    public IEnumerable<User> List()
    {
        return userRepository.GetAll();
    }
    //[AllowAnonymous]
    //[HttpGet]
    //[Route("Error")]
    //public void Error()
    //{
    //    throw new Exception("Crazy Exception");
    //}
    [HttpPost]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Create")]
    public async Task<GenericResponse> Create ([FromBody] UserRequest userRequest)
    {
        var user = userRequest.User;
        logger.Info("Creating user", user);
        user.Id = Guid.NewGuid().ToString();
        var passwordHasher = new PasswordHasher<User>();
        if (user.Password != null)
            user.HashedPassword = passwordHasher.HashPassword(user, user.Password);

        var existingUser = userRepository.FindByEmail(user.Email);
        if (existingUser != null)
            return new GenericResponse { Success = false, Message = "User already exists, please login" };

        user.RoleEnum = UserType.User;
        user.CreatedAt = DateTime.UtcNow;
        user.ModifiedAt = DateTime.UtcNow;
        userRepository.Insert(user);
        var invite = new Invite(Guid.NewGuid().ToString(),user.Id, DateTime.UtcNow.AddHours(2), userRequest.RedirectUrl);
        inviteRepository.Insert(invite);
        await SendInvite(user, invite);
        return new GenericResponse { Success = true, Message = "User created Successfully", Id = user.Id};
    }

    [HttpPost]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("ForgotPassword")]
    public async Task<GenericResponse> ForgotPassword([FromQuery] string email)
    {
        var user = userRepository.FindByEmail(email);
        if (user == null) return new GenericResponse { Message = "If a user exists then you should have an email", Success = true };
        var invite = new Invite(Guid.NewGuid().ToString(), user.Id, DateTime.UtcNow.AddHours(2), null);
        inviteRepository.Insert(invite);
        await SendForgotPassword(user, invite);
        return new GenericResponse { Message = "If a user exists then you should have an email", Success = true };
    }

    [HttpPost]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("SetPassword")]
    public async Task<GenericResponse> SetPassword([FromBody] ResetRequest req)
    {
        logger.Info("Setting password", req);
        var invite = inviteRepository.Get(req.InviteId);
        if (invite == null || invite.ExpiresAt < DateTime.UtcNow) return new GenericResponse { Message = "Invite not found or expired", Success = false };
        var user = userRepository.Get(invite.UserId);
        var passwordHasher = new PasswordHasher<User>();
        user.HashedPassword = passwordHasher.HashPassword(user, req.Password);
        userRepository.Update(user);
        return new GenericResponse { Message = "Password reset successful", Success = true };
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Promote")]
    public async Task<GenericResponse> Promote(string id)
    {
        var user = userRepository.Get(id);
        logger.Info("Promoting user", user);

        if (user == null) return new GenericResponse { Success = false, Message = "User could not be promoted" };
        user.RoleEnum = UserType.Admin;
        userRepository.Update(user);
        return new GenericResponse { Success = true, Message = "User created Successfully", Id = user.Id };
    }

    private async Task SendForgotPassword(User user, Invite invite)
    {
        var url = $"{Request.Scheme}://{Request.Host}/#/forgot-password?id={invite.Id}";
        logger.Info("Sending forgot password", url);
        await amazonSes.SendEmailAsync(new SendEmailRequest
        {
            Source = "fintech@savvyotter.net",
            Destination = new Destination
            {
                ToAddresses = new[] { user.Email }.ToList()
            },
            Message = new Message(new Content("Forgot Password: Plant Agents Collective"),
            new Body
            {
                Html =
            new Content($"Click the link to <a href='{url}'>reset your password</a>"
            )
            })
        });
    }

    private async Task SendInvite(User user, Invite invite)
    {
        var url = $"{Request.Scheme}://{Request.Host}/invite/verify?id={invite.Id}";
        logger.Info("Send invite", url);
        await amazonSes.SendEmailAsync(new SendEmailRequest
        {
            Source = "fintech@savvyotter.net",
            Destination = new Destination
            {
                ToAddresses = new []{ user.Email }.ToList()
            },
            Message = new Message(new Content("Plant Agents Collective"),
            new Body
            {
                Html =
            new Content($"The Plants Agents Collective requires you to verify your email in order to proceed.  Please click the link to verify your email.<br /> <a href='{url}'>Verify your Email</a>"
            )
            })
        });
    }

    [HttpPut]
    [Authorize(Roles ="Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Update")]
    public User? Update([FromBody] User user)
    {
        logger.Info("Updating user", user);
        if (user.Id == null) return null;
        var existingUser = userRepository.Get(user.Id);
        //if (existingUser == null) return null;
        existingUser.Email = user.Email;
        existingUser.ModifiedAt = DateTime.Now;
        if (existingUser.Password != null && user.Password != null)
        {
            var passwordHasher = new PasswordHasher<User>();
            existingUser.HashedPassword = passwordHasher.HashPassword(user, user.Password);
        }
        user.ModifiedAt = DateTime.UtcNow;
        userRepository.Update(user);
        return user;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Search")]
    public IEnumerable<User> Search(bool showAdminOnly, string? email, int skip=0, int take = 20)
    {
        return userRepository.Find(showAdminOnly,email, skip, take);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Export")]
    public IEnumerable<string[]> Export(bool showAdminOnly, int offset)
    {
       var users = userRepository.Find(showAdminOnly, null, 0, int.MaxValue);
#pragma warning disable CS8601 // Possible null reference assignment.
        return users.Select(u => new string[] { u.Id, u.Email, u.Role, u.Verified.ToString(), u.IntendedUse, u.CreatedAt.AddMinutes(-offset).ToString(), u.ModifiedAt.AddMinutes(-offset).ToString() });
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Resend")]
    public async Task<bool> Resend(string id)
    {
        var user = userRepository.Get(id);
        var invite = new Invite(Guid.NewGuid().ToString(), id, DateTime.UtcNow.AddDays(2), null);
        inviteRepository.Insert(invite);
        await SendInvite(user, invite);
        return true;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User,Vendor")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("GenApi")]
    public ApiResponse GenApi()
    {
        var info = apiInfoRepository.FindByUserId(UserId);
        if (info == null)
        {
            return new ApiResponse { Success = false, Message = "Must provide api information before obtaining a key. <a style='color:#fff' href='/#/api-registration'>click here</a>" };
        }

        return new ApiResponse { Success = true, Value = userRepository.GenApiKey(UserId) };
    }
    [HttpGet]
    [Route("GetApiKey")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "User,Admin,Vendor")]
    public GenericResponse GetApi()
    {
        var user = userRepository.Get(UserId);
        return new GenericResponse { Id = user.ApiKey, Success = true };
    }


    [HttpPost]
    [Route("Delete")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Authorize]
    public bool Delete(string id)
    {
        //TODO:  Ensure that authorization is achieved.
        var user = userRepository.Get(id);
        if (user != null)
        {
            userRepository.Delete(user);
            return true;
        }
        return false;
    }
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("logout")]
    public async Task<bool> Logout()
    {
        await HttpContext.SignOutAsync();
        return true;
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<LoginResponse> Login([FromBody]LoginRequest request)
    {
        var user = userRepository.FindByEmail(request.Email);
        var passwordHasher = new PasswordHasher<User>();
        if (user == null || string.IsNullOrEmpty(request.Password) || user.HashedPassword == null) return new LoginResponse { Success = false, Verified = false } ;
        var result =  passwordHasher.VerifyHashedPassword(user, user.HashedPassword, request.Password);
        var success = result == PasswordVerificationResult.Success && user.Verified;
        if (success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id ?? ""),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                IsPersistent = true,
                IssuedUtc =DateTime.UtcNow,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
        return new LoginResponse { Email = user.Email, Role = user.Role ?? "User", Success = result == PasswordVerificationResult.Success, Verified = user.Verified };
    }
}
public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
public class LoginResponse
{
    public string? Email { get; set; }
    public string? Role { get; set; }
    public required bool Verified { get; set; }
    public required bool Success { get; set; }
}
public class ApiResponse
{
    public string? Value { get; set; }
    public bool Success { get; internal set; }
    public string Message { get; internal set; }
}
public class GenericResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Id { get; set; }
    public string? RedirectUrl { get; set; }
}

public class ResetRequest
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string InviteId { get; set; }
    public string Password { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}