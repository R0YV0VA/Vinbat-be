namespace Vinbat_be.Models;

public class Case
{
    /*
    [Id] INT NOT NULL,
	[Username] NVARCHAR (50) NOT NULL,
	[Connection] NVARCHAR (50) NOT NULL,
	[Message] NVARCHAR (50) NOT NULL,
	[Status] BIT NOT NULL,
     */
	public int Id { get; set; }
	public string Username { get; set; }
	public string Connection { get; set; }
	public string Message { get; set; }
	public bool Status { get; set; }
}
