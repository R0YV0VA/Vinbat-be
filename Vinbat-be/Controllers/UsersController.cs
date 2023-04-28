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
public class UsersController : ControllerBase
{
    private readonly UsersContext usersContext;
    public UsersController(UsersContext _usersContext)
    {
        this.usersContext = _usersContext;
    }

    [Authorize]
    [EnableCors]
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
}
