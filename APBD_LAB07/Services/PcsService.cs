using APBD_LAB07.Data;
using APBD_LAB07.DTOs;
using APBD_LAB07.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_LAB07.Services;

public class PcsService:IPcsService
{
    private readonly AppDbContext _context;
    
    public PcsService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PcDto>> GetAllPcsAsync()
    {
        var pcs = await _context.PCs.ToListAsync();
        return pcs.Select(p => new PcDto
        {
            Id = p.Id, 
            Name = p.Name, 
            Weight = p.Weight, 
            Warranty = p.Warranty, 
            CreatedAt = p.CreatedAt, 
            Stock = p.Stock
        });
    }

    public async Task<PcDetailsDto?> GetPcWithComponentsAsync(int id)
    {
        var pc = await _context.PCs
            .Include(p => p.PCComponents)
            .ThenInclude(pc => pc.Component)
            .ThenInclude(c => c.ComponentManufacturer)
            .Include(p => p.PCComponents)
            .ThenInclude(pc => pc.Component)
            .ThenInclude(c => c.ComponentType)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pc == null) return null;

        var components = pc.PCComponents.Select(c => new PcComponentDto(
            c.Amount,
            new ComponentDto(
                c.Component.Code,
                c.Component.Name,
                c.Component.Description,
                new ManufacturerDto(c.Component.ComponentManufacturer.Id, c.Component.ComponentManufacturer.Abbreviation, c.Component.ComponentManufacturer.FullName, c.Component.ComponentManufacturer.FoundationDate),
                new TypeDto(c.Component.ComponentType.Id, c.Component.ComponentType.Abbreviation, c.Component.ComponentType.Name)
            )
        ));

        return new PcDetailsDto
        {
            Id = pc.Id,
            Name = pc.Name,
            Weight = pc.Weight,
            Warranty = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock = pc.Stock,
            Components = components
        };
    }

    public async Task<PcDto> CreatePcAsync(CreateUpdatePcDto dto)
    {
        var pc = new PC
        {
            Name = dto.Name,
            Weight = dto.Weight,
            Warranty = dto.Warranty,
            CreatedAt = dto.CreatedAt,
            Stock = dto.Stock
        };

        _context.PCs.Add(pc);
        await _context.SaveChangesAsync();

        return new PcDto
        {
            Id = pc.Id, 
            Name = pc.Name, 
            Weight = pc.Weight, 
            Warranty = pc.Warranty, 
            CreatedAt = pc.CreatedAt, 
            Stock = pc.Stock
        };
    }

    public async Task<bool> UpdatePcAsync(int id, CreateUpdatePcDto dto)
    {
        var pc = await _context.PCs.FindAsync(id);
        if (pc == null) return false;

        pc.Name = dto.Name;
        pc.Weight = dto.Weight;
        pc.Warranty = dto.Warranty;
        pc.CreatedAt = dto.CreatedAt;
        pc.Stock = dto.Stock;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePcAsync(int id)
    {
        var pc = await _context.PCs.FindAsync(id);
        if (pc == null) return false;

        _context.PCs.Remove(pc);
        await _context.SaveChangesAsync();
        return true;
    }
}