namespace Vinbat_be.Models;

public class User
{
    /*
    [Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Login]    NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR (50) NOT NULL,
    [PurchasesAmount] INT,
	[Discount] INT,
    [Status] INT NOT NULL,
    PRIMARY KEY CLUSTERED([Id] ASC)*/
    public int Id { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int PurchasesAmount { get; set; }
    public int Discount { get; set; }
    public int Status { get; set; }
}
