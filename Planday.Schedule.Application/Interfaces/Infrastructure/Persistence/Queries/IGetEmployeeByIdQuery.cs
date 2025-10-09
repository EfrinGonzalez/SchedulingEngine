using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries
{
    public interface IGetEmployeeByIdQuery
    {
        Task<Employee?> GetEmployeeByIdAsync(long employeeId, CancellationToken cancellationToken = default);
    }
}
