using APBD_LAB07.DTOs;
using APBD_LAB07.Models;
using APBD_LAB07.Services;
using Microsoft.AspNetCore.Mvc;

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

    //[HttpPost]
    //public async Task<CreatedAtActionResult> Post([FromBody] CreateAppointmentRequestDto appointmentRequest)
    //{
       // return Ok( await _appointmentService.Create(appointmentRequest));
        


        
   // }
}