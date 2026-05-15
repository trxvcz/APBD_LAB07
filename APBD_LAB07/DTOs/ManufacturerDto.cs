namespace APBD_LAB07.DTOs;

public class ManufacturerDto(int componentManufacturerId, string componentManufacturerAbbreviation, string componentManufacturerFullName, DateTime componentManufacturerFoundationDate)
{
    public int Id{get;set;}
    public string Abbreviation{get;set;}
    public string FullName{get;set;}
    public DateTime FoundationDate{get;set;}
}