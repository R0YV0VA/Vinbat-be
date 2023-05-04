namespace Vinbat_be.Models;

public class ResetPassword
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Password { get; set; }
    public string GUID { get; set; }
}
