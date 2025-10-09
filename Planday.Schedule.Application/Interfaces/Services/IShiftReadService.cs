using Planday.Schedule.Application.Dtos;

namespace Planday.Schedule.Application.Interfaces.Services
{
    public interface IShiftReadService
    {
        Task<IReadOnlyList<ShiftDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ShiftDto?> GetShiftByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
