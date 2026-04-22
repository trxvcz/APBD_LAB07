using APBD_LAB07.DTOs;
using APBD_LAB07.Services;
using Microsoft.AspNetCore.Mvc;
using APBD_LAB07.Exceptions;

namespace APBD_LAB07.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(AppointmentService appointmentService) : ControllerBase
{
    private readonly AppointmentService _appointmentService = appointmentService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status, [FromQuery] string? patientLastName)
    {
        
        if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(patientLastName))
        {
            var res =  await _appointmentService.GetAll();

            if (res.Count == 0)
            {
                return NotFound(res);
            }
            else
            {
                return Ok(res);
            }
        }
        else
        {
            var res = await _appointmentService.GetAll(status, patientLastName);
            if (res.Count == 0)
            {
                return NotFound(res);
            }
            else
            {
                return Ok(res);
            }
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _appointmentService.GetById(id);
        if (result == null)
        {
            return NotFound(result);
        }else
        {
            return Ok(result);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAppointmentRequestDto appointmentRequest)
    {
        try
        {
            var res = await _appointmentService.Create(appointmentRequest);
            return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponseDto { Message = "Wystąpił nieoczekiwany błąd serwera." });
        }

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateAppointmentRequestDto appointmentRequest)
    {

        try
        {
            var res = await _appointmentService.Put(id, appointmentRequest);
            return Ok(res);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponseDto { Message = "Wystąpił nieoczekiwany błąd serwera.", Type = ex.GetType().ToString() });
        }
        
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        try
        {
            await _appointmentService.DeleteAppointment(id);
            return NoContent();
        }
        catch (ConflictException ex) {
            return Conflict(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        catch (NotFoundException ex) {
            return NotFound(new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }catch(Exception ex) {
            return StatusCode(500, new ErrorResponseDto { Message = ex.Message, Type = ex.GetType().ToString() });
        }
        
        
    }
}