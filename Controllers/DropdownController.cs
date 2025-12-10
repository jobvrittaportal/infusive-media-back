using Infusive_back.EntityData;
using Microsoft.AspNetCore.Mvc;
using static Infusive_back.Controllers.IndustryTypeController;

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

        [HttpGet]
        [Route("IndustryType")]
        public IActionResult GetIndustryTypes()
        {
            try
            {

                var query = (from ityp in db.IndustryType
                             select new
                             {
                                 IndustryId = ityp.Id,
                                 IndustryName = ityp.IndustryName ?? "",
                             });

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("country")]
        public IActionResult GetCountryDropdown()
        {
            try
            {
                var countries = db.Country.Select(r => new {
                    CountryId = r.Id,
                    CountryName = r.Name ,
                }).ToList();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("state/{countryId}")]
        public IActionResult GetStatesByCountry(int countryId)
        {
            try
            {
                if (countryId <= 0)
                {
                    return BadRequest(new { error = "Country ID is required" });
                }

                var states = db.State
                    .Where(s => s.CountryId == countryId)
                    .Select(s => new
                    {
                        StateId = s.Id,
                        StateName = s.Name
                    }).ToList();

                return Ok(states);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("city/{stateId}")]
        public IActionResult GetCitiesByState(int stateId)
        {
            try
            {
                if (stateId <= 0)
                {
                    return BadRequest(new { error = "State ID is required" });
                }

                var citites = db.City
                    .Where(s => s.StateId == stateId)
                    .Select(s => new
                    {
                        CityId = s.Id,
                        CityName = s.Name
                    }).ToList();

                return Ok(citites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("countrycodedropdown")]
        public IActionResult GetCountryCodeDropdown()
        {
            try
            {
                var countries = db.Country.Select(r => new
                {
                    CountryId = r.Id,
                    countryName = r.Name,
                    flag = r.FlagUrl,
                    dialCode = r.DialCode,
                    code = r.Code,

                }).ToList();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
