using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Infusive_back.Controllers.RoleController;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Infusive_back.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StateController(MyDbContext db) : ControllerBase
  {

    private readonly MyDbContext db = db;

    [HttpGet]
    [Authorize]
    public IActionResult GetState([FromQuery] string? text, [FromQuery] string? lazyParams)
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

        var query = db.State.Include(u => u.Country).AsQueryable();

        if (!string.IsNullOrEmpty(text))
        {
          query = query.Where(u => u.Name.Contains(text));
        }

        var totalRecords = query.Count();
        if (sortOrder == 1) query = query.OrderByDynamic(sortField, true);
        else query = query.OrderByDynamic(sortField, false);

        var states = query.Skip(first).Take(rows).Select(f => new
        {
          f.Id,
          f.Name,
          f.CountryId,
          CountryName = f.Country.Name,
        }).ToList();

        return Ok(new { states = states, count = totalRecords });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddState(AddStateDto dto)
    {
      try
      {
        State? alreadyExist = db.State.SingleOrDefault(f => f.Name == dto.Name && f.CountryId == dto.CountryId);

        if (alreadyExist != null)
        {
          return BadRequest(new { error = "Already Exist" });
        }
        else
        {
          State state = new State
          {
            Name = dto.Name,
            CountryId = dto.CountryId
          };
          db.State.Add(state);
          db.SaveChanges();

          return Ok(new { state });
        }
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    [HttpPut]
    [Authorize]
    public IActionResult EditState(int id, [FromBody] AddStateDto updatedData)
    {
      try
      {
      
        var state = db.State.SingleOrDefault(r => r.Id == id);
        if (state == null)
        {
          return NotFound(new { error = "State not found" });
        }
        state.Name = updatedData.Name;
        state.CountryId = updatedData.CountryId;
        db.SaveChanges();
        return Ok(new { state });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteState(int id)
    {
      try
      {
        var state = db.State.SingleOrDefault(r => r.Id == id);
        if (state == null)
        {
          return NotFound(new { error = "State not found" });
        }
        db.State.Remove(state);
        db.SaveChanges();

        return Ok(new { message = "State deleted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


    [HttpGet("dropdown/{countryId}")]
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
              Id = s.Id,
              Name = s.Name
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

  }

  public class AddStateDto
  {
    [MaxLength(255)]
    public required string Name { get; set; }
    public required int CountryId { get; set; }
  }

  public class UpdattateDto
  {
    [MaxLength(255)]
    public required string Name { get; set; }
    public required int Id { get; set; }
    public required int CountryId { get; set; }
  }

}
