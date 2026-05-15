namespace APBD_LAB07.Models;

public class Component
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ComponentManufacturersId { get; set; }
    public ComponentManufacturer ComponentManufacturer { get; set; } = null!;
    public int ComponentTypesId { get; set; }
    public ComponentType ComponentType { get; set; } = null!;
    public ICollection<PcComponent> PCComponents { get; set; } = new List<PcComponent>();
}