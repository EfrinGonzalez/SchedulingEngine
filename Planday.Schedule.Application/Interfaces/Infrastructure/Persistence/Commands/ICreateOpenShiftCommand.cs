using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands
{
    public interface ICreateOpenShiftCommand
    {
        Task<Shift> CreateOpenShiftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    }
}
