using Vinbat_be.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vinbat_be.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Vinbat_be.Controllers;

[ApiController]
[Route("auth")]
public class AuthorizeController : ControllerBase
{
    private readonly JwtAuthentificationManager jwtAuthentificationManager;
    private readonly UsersContext usersContext;

    public AuthorizeController(JwtAuthentificationManager jwtAuthentificationManager, UsersContext usersContext)
    {
        this.jwtAuthentificationManager = jwtAuthentificationManager;
        this.usersContext = usersContext;
    }
    [AllowAnonymous]
    [EnableCors]
    [HttpPost("login")]
    public async Task<IActionResult> SignIn([FromBody] LoginUser user)
    {
        var token = await jwtAuthentificationManager.Authenticate(user.Login, user.Password, usersContext);
        if (token == null)
            return Unauthorized();
        return Ok(token);
    }

    [AllowAnonymous]
    [EnableCors]
    [HttpPost("register")]
    public async Task<IActionResult> SignUp([FromBody] RegisterUser signUpUser)
    {
        User user = new User();
        user.Id = new Random().Next(1000, Int32.MaxValue);
        user.Name = signUpUser.Name;
        user.Login = signUpUser.Login;
        user.Password = JwtAuthentificationManager.CreateMD5(signUpUser.Password);
        user.Status = 1;
        await usersContext.Users.AddAsync(user);
        await usersContext.SaveChangesAsync();
        return Ok();
    }

    [AllowAnonymous]
    [EnableCors]
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
