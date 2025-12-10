
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Infusive_back.EntityData;
using Infusive_back.Models;
using static Infusive_back.Controllers.RoleController;


namespace Infusive_back.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CityController(MyDbContext db, IWebHostEnvironment webHostEnvironment) : ControllerBase
  {

    private readonly MyDbContext db = db;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;


    [HttpGet]
    [Authorize]
    public IActionResult GetCity([FromQuery] string? text, [FromQuery] string? lazyParams)
    {
      try
      {
        int first = 0, rows = 10, page = 1;
        string sortField = "Id";
        int? sortOrder = -1;
        if (!string.IsNullOrEmpty(lazyParams))
        {
          try
          {
            var lazy = JsonConvert.DeserializeObject<LazyParams>(lazyParams);
            if (lazy != null)
            {
              first = lazy.First;
              rows = lazy.Rows;
              page = lazy.Page;
              sortField = lazy.SortField ?? "Id";
              sortOrder = lazy.SortOrder;
            }
          }
          catch (Exception)
          {
            return BadRequest(new { error = "Invalid lazyParams format" });
          }
        }

        var query = db.City.Include(c => c.State).ThenInclude(s => s.Country).AsQueryable();

        if (!string.IsNullOrEmpty(text))
        {
          query = query.Where(u => u.Name.Contains(text) || u.State.Name.Contains(text));
        }

        var totalRecords = query.Count();
        if (sortOrder == 1) query = query.OrderByDynamic(sortField, true);
        else query = query.OrderByDynamic(sortField, false);

        var cities = query.Skip(first).Take(rows).Select(f => new
        {
          f.Id,
          f.Name,
          f.StateId,
          StateName = f.State.Name,
          CountryId = f.State.CountryId,
          CountryName = f.State.Country.Name,
       
          f.Description,
         
        }).ToList();

        return Ok(new { cities, count = totalRecords });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddCity(AddCityDto dto)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
          return BadRequest(new { error = "City name is required." });
        }
        dto.Name = dto.Name.Trim();

        City? alreadyExist = db.City.FirstOrDefault(f => f.Name == dto.Name && f.StateId == dto.StateId);

        if (alreadyExist != null)
        {
          return BadRequest(new { error = "Already Exist" });
        }
        else
        {
          City city = new City
          {
            Name = dto.Name,
            StateId = dto.StateId,
            Description = dto.Description
          };
          db.City.Add(city);
          db.SaveChanges();

          return Ok(new { city });
        }
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    [HttpPut]
    public IActionResult EditCity( [FromBody] AddCityDto updatedData)
    {
      try
      {
       
        if (string.IsNullOrWhiteSpace(updatedData.Name))
        {
          return BadRequest(new { error = "City name is required." });
        }
        updatedData.Name = updatedData.Name.Trim();
        bool cityExists = db.City.Any(c =>
          c.Name.ToLower() == updatedData.Name.ToLower() &&
          c.StateId == updatedData.StateId &&
          c.Id != updatedData.Id);

        if (cityExists)
        {
          return BadRequest(new { error = "A city with this name already exists in the selected state." });
        }

        var city = db.City.SingleOrDefault(r => r.Id == updatedData.Id);
        if (city == null)
        {
          return NotFound(new { error = "City not found" });
        }
        city.Name = updatedData.Name;
        city.StateId = updatedData.StateId;
      
        city.Description = updatedData.Description;
      

        db.SaveChanges();
        return Ok(new { city });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteCity(int id)
    {
      try
      {
        var city = db.City.SingleOrDefault(r => r.Id == id);
        if (city == null)
        {
          return NotFound(new { error = "City not found" });
        }
        db.City.Remove(city);
        db.SaveChanges();

        return Ok(new { message = "City deleted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


    [HttpGet("dropdown/{stateId}")]
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
              Id = s.Id,
              Name = s.Name
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




  }

  public class AddCityDto
  {
    [MaxLength(255)]
    public required string Name { get; set; }
    public required int StateId { get; set; }
    public int? Id { get; set; }
    public string? Description { get; set; }
  }






}
