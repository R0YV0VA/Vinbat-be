namespace Vinbat_be.Models;

public class PostBattery
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public int Capacity { get; set; }
    public int CapacitiveGroup { get; set; }
    public int StartingСurrent { get; set; }
    public int Voltage { get; set; }
    public int PositiveTerminal { get; set; }
    public int Application { get; set; }
    public int TypeOfElectolite { get; set; }
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public int Price { get; set; }
    public int WholesalePrice { get; set; }
    public bool Availability { get; set; }
}
