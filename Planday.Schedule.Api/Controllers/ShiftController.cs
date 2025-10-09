using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Application.Dtos;
using Planday.Schedule.Application.Interfaces.Services;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("api/shifts")]
    public class ShiftController(IShiftReadService shiftReadService, IShiftWriteService shiftWriteService) : ControllerBase
    {
        private readonly IShiftReadService _shiftReadService = shiftReadService;
        private readonly IShiftWriteService _shiftWriteService = shiftWriteService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ShiftDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var shifts = await _shiftReadService.GetAllAsync(cancellationToken);
            return Ok(shifts);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ShiftDto>> GetShiftByID(long id, CancellationToken cancellationToken)
        {
            var shift = await _shiftReadService.GetShiftByIdAsync(id, cancellationToken);

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }      

        [HttpPost]
        public async Task<ActionResult<ShiftDto>> CreateOpenShift([FromBody] ShiftCreateDto shiftCreateDto, CancellationToken cancellationToken)
        {
            if (shiftCreateDto is null)
            {
                return BadRequest("The request body is required.");
            }
            try
                {                 
                    var createdShift = await _shiftWriteService.CreateShiftAsync(shiftCreateDto, cancellationToken);
                    return CreatedAtAction(nameof(GetShiftByID), new { id = createdShift.Id }, createdShift);
                }
                catch (ArgumentNullException)
                {
                   
                    return BadRequest("The request body is required.");
                }         
                catch (ArgumentException ex)
                {
                   
                     return BadRequest(ex.Message);
                }          

        }

        [HttpPost("{id:long}/assign")]
        public async Task<ActionResult<ShiftDto>> AssignShift(long id, [FromBody] ShiftAssignmentDto shiftAssignmentDto, CancellationToken cancellationToken)
        {
            if (shiftAssignmentDto is null)
            {
                return BadRequest("The request body is required.");
            }

            try
            {
                var shift = await _shiftWriteService.AssignShiftAsync(id, shiftAssignmentDto.EmployeeId, cancellationToken);
                return Ok(shift);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

