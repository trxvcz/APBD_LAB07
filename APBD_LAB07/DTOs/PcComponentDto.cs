namespace APBD_LAB07.DTOs;

public class PcComponentDto(int argAmount, ComponentDto componentDto)
{
    public int Amount{get;set;}
    public ComponentDto Component{ get; set; }
}