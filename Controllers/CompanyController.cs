using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Infusive_back.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CompanyController : ControllerBase
  {
    readonly private MyDbContext db;
    private readonly JwtAuth.ITokenManager tokenManager;
    public CompanyController(MyDbContext db, JwtAuth.ITokenManager tokenManager)
    {
      this.db = db;
      this.tokenManager = tokenManager;
    }



    [HttpGet]
    [Authorize]
    public IActionResult GetCompany([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
    {
      try
      {
        var query = db.Company.Include(u => u.IndustryType)
            .Select(user => new
            {
              user.Id,
              user.CompanyName,
              user.PhoneCountryCode,
              user.CompanyPhone,
              user.CompanyEmail,
              WebsiteUrl = user.WebsiteUrl,
              Feid = user.Feid,
              AddressLine = user.AddressLine,
              PostalZipCode = user.PostalZipCode,
              IndustryType = user.IndustryType.IndustryName.ToList()
            });
        if (!string.IsNullOrEmpty(search))
        {
          var lower = search.ToLower();
          query = query.Where(u =>
              u.CompanyName.ToLower().Contains(lower)
          );
        }
        var totalCount = query.Count();
        var company = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();
        return Ok(new { totalCount = totalCount, companies = company });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddCompany([FromBody] CompanyDto company)
    {
      try
      {
        var existingCompany = db.Company.FirstOrDefault(u => u.CompanyName == company.CompanyName);
        if (existingCompany != null)
          return BadRequest("Company already exists !");

        var newCompany = new Company
        {
          CompanyName = company.CompanyName,
          IndustrytypeId = company.IndustrytypeId,
          PhoneCountryCode = company.PhoneCountryCode,
          CompanyPhone = company.CompanyPhone,
          CompanyEmail = company.CompanyEmail,
          WebsiteUrl = company.WebsiteUrl,
          Feid = company.Feid,
          AddressLine = company.AddressLine,
          PostalZipCode = company.PostalZipCode,
        };
        db.Company.Add(newCompany);
        db.SaveChanges();
        return Ok(new { message = "Posted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


    [HttpPut]
    [Authorize]
    public IActionResult UpdateCompany([FromBody] UpdateCompanyDto companyDto)
    {
      try
      {

        var comapny = db.Company.SingleOrDefault(r => r.Id == companyDto.Id);
        if (comapny == null)
        {
          return NotFound(new { error = "Company not found" });
        }

        Company? alreadyExist = db.Company.SingleOrDefault(f => f.CompanyName.Trim().Equals(companyDto.CompanyName.Trim(), StringComparison.CurrentCultureIgnoreCase)&&  f.Id != comapny.Id);
        if (alreadyExist != null)
        {
          return BadRequest(new { error = "Company is already exist" });
        }

        comapny.CompanyName = companyDto.CompanyName;
        comapny.IndustrytypeId = companyDto.IndustrytypeId;
        comapny.PhoneCountryCode = companyDto.PhoneCountryCode;
        comapny.CompanyPhone = companyDto.CompanyPhone;
        comapny.CompanyEmail = companyDto.CompanyEmail;
        comapny.WebsiteUrl = companyDto.WebsiteUrl;
        comapny.Feid = companyDto.Feid;
        comapny.AddressLine = companyDto.AddressLine;
        comapny.PostalZipCode = companyDto.PostalZipCode;


        db.SaveChanges();
        return Ok(new { message = "Updated successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteCompany(int id)
    {
      try
      {
        var company = db.Company.SingleOrDefault(r => r.Id == id);
        if (company == null)
        {
          return NotFound(new { error = "Company not found" });
        }
        db.Company.Remove(company);
        db.SaveChanges();

        return Ok(new { message = "Company deleted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

    [HttpGet("dropdown/{Id}")]
    public IActionResult GetCompany()
    {
      try
      {
      

        var companies = db.Company
            .Select(s => new
            {
              Id = s.Id,
              Name = s.CompanyName
            }).ToList();

        return Ok(companies);
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

    [HttpPut("toggle/{id}")]
    [Authorize]
    public IActionResult ToggleStatus(int id)
    {
      try
      {
        var company = db.Company.SingleOrDefault(r => r.Id == id);
        if (company == null)
        {
          return NotFound(new { error = "Company not found" });
        }
        company.Status = !company.Status;
        db.Company.Update(company);
        db.SaveChanges();

        return Ok(new { message = "Status changed successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }

  }

  public class CompanyDto
  {
    public required string CompanyName { get; set; }
    public required int IndustrytypeId { get; set; }
    public required string PhoneCountryCode { get; set; }
    public required string CompanyPhone { get; set; }
    public required string CompanyEmail { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Feid { get; set; }
    public required string AddressLine { get; set; }
    public required string PostalZipCode { get; set; }
  }

  public class UpdateCompanyDto
  {
    public required int Id { get; set; }
    public required string CompanyName { get; set; }
    public int IndustrytypeId { get; set; }
    public required string PhoneCountryCode { get; set; }
    public required string CompanyPhone { get; set; }
    public required string CompanyEmail { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Feid { get; set; }
    public required string AddressLine { get; set; }
    public required string PostalZipCode { get; set; }
  }
}
