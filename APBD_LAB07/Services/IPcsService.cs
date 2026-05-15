using APBD_LAB07.DTOs;

namespace APBD_LAB07.Services;

public interface IPcsService
    {
        Task<IEnumerable<PcDto>> GetAllPcsAsync();
        Task<PcDetailsDto?> GetPcWithComponentsAsync(int id);
        Task<PcDto> CreatePcAsync(CreateUpdatePcDto dto);
        Task<bool> UpdatePcAsync(int id, CreateUpdatePcDto dto);
        Task<bool> DeletePcAsync(int id);
    }
