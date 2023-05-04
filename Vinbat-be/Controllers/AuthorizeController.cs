using Vinbat_be.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vinbat_be.Contexts;
using Microsoft.EntityFrameworkCore;
using Vinbat_be.Services;
using System.Net;

namespace Vinbat_be.Controllers;

[ApiController]
[Route("auth")]
public class AuthorizeController : Controller
{
    private readonly JwtAuthentificationManager jwtAuthentificationManager;
    private readonly UsersContext usersContext;
    private readonly ConfirmUsersContext confirmUsersContext;
    private readonly EmailService emailService;
    private readonly string Link;

    public AuthorizeController(JwtAuthentificationManager jwtAuthentificationManager, UsersContext usersContext, ConfirmUsersContext confirmUsersContext, IConfiguration configuration)
    {
        this.jwtAuthentificationManager = jwtAuthentificationManager;
        this.usersContext = usersContext;
        this.confirmUsersContext = confirmUsersContext;
        emailService = new EmailService(configuration["EmailAdress"], configuration["EmailPassword"]);
        Link = configuration["AllowedOrigin"];
    }
    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpPost("login")]
    public async Task<IActionResult> SignIn([FromBody] LoginUser user)
    {
        var token = await jwtAuthentificationManager.Authenticate(user.Login, user.Password, usersContext);
        if (token == null)
            return Unauthorized();
        return Ok(token);
    }

    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpPost("register")]
    public async Task<IActionResult> SignUp([FromBody] RegisterUser signUpUser)
    {
        ConfirmUsers user = new ConfirmUsers();
        user.Id = new Random().Next(1000, Int32.MaxValue);
        user.Name = signUpUser.Name;
        user.Login = signUpUser.Login;
        user.Password = JwtAuthentificationManager.CreateMD5(signUpUser.Password);
        user.GUID = Guid.NewGuid().ToString();
        var users = await usersContext.Users.Where(u => u.Login == user.Login).ToListAsync();
        var confirmUsers = await confirmUsersContext.ConfirmUsers.Where(u => u.Login == user.Login).ToListAsync();
        if (users.Count > 0 || confirmUsers.Count > 0)
            return Conflict();
        await emailService.Send(signUpUser.Login, "Підтвердження реєстрації", $"Для підтвердження реєстрації перейдіть по даному посиланню: {Link}:7224/auth/confirm-registration?token={user.GUID}\n\nНе показуйте це повідомлення нікому.");
        await confirmUsersContext.ConfirmUsers.AddAsync(user);
        await confirmUsersContext.SaveChangesAsync();
        return Ok();
    }

    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpGet("confirm-registration")]
    public async Task<ContentResult> ConfirmRegistration([FromQuery] string token)
    {
        var confirmUsers = await confirmUsersContext.ConfirmUsers.FirstOrDefaultAsync(u => u.GUID == token);
        if (confirmUsers == null)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Content = "<html><div style=\"align-items: center; justify-content: center; height: 100%; width: 100%;\"><div style=\"background-color: #ff3c3c; border-radius: 5px; box-shadow: 0 0 10px #b00000; align-items: center; justify-content: center; padding: 70px; text-align: center;\"><h1 style=\"font-size: 50px; font-weight: 600; color: #fff;\">400. Bad request</h1></div></div></html>"
            };
        }
        User user = new User();
        user.Id = new Random().Next(1000, Int32.MaxValue);
        user.Name = confirmUsers.Name; 
        user.Login = confirmUsers.Login;
        user.Password = confirmUsers.Password;
        user.Status = 1;
        await usersContext.Users.AddAsync(user);
        confirmUsersContext.ConfirmUsers.Remove(confirmUsers);
        await confirmUsersContext.SaveChangesAsync();
        await usersContext.SaveChangesAsync();
        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = (int)HttpStatusCode.OK,
            Content = "<html><div style=\"align-items: center; justify-content: center; height: 100%; width: 100%;\"><div style=\"background-color: #ff3c3c; border-radius: 5px; box-shadow: 0 0 10px #b00000; align-items: center; justify-content: center; padding: 70px; text-align: center;\"><h1 style=\"font-size: 50px; font-weight: 600; color: #fff;\">Registration confirmed</h1><p style=\"font-size: 20px; font-weight: 300; color: #fff;\">You can close this page</p></div></div></html>"
        };
    }

    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpPut("newpass")]
    public async Task<IActionResult> NewPass([FromBody] LoginUser newPassUser)
    {
        User user = await usersContext.Users.FirstOrDefaultAsync(u => u.Login == newPassUser.Login);
        if (user == null)
            return NotFound();
        user.Password = JwtAuthentificationManager.CreateMD5(newPassUser.Password);
        await usersContext.SaveChangesAsync();
        return Ok();
    }
}
