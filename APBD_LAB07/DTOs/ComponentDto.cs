namespace APBD_LAB07.DTOs;

public class ComponentDto(string componentCode, string componentName, string componentDescription, ManufacturerDto manufacturerDto, TypeDto typeDto)
{
    public string Code{get;set;}
    public string Name{get;set;}
    public string Description{get;set;}
    public ManufacturerDto Manufacturer{get;set;}
    public TypeDto Type{get;set;}
}