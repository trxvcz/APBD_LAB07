using APBD_LAB07.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_LAB07.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(AppointmentService appointmentService) : ControllerBase
    {   
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(appointmentService.GetAll());
        }

        [HttpGet("{id:int}")]
        public IActionResult Get([FromQuery]int? id, [FromQuery] DateTime? dateTime, [FromQuery] string? status, [FromQuery] string? reason, [FromQuery]string? patientFullName, [FromQuery]string? patientEmail)
        {
            return Ok(appointmentService.GetAll(id, dateTime, status, reason, patientFullName, patientEmail));
        }
    }
}
