namespace APBD_LAB07.DTOs;

public class CreateUpdatePcDto
{
    public string Name { get; set; } = null!;
    public float Weight{ get; set; }
    public int Warranty{ get; set; }
    public DateTime CreatedAt{ get; set; }
    public int Stock{ get; set; }
}