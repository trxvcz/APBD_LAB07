namespace APBD_LAB07.Models;

public class PcComponent
{
    public int PCId { get; set; }
    public PC PC { get; set; } = null!;
    public string ComponentCode { get; set; } = null!;
    public Component Component { get; set; } = null!;
    public int Amount { get; set; }
}