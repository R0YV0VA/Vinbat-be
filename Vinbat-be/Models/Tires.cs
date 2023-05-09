namespace Vinbat_be.Models;

public class Tires
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Season { get; set; }
    public int Radius { get; set; }
    public string? Size { get; set; }
    public string? Brand { get; set; }
    public string? Country { get; set; }
    public string? Date { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int Price { get; set; }
    public int WholesalePrice { get; set; }
    public bool Availability { get; set; }
}
