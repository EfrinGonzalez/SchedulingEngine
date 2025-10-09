using Planday.Schedule.Application.Dtos;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Services;


namespace Planday.Schedule.Application.Services
{
    public class ShiftWriteService(ICreateOpenShiftCommand createOpenShiftCommand,
        IAssignShiftCommand assignShiftCommand,
        IGetShiftByIdQuery getShiftByIdQuery,
        IGetEmployeeByIdQuery getEmployeeByIdQuery,
        IEmployeeHasOverlappingShiftQuery employeeHasOverlappingShiftQuery) : IShiftWriteService
    {
        private readonly ICreateOpenShiftCommand _createOpenShiftCommand = createOpenShiftCommand;
        private readonly IAssignShiftCommand _assignShiftCommand = assignShiftCommand;
        private readonly IGetShiftByIdQuery _getShiftByIdQuery = getShiftByIdQuery;
        private readonly IGetEmployeeByIdQuery _getEmployeeByIdQuery = getEmployeeByIdQuery;
        private readonly IEmployeeHasOverlappingShiftQuery _employeeHasOverlappingShiftQuery = employeeHasOverlappingShiftQuery;

        public async Task<ShiftDto> CreateShiftAsync(ShiftCreateDto shiftCreateDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(shiftCreateDto);

            if (shiftCreateDto.Start >= shiftCreateDto.End)
            {
                throw new ArgumentException("The shift start time must be earlier than the end time.", nameof(shiftCreateDto));
            }

            if (shiftCreateDto.Start.Date != shiftCreateDto.End.Date)
            {
                throw new ArgumentException("The shift must start and end on the same day.", nameof(shiftCreateDto));
            }

            var shift = await _createOpenShiftCommand.CreateOpenShiftAsync(
                shiftCreateDto.Start,
                shiftCreateDto.End,
                cancellationToken);

            return new ShiftDto(shift.Id, shift.EmployeeId, shift.Start, shift.End);
        }

        public async Task<ShiftDto> AssignShiftAsync(long shiftId, long employeeId, CancellationToken cancellationToken = default)
        {
            if (shiftId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shiftId), "The shift identifier must be a positive value.");
            }

            if (employeeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(employeeId), "The employee identifier must be a positive value.");
            }

            var shift = await _getShiftByIdQuery.GetShiftByIdAsync(shiftId, cancellationToken);
            if (shift is null)
            {
                throw new KeyNotFoundException($"Shift '{shiftId}' does not exist.");
            }

            var employee = await _getEmployeeByIdQuery.GetEmployeeByIdAsync(employeeId, cancellationToken);
            if (employee is null)
            {
                throw new KeyNotFoundException($"Employee '{employeeId}' does not exist.");
            }

            if (shift.EmployeeId.HasValue && shift.EmployeeId != employeeId)
            {
                throw new InvalidOperationException("The shift is already assigned to a different employee.");
            }

            if (shift.EmployeeId == employeeId)
            {
                return new ShiftDto(shift.Id, shift.EmployeeId, shift.Start, shift.End);
            }

            var hasOverlap = await _employeeHasOverlappingShiftQuery.HasOverlappingShiftAsync(
                employeeId,
                shift.Start,
                shift.End,
                shift.Id,
                cancellationToken);

            if (hasOverlap)
            {
                throw new InvalidOperationException("The employee already has a shift that overlaps with the requested time span.");
            }

            await _assignShiftCommand.AssignShiftAsync(shift.Id, employeeId, cancellationToken);

            return new ShiftDto(shift.Id, employeeId, shift.Start, shift.End);
        }
    }
}
