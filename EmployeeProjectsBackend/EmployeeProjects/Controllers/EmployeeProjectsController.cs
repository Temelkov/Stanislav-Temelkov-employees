using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace EmployeeProjects.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeProjectsController : ControllerBase
    {
        private readonly IEmployeeProjectService _service;

        public EmployeeProjectsController(IEmployeeProjectService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var csvContent = await reader.ReadToEndAsync();
                var projects = await _service.ParseProjectsAsync(csvContent);
                var results = await _service.GetLongestWorkingPairsAsync(projects);

                return Ok(results);
            }
        }
    }
}
