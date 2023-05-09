using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Security.Claims;
using Vinbat_be.Contexts;
using Vinbat_be.Models;
using Vinbat_be.Services;

namespace Vinbat_be.Controllers;

[ApiController]
[Route("catalogue")]
public class CatalogueController : Controller
{
    private readonly BatteriesContext batteriesContext;
    private readonly TiresContext tiresContext;
    private readonly UsersContext usersContext;
    private readonly string Link;
    public CatalogueController(BatteriesContext batteriesContext, TiresContext tiresContext, UsersContext usersContext, IConfiguration configuration)
    {
        this.batteriesContext = batteriesContext;
        this.tiresContext = tiresContext;
        this.usersContext = usersContext;
        Link = $"{configuration["AllowedOrigin"]}:{configuration["Port"]}/Images/" ;
    }

    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpGet("batteries")]
    public async Task<IActionResult> GetAllBatteries([FromQuery] int page)
    {
        page--;
        var batteries = await batteriesContext.Batteries.Skip(page * 20).Take(20).ToListAsync();
        foreach(var item in batteries)
        {
            item.Image = Link + item.Image;
        }
        return Ok(batteries);
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpPost("batteries")]
    public async Task<IActionResult> AddBattery([FromForm] PostBattery battery)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (user.Status != 1)
            return BadRequest();
        string imageName = Guid.NewGuid().ToString() + ".png";
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", imageName);
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await battery.Image.CopyToAsync(stream);
        }
        var newBattery = new Battery
        {
            Id = new Random().Next(1000, Int32.MaxValue),
            Name = battery.Name,
            Brand = battery.Brand,
            Capacity = battery.Capacity,
            CapacitiveGroup = battery.CapacitiveGroup,
            StartingСurrent = battery.StartingСurrent,
            Voltage = battery.Voltage,
            PositiveTerminal = battery.PositiveTerminal,
            Application = battery.Application,
            TypeOfElectolite = battery.TypeOfElectolite,
            Image = Link + imageName,
            Description = battery.Description,
            Price = battery.Price,
            WholesalePrice = battery.WholesalePrice,
            Availability = battery.Availability
        };
        await batteriesContext.Batteries.AddAsync(newBattery);
        await batteriesContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpPost("load-battaries-excel")]
    public async Task<IActionResult> LoadBattariesExcel([FromForm] ExcelTable table)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (user.Status != 1)
            return BadRequest();
        string fileName = Guid.NewGuid().ToString() + ".xlsx";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Excel", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await table.Table.CopyToAsync(stream);
        }
        var reader = new ExcelReader();
        var batteries = await reader.ReadBattariesFromExcel(table.SheetName, filePath);
        if (batteries == null)
            return BadRequest();
        foreach (var battery in batteries)
        {
            await batteriesContext.Batteries.AddAsync(battery);
        }
        await batteriesContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [EnableCors("AuthReq")]
    [HttpPost("load-tires-excel")]
    public async Task<IActionResult> LoadTiresExcel([FromForm] ExcelTable table)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var user = await usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();
        if (user.Status != 1)
            return BadRequest();
        string fileName = Guid.NewGuid().ToString() + ".xlsx";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Excel", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await table.Table.CopyToAsync(stream);
        }
        var reader = new ExcelReader();
        var tires = await reader.ReadTiresFromExcel(table.SheetName, filePath);
        if (tires == null)
            return BadRequest();
        foreach (var tire in tires)
        {
            await tiresContext.Tires.AddAsync(tire);
        }
        await tiresContext.SaveChangesAsync();
        return Ok();
    }
}