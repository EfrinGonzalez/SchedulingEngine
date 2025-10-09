using Planday.Schedule.Application.Dtos;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using Planday.Schedule.Application.Interfaces.Services;
using Planday.Schedule.Domain.Entities;


namespace Planday.Schedule.Application.Services
{
    public class ShiftReadService : IShiftReadService
    {
        private readonly IGetAllShiftsQuery _getAllShiftsQuery;
        private readonly IGetShiftByIdQuery _getShiftByIdQuery;
        private readonly IEmployeeInfoService _employeeInfoService;


        public ShiftReadService(
             IGetAllShiftsQuery getAllShiftsQuery,
             IGetShiftByIdQuery getShiftByIdQuery,
             IEmployeeInfoService employeeInfoService)
        {
            _getAllShiftsQuery = getAllShiftsQuery;
            _getShiftByIdQuery = getShiftByIdQuery;
            _employeeInfoService = employeeInfoService;
        }
        public async Task<IReadOnlyList<ShiftDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var shifts = await _getAllShiftsQuery.GetAllShiftsAsync(cancellationToken);
            var result = new List<ShiftDto>(shifts.Count);

            foreach (var shift in shifts)
            {
                var email = await GetEmployeeEmailAsync(shift.EmployeeId, cancellationToken);
                result.Add(MapToDto(shift, email));
            }

            return result;
        }

        public async Task<ShiftDto?> GetShiftByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var shift = await _getShiftByIdQuery.GetShiftByIdAsync(id, cancellationToken);
            if (shift == null)
            {
                return null;
            }

            var email = await GetEmployeeEmailAsync(shift.EmployeeId, cancellationToken);
            return MapToDto(shift, email);
        }

        private async Task<string?> GetEmployeeEmailAsync(long? employeeId, CancellationToken cancellationToken)
        {
            if (!employeeId.HasValue)
            {
                return null;
            }

            return await _employeeInfoService.GetEmployeeEmailAsync(employeeId.Value, cancellationToken);
        }

        private static ShiftDto MapToDto(Shift shift, string? employeeEmail)
        {
            return new ShiftDto(shift.Id, shift.EmployeeId, shift.Start, shift.End, employeeEmail);
        }
    }
}
