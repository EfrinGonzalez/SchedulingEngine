using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries
{ 
    public interface IGetShiftByIdQuery
    {
        Task<Shift?> GetShiftByIdAsync(long id, CancellationToken cancellationToken);
    }
}