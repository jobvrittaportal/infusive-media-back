using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPocController : ControllerBase
    {
        readonly private MyDbContext db;
        private readonly JwtAuth.ITokenManager tokenManager;
        public CompanyPocController(MyDbContext db, JwtAuth.ITokenManager tokenManager)
        {
            this.db = db;
            this.tokenManager = tokenManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetPoc([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search, [FromQuery] int? companyId)
        {
            try
            {
                var query = db.CompanyPoc.Include(u => u.Company).Include(u => u.Designation)
                    .Select(user => new
                    {
                        user.Id,
                        Name = user.Name,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company.CompanyName,
                        Email = user.Email,
                        PhoneCountryCode = user.PhoneCountryCode,
                        PhoneNumber = user.PhoneNumber,
                        Whatsapp = user.Whatsapp,
                        DesignationName = user.Designation.DesignationName,
                        DesignationId = user.DesignationId,
                        LinkedinUrl = user.LinkedinUrl,
                        //CompanyName = user.Company.CompanyName.ToList()
                    });

                if (companyId.HasValue)
                {
                    query = query.Where(u => u.CompanyId == companyId);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLower();
                    query = query.Where(u =>
                        u.Name.ToLower().Contains(lower)
                    );
                }

                var totalCount = query.Count();
                var poc = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();
                return Ok(new { totalCount = totalCount, pocs = poc });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddPOC([FromBody] PocDto poc)
        {
            try
            {
                var existingPoc = db.CompanyPoc.FirstOrDefault(u => u.PhoneNumber == poc.PhoneNumber || u.Email == poc.Email);
                if (existingPoc != null)
                    return BadRequest("This Poc already exists !");

                var newPoc = new CompanyPoc
                {
                    Name = poc.Name,
                    CompanyId = poc.CompanyId,
                    Email = poc.Email,
                    PhoneCountryCode = poc.PhoneCountryCode,
                    PhoneNumber = poc.PhoneNumber,
                    Whatsapp = poc.Whatsapp,
                    DesignationId = poc.DesignationId,
                    LinkedinUrl = poc.LinkedinUrl,
                };
                db.CompanyPoc.Add(newPoc);
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
        public IActionResult UpdatePoc([FromBody] UpdatePocDto updatePoc)
        {
            try
            {

                var companyPoc = db.CompanyPoc.SingleOrDefault(r => r.Id == updatePoc.Id);
                if (companyPoc == null)
                {
                    return NotFound(new { error = "POC not found" });
                }

                CompanyPoc? alreadyExist = db.CompanyPoc
                            .Where(f => f.Id != updatePoc.Id)
                            .Where(f =>
                                f.Email.Trim().ToLower() == updatePoc.Email.Trim().ToLower() ||
                                f.PhoneNumber.Trim().ToLower() == updatePoc.PhoneNumber.Trim().ToLower()
                            ).FirstOrDefault();

                if (alreadyExist != null)
                {
                    return BadRequest(new { error = "poc is already exist" });
                }

                companyPoc.Name = updatePoc.Name;
                companyPoc.CompanyId = updatePoc.CompanyId;
                companyPoc.Email = updatePoc.Email;
                companyPoc.PhoneCountryCode = updatePoc.PhoneCountryCode;
                companyPoc.PhoneNumber = updatePoc.PhoneNumber;
                companyPoc.Whatsapp = updatePoc.Whatsapp;
                companyPoc.DesignationId = updatePoc.DesignationId;
                companyPoc.LinkedinUrl = updatePoc.LinkedinUrl;



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
        public IActionResult DeletePocs(int id)
        {
            try
            {
                var poc = db.CompanyPoc.SingleOrDefault(r => r.Id == id);
                if (poc == null)
                {
                    return NotFound(new { error = "POCs not found" });
                }
                db.CompanyPoc.Remove(poc);
                db.SaveChanges();

                return Ok(new { message = "POCs deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }


    public class PocDto
    {
        public required string Name { get; set; }
        public int CompanyId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
        public string? Whatsapp { get; set; }
        public int? DesignationId { get; set; }
        public string? LinkedinUrl { get; set; }
    }

    public class UpdatePocDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public int CompanyId { get; set; }
        public string? Email { get; set; }
        public string? PhoneCountryCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Whatsapp { get; set; }
        public int? DesignationId { get; set; }
        public string? LinkedinUrl { get; set; }
    }
}
