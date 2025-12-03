using Infusive_back.EntityData;
using Microsoft.AspNetCore.Mvc;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;


        [HttpGet]
        [Route("getRoles")]

        public IActionResult GetRole()
        {
            try
            {
                var roles = db.Role
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                    }).ToList();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}