using APBD_LAB07.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_LAB07.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(AppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<OkObjectResult> Get([FromQuery] string? status, [FromQuery] string? patientLastName)
    {
        
        if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(patientLastName))
        {
            var res =  await appointmentService.GetAll();    
            return Ok(res);
        }
        else
        {
            var res = await appointmentService.GetAll(status, patientLastName);
            return Ok(res);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<OkObjectResult> GetById(int id)
    {
        var result = await appointmentService.GetById(id);
        return Ok(result);
    }
}