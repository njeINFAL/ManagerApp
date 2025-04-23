using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET service list
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _context.Services
                .Select(s => new ServiceDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    ServiceDurationMinutes = s.ServiceDurationMinutes,
                    ServicePrice = s.ServicePrice,
                    Status = "Available"
                })
                .ToListAsync();

            return Ok(services);
        }

    }
}
