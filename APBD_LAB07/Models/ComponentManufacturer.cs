namespace APBD_LAB07.Models;

public class ComponentManufacturer
{
    public int Id { get; set; }
    public string Abbreviation { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime FoundationDate { get; set; }
    public ICollection<Component> Components { get; set; } = new List<Component>();
}