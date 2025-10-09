using Planday.Schedule.Application.Dtos;

namespace Planday.Schedule.Application.Interfaces.Services
{
    public interface IShiftWriteService
    {
        Task<ShiftDto> CreateShiftAsync(ShiftCreateDto shiftCreateDto, CancellationToken cancellationToken = default);
        Task<ShiftDto> AssignShiftAsync(long shiftId, long employeeId, CancellationToken cancellationToken = default);

    }
}
