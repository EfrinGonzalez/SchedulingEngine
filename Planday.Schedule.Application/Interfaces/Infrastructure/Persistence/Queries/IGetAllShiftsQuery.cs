using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries
{
    public interface IGetAllShiftsQuery
    {
        Task<IReadOnlyCollection<Shift>> GetAllShiftsAsync(CancellationToken cancellationToken = default);
    }    
}

