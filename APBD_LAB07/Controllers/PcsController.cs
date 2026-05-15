using APBD_LAB07.DTOs;
using APBD_LAB07.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD_LAB07.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PcsController : ControllerBase
    {
        private readonly IPcsService _pcsService;

        public PcsController(IPcsService pcsService)
        {
            _pcsService = pcsService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _pcsService.GetAllPcsAsync();
            return Ok(res);
        }

        [HttpGet("{id}/components")]
        public async Task<IActionResult> GetByIdWithComponents(int id)
        {
            var res = await _pcsService.GetPcWithComponentsAsync(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateUpdatePcDto dto)
        {
            var res = await _pcsService.CreatePcAsync(dto);
            return CreatedAtAction(nameof(GetByIdWithComponents), new { id = res.Id }, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]CreateUpdatePcDto dto)
        {
            var succes = await _pcsService.UpdatePcAsync(id, dto);
            if (!succes) return NotFound();
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var succes = await _pcsService.DeletePcAsync(id);
            if (!succes) return NotFound();
            return NoContent();
        }
}
}
