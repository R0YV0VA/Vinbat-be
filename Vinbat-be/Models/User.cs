﻿namespace Vinbat_be.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int PurchasesAmount { get; set; }
    public int Discount { get; set; }
    public int Status { get; set; }
}
