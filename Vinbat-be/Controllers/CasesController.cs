﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vinbat_be.Contexts;
using Vinbat_be.Models;

namespace Vinbat_be.Controllers;

[ApiController]
[Route("cases")]
public class CasesController : Controller
{
    private readonly CasesContext casesContext;
    public CasesController(CasesContext casesContext)
    {
        this.casesContext = casesContext;
    }
    [AllowAnonymous]
    [EnableCors("NonAuth")]
    [HttpPost]
    public async Task<IActionResult> CreateCase([FromBody] CaseAdd addcase)
    {
        Case @case = new Case();
        @case.Id = new Random().Next(1000, Int32.MaxValue/10);
        @case.Username = addcase.Username;
        @case.Connection = addcase.Connection;
        @case.Message = addcase.Message;
        @case.Status = false;
        await casesContext.Cases.AddAsync(@case);
        await casesContext.SaveChangesAsync();
        return Ok();
    }
}
