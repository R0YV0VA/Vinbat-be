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
    public UsersController(UsersContext _usersContext)
    {
        this.usersContext = _usersContext;
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
    public async Task<IActionResult> ChangeLoginName([FromBody] LoginName LoginName)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (LoginName != null)
            user.Name = LoginName.Name;
        if (LoginName != null)
            user.Login = LoginName.Login;
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
        if (newPass.Password != null)
            if(user.Password == JwtAuthentificationManager.CreateMD5(newPass.Password))
                user.Password = JwtAuthentificationManager.CreateMD5(newPass.NewPassword);
            else
                return Unauthorized("Паролі не співпадають!");
        else
            return Unauthorized("Введіть пароль!");
        await usersContext.SaveChangesAsync();
        return Ok();
    }
}
