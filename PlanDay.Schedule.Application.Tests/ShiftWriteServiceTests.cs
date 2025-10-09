using Microsoft.VisualStudio.CodeCoverage;
using Moq;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Services;
using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Application.Tests;

[TestFixture]
public class ShiftWriteServiceTests
{
    private Mock<ICreateOpenShiftCommand> _createOpenShiftCommand = null!;
    private Mock<IAssignShiftCommand> _assignShiftCommand = null!;
    private Mock<IGetShiftByIdQuery> _getShiftByIdQuery = null!;
    private Mock<IGetEmployeeByIdQuery> _getEmployeeByIdQuery = null!;
    private Mock<IEmployeeHasOverlappingShiftQuery> _employeeHasOverlappingShiftQuery = null!;
    private ShiftWriteService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _createOpenShiftCommand = new Mock<ICreateOpenShiftCommand>();
        _assignShiftCommand = new Mock<IAssignShiftCommand>();
        _getShiftByIdQuery = new Mock<IGetShiftByIdQuery>();
        _getEmployeeByIdQuery = new Mock<IGetEmployeeByIdQuery>();
        _employeeHasOverlappingShiftQuery = new Mock<IEmployeeHasOverlappingShiftQuery>();

        _service = new ShiftWriteService(
            _createOpenShiftCommand.Object,
            _assignShiftCommand.Object,
            _getShiftByIdQuery.Object,
            _getEmployeeByIdQuery.Object,
            _employeeHasOverlappingShiftQuery.Object);
    }

        /*
            As the overlapping functinality is one of the critical business cases, here are some tests that cover overlappins.:

            Case | New Shift Start  | New Shift End  | Existing Shift Start  | Existing Shift End  | Expected Overlap
            -----|------------------|----------------|-----------------------|---------------------|------------------
            1    | 10:00            | 12:00          | 11:00                 | 13:00               | overlap (partial)
            2    | 10:00            | 12:00          | 09:00                 | 11:00               | overlap (partial)
            3    | 10:00            | 12:00          | 10:30                 | 11:30               | overlap (contained)
            4    | 10:00            | 12:00          | 08:00                 | 14:00               | overlap (contained within)
            5    | 10:00            | 12:00          | 12:00                 | 14:00               | no overlap (back-to-back)
        */

    [TestCase("2025-10-09T10:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T11:00:00Z", "2025-10-09T13:00:00Z", true, TestName = "Overlap_Partial_End")]
    [TestCase("2025-10-09T10:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T09:00:00Z", "2025-10-09T11:00:00Z", true, TestName = "Overlap_Partial_Start")]
    [TestCase("2025-10-09T10:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T10:30:00Z", "2025-10-09T11:30:00Z", true, TestName = "Overlap_Contained")]
    [TestCase("2025-10-09T10:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T08:00:00Z", "2025-10-09T14:00:00Z", true, TestName = "Overlap_Wrapped")]
    [TestCase("2025-10-09T10:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T12:00:00Z", "2025-10-09T14:00:00Z", false, TestName = "NoOverlap_BackToBack")]
    public async Task AssignShiftAsync_Throws_WhenEmployeeHasOverlappingShift(
        string newStartStr,
        string newEndStr,
        string existingStartStr,
        string existingEndStr,
        bool expectedOverlap)
    {
        // Arrange
        var newShiftStart = DateTime.Parse(newStartStr);
        var newShiftEnd = DateTime.Parse(newEndStr);
        var existingShiftStart = DateTime.Parse(existingStartStr);
        var existingShiftEnd = DateTime.Parse(existingEndStr);

        var shift = new Shift(3, null, newShiftStart, newShiftEnd);
        var employee = new Employee(7, "Test");

        _getShiftByIdQuery
            .Setup(q => q.GetShiftByIdAsync(shift.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);

        _getEmployeeByIdQuery
            .Setup(q => q.GetEmployeeByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _employeeHasOverlappingShiftQuery
            .Setup(q => q.HasOverlappingShiftAsync(7, shift.Start, shift.End, shift.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOverlap);

        // Act & Assert
        if (expectedOverlap)
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AssignShiftAsync(shift.Id, 7));

            Assert.That(ex?.Message, Is.EqualTo("The employee already has a shift that overlaps with the requested time span."));
        }
        else
        {
            Assert.DoesNotThrowAsync(async () => await _service.AssignShiftAsync(shift.Id, 7));
        }
    }
}