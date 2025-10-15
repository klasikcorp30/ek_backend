using Ekklesia.Api.Models;
using Ekklesia.Api.Models.DTOs;

namespace Ekklesia.Api.Services.Interfaces
{
    public interface IChurchService
    {
        Task<IEnumerable<ChurchDto>> GetAllChurchesAsync(int page = 1, int pageSize = 20, string? status = null, string? city = null, string? state = null, string? denomination = null);
        Task<ChurchDto?> GetChurchByIdAsync(int id);
        Task<ChurchDto> CreateChurchAsync(CreateChurchDto createChurchDto, string createdBy);
        Task<ChurchDto?> UpdateChurchAsync(int id, UpdateChurchDto updateChurchDto, string updatedBy);
        Task<bool> DeleteChurchAsync(int id);
        Task<bool> UpdateChurchStatusAsync(int id, ChurchStatusUpdateDto statusUpdate, string updatedBy);
        Task<IEnumerable<ChurchDto>> SearchChurchesAsync(string searchTerm, double? latitude = null, double? longitude = null, double? radiusKm = null);
        Task<int> ImportChurchesFromCsvAsync(Stream csvStream, string importedBy);
    }
}