namespace APBD_LAB07.DTOs;

public class TypeDto(int componentTypeId, string componentTypeAbbreviation, string componentTypeName)
{
    public int Id{get;set;}
    public string Abbreviation{get;set;}
    public string Name{get;set;}
}