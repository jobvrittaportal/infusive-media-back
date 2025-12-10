using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Infusive_back.Controllers.RoleController;
using System.Text.Json;

namespace Infusive_back.Controllers
{

  [Route("api/[controller]")]
  [ApiController]
  public class CountryController(MyDbContext db) : ControllerBase
  {
    private readonly MyDbContext db = db;

    [HttpGet]
    [Authorize]
    public IActionResult GetCountry([FromQuery] string? text, [FromQuery] string? lazyParams)
    {
      try
      {
        int first = 0, rows = 10;
        string sortField = "Id";
        int sortOrder = -1;

        if (!string.IsNullOrEmpty(lazyParams))
        {
          var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
          var parsed = JsonSerializer.Deserialize<LazyParams>(lazyParams, options);
          if (parsed != null)
          {
            first = parsed.First;
            rows = parsed.Rows;
            sortField = parsed.SortField ?? "Id";
            sortOrder = parsed.SortOrder ?? -1;
          }
        }

        var query = db.Country.AsQueryable();
        if (!string.IsNullOrEmpty(text))
        {
          query = query.Where(r => r.Name.Contains(text));
        }
        var totalRecords = query.Count();

        query = query.OrderByDynamic(sortField, sortOrder == 1);

        var result = query.Skip(first).Take(rows).ToList();

        return Ok(new { countries = result, TotalCount = totalRecords });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddCountry(Country country)
    {
      try
      {
        Country? alreadyExist = db.Country.SingleOrDefault(f => f.Name == country.Name);

        if (alreadyExist != null)
        {
          return BadRequest(new { error = "Already Exist" });
        }
        else
        {
          db.Country.Add(country);
          db.SaveChanges();

          return Ok(new { country });
        }
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult EditCountry(int id, [FromBody] Country updatedData)
    {
      try
      {
        if (id <= 0)
        {
          return BadRequest(new { error = "ID is required" });
        }
        if (string.IsNullOrWhiteSpace(updatedData.Name))
        {
          return BadRequest(new { error = "Country is required" });
        }
        var country = db.Country.SingleOrDefault(r => r.Id == id);
        if (country == null)
        {
          return NotFound(new { error = "Country not found" });
        }
        country.Name = updatedData.Name;
        country.Code = updatedData.Code;
        country.DialCode = updatedData.DialCode;
        country.FlagUrl = updatedData.FlagUrl;
        db.SaveChanges();
        return Ok(new { country });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteCountry(int id)
    {
      try
      {
        var country = db.Country.SingleOrDefault(r => r.Id == id);
        if (country == null)
        {
          return NotFound(new { error = "Country not found" });
        }
        db.Country.Remove(country);
        db.SaveChanges();

        return Ok(new { message = "Country deleted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


    [HttpGet("dropdown")]
    public IActionResult GetCountryDropdown()
    {
      try
      {
        var countries = db.Country.Select(r => new { r.Id, r.Name }).ToList();
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

    [HttpGet("countrycodedropdown")]
    public IActionResult GetCountryCodeDropdown()
    {
      try
      {
        var countries = db.Country.Select(r => new
        {
          CountryId = r.Id,
          countryName = r.Name,
          flag=r.FlagUrl,
          dialCode=r.DialCode,
          code=r.Code,

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
