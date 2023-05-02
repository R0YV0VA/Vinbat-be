using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vinbat_be.Contexts;
using Vinbat_be.Models;

namespace Vinbat_be.Controllers;

[ApiController]
[Route("users")]
public class UsersController : Controller
{
    private readonly UsersContext usersContext;
    private readonly JwtAuthentificationManager jwtAuthentificationManager;
    public UsersController(UsersContext _usersContext, JwtAuthentificationManager _jwtAuthentificationManager)
    {
        this.usersContext = _usersContext;
        this.jwtAuthentificationManager = _jwtAuthentificationManager;
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpGet("my-account")]
    public async Task<IActionResult> GetMyUserSettings()
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var myAccount = new MyAccount();
        myAccount.Name = user.Name;
        myAccount.Login = user.Login;
        myAccount.PurchasesAmount = user.PurchasesAmount;
        myAccount.Discount = user.Discount;
        myAccount.Status = user.Status;
        return Ok(myAccount);
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpGet("is-logged")]
    public IActionResult IsLogged()
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        if (userId == null)
            return Unauthorized();
        return Ok();
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpPut("change-login-name")]
    public async Task<IActionResult> ChangeLoginName([FromBody] LoginName? loginName)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (loginName.Name == "" && loginName.Login == "")
            return Unauthorized("Введіть нові дані!");
        if (loginName.Name != "")
        {
            user.Name = loginName.Name;
        }
        if (loginName.Login != "")
        {
            user.Login = loginName.Login;
            await usersContext.SaveChangesAsync();
            return Ok();
        }
        await usersContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] NewPass newPass)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (newPass.OldPassword != "")
            if(user.Password == JwtAuthentificationManager.CreateMD5(newPass.OldPassword))
                user.Password = JwtAuthentificationManager.CreateMD5(newPass.NewPassword);
            else
                return BadRequest("Паролі не співпадають!");
        else
            return Unauthorized("Введіть пароль!");
        await usersContext.SaveChangesAsync();
        var token = await jwtAuthentificationManager.Authenticate(user.Login, newPass.NewPassword, usersContext);
        return Ok(token);
    }
}
